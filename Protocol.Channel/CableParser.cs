using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using Hydrology.Entity;
using Protocol.Channel.Interface;
using System.Threading;
using Protocol.Data.Interface;
using Protocol.Channel.Reservoir;
using Protocol.Data.ZYJBX;

using System.IO;
using Protocol.Manager;

/************************************************************************************
* Copyright (c) 2018 All Rights Reserved.
*命名空间：Protocol.Channel
*文件名： CableParder
*创建人： XXX
*创建时间：2018-12-28 15:30:41
*描述
*=====================================================================
*修改标记
*修改时间：2018-12-28 15:30:41
*修改人：XXX
*描述：
************************************************************************************/

namespace Protocol.Channel.Cable
{
    public class CableParser : ICable
    {
        #region 属性与变量
        public SerialPort Port { get; set; }
        public IUp Up { get; set; }

        private List<string> m_listDatas;
        private List<byte> m_inputBuffer;
        private Semaphore m_semephoreData;



        private EChannelType m_channelType = EChannelType.Cable;
        private EListeningProtType m_portType = EListeningProtType.SerialPort;

        // 多线程相关
       
        // 用来唤醒数据处理线程
        private Thread m_threadDealData;    // 处理数据的进程 
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public CableParser()
        {
            // 构造函数，开启数据处理进程

            m_semephoreData = new Semaphore(0, Int32.MaxValue);

            m_threadDealData = new Thread(new ThreadStart(DealData))
            {
                Name = "Cable处理线程"
            };
            m_threadDealData.Start();
        }
        /// <summary>
        /// 初始话
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        public void Init(string portName, int baudRate)
        {
            //InvokeMessage(String.Format("初始化串口{0}...", portName), "初始化");
            //  初始化串口信息
            this.Port = new SerialPort()
            {
                PortName = portName,
                BaudRate = baudRate,
                StopBits = StopBits.One,
                DataBits = 8,
                Parity = Parity.None,
            };
            this.Port.DataReceived += new SerialDataReceivedEventHandler(Port_DataReceived);
            this.m_listDatas = new List<string>();
            this.m_inputBuffer = new List<byte>();
        }

        public void InitInterface(IUp up, IDown down, IUBatch udisk, IFlashBatch flash, ISoil soil)
        {
            this.Up = up;
            //this.Down = down;
            //this.UBatch = udisk;
            //this.FlashBatch = flash;
            //this.Soil = soil;
            //Debug.WriteLine("接口初始化完成");
        }

        public void Close()
        {
            // 关闭线程处理函数
            try
            {
                m_threadDealData.Abort();
                Debug.WriteLine("挂起" + m_threadDealData.Name);
            }
            catch (Exception exp) { Debug.WriteLine(exp); }

            try
            {
                bool isOpen = this.Port.IsOpen;
                int portNum = Int32.Parse(Port.PortName.Replace("COM", ""));
                //  关闭串口
                Port.Close();
            }
            catch (Exception exp) { Debug.WriteLine(exp); }
        }

        public bool Open()
        {
            try
            {
                if (m_threadDealData.ThreadState == System.Threading.ThreadState.Aborted ||
                    m_threadDealData.ThreadState == System.Threading.ThreadState.AbortRequested)
                {
                    m_threadDealData = new Thread(new ThreadStart(DealData))
                    {
                        Name = "Cable处理线程"
                    };
                    m_threadDealData.Start();

                    Debug.WriteLine("恢复" + m_threadDealData.Name);
                }
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex.ToString());
            }
            try
            {
                //InvokeMessage(String.Format("开启串口{0}", Port.PortName), "初始化");
                Port.Open();
                InvokeMessage(String.Format("开启串口{0}成功", Port.PortName), "初始化");
                return true;
            }
            catch (Exception ex)
            {
                InvokeMessage(String.Format("开启串口{0}失败", Port.PortName), "初始化");
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }
        #region 不需要的实现
        public IDown Down
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IFlashBatch FlashBatch
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsCommonWorkNormal
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public ISoil Soil
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IUBatch UBatch
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion


        #region 帮助方法
        private void SendText(string msg)
        {
            if (this.SerialPortStateChanged != null)
                this.SerialPortStateChanged(this, new CEventSingleArgs<CSerialPortState>(new CSerialPortState()
                {
                    BNormal = false,
                    PortNumber = Int32.Parse(Port.PortName.Replace("COM", "")),
                    PortType = this.m_portType
                }));
            try
            {
                this.Port.Write(msg);
            }
            catch (Exception exp)
            { }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //  读取串口内容
            int n = Port.BytesToRead;
            if (n != 0)
            {
                if (this.SerialPortStateChanged != null)
                    this.SerialPortStateChanged(this, new CEventSingleArgs<CSerialPortState>(new CSerialPortState()
                    {
                        BNormal = true,
                        PortNumber = Int32.Parse(Port.PortName.Replace("COM", "")),
                        PortType = this.m_portType
                    }));
            }
            byte[] buf = new byte[n];
            Port.Read(buf, 0, n);
            m_inputBuffer.AddRange(buf);
            m_semephoreData.Release(1);

        }

        private void DealData()
        {
            while (true)
            {
                try
                {
                    m_semephoreData.WaitOne();
                    Thread.Sleep(1000);
                    string data = string.Empty;
                    string rawdata = System.Text.Encoding.ASCII.GetString(m_inputBuffer.ToArray());
                    WriteToFileClass writeClass = new WriteToFileClass("ReceivedLog");
                    Thread t = new Thread(new ParameterizedThreadStart(writeClass.WriteInfoToFile));
                    t.Start("GPRS： " + "长度：" + rawdata.Length + " " + rawdata + "\r\n");
                    //InvokeMessage(rawdata, "原始数据");
                    if (rawdata == null || rawdata == "")
                    {
                        continue;
                    }
                    if (!rawdata.EndsWith(";") && !rawdata.Contains(";") && !rawdata.Contains(";") && !rawdata.Contains(";"))
                    {
                        InvokeMessage("未包含结束符号", "输出");
                        continue;
                    }
                    m_inputBuffer.Clear();
                    string temp = rawdata.Trim();
                    string result = string.Empty;
                    //InvokeMessage(temp, "原始数据");
                    //TODO 判定结束符
                    if (rawdata.Contains("$"))
                    {
                        data = rawdata;
                        string[] dataList = data.Split('$');
                        //上行报文接收需回复TRU
                        
                        //数据解析
                        for (int i = 0; i < dataList.Count(); i++)
                        {
                            string oneGram = dataList[i];
                            CReportStruct report = new CReportStruct();
                            Protocol.Data.ZFXY.UpParse up1 = new Data.ZFXY.UpParse();
                                //UpPaser up = new UpPaser();
                                if (up1.Parse(oneGram, out report))
                                {
                                        report.ChannelType = EChannelType.Cable;
                                        report.ListenPort = "COM" + this.Port.PortName;
                                        if (this.UpDataReceived != null)
                                        {
                                            InvokeMessage(oneGram, "[CABLE]接收");
                                            this.UpDataReceived.Invoke(null, new UpEventArgs() { Value = report, RawData = oneGram });
                                        }
                                    }
                                }
                            }
                            
                        }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp.Message);
                    m_inputBuffer.Clear();
                    //InvokeMessage(rawdata, "接收");
                }
            }//end of while
        }
        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="description"></param>
        private void InvokeMessage(string msg, string description)
        {
            if (this.MessageSendCompleted != null)
                this.MessageSendCompleted(null, new SendOrRecvMsgEventArgs()
                {
                    ChannelType = this.m_channelType,
                    Msg = msg,
                    Description = description
                });
        }
        #endregion
        
        public void InitStations(List<CEntityStation> stations)
        {
            throw new NotImplementedException();
        }

        #region 事件
        public event EventHandler<BatchEventArgs> BatchDataReceived;
        public event EventHandler<COUTEventArgs> COUTCompleted;
        public event EventHandler<DownEventArgs> DownDataReceived;
        public event EventHandler<ReceiveErrorEventArgs> ErrorReceived;
        public event EventHandler<SendOrRecvMsgEventArgs> MessageSendCompleted;
        public event EventHandler<CEventSingleArgs<CSerialPortState>> SerialPortStateChanged;
        public event EventHandler<CEventSingleArgs<CEntitySoilData>> SoilDataReceived;
        public event EventHandler<UpEventArgs> UpDataReceived;
        public event EventHandler<UpEventArgs_new> UpDataReceived_new;
        #endregion
    }
}