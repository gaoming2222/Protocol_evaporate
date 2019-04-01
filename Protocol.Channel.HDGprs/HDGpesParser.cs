using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using Hydrology.Entity;
using Protocol.Channel.Interface;
using Protocol.Data.Interface;
using System.IO;
//using Protocol.Data.HJJBX;
//using Protocol.Data.Lib;
//using Protocol.Data.SXDZ;
using Protocol.Data.ZYJBX;
using Protocol.Manager;
//using Protocol.Data.XYJBX;


namespace Protocol.Channel.HDGprs
{
    public class HDGpesParser : IHDGprs
    {
        internal class MyMessage
        {
            public string ID;
            public string MSG;
        }
        #region 成员变量
        static bool s_isFirstSend = true;
        private Semaphore m_semaphoreData;    //用来唤醒消费者处理缓存数据
        private Mutex m_mutexListDatas;     // 内存data缓存的互斥量
        private Thread m_threadDealData;    // 处理数据线程
        private List<HDModemDataStruct> m_listDatas;   //存放data的内存缓存

        private System.Timers.Timer m_timer = new System.Timers.Timer()
        {
            Enabled = true,
            Interval = 5000
        };
        private int GetReceiveTimeOut()
        {
            return (int)(m_timer.Interval);
        }

        public static CDictionary<String, String> HdProtocolMap = new CDictionary<string, string>();
        #endregion

        #region 构造方法
        public HDGpesParser()
        {
            m_semaphoreData = new Semaphore(0, Int32.MaxValue);
            m_listDatas = new List<HDModemDataStruct>();
            m_mutexListDatas = new Mutex();

            m_threadDealData = new Thread(new ThreadStart(this.DealData));
            m_threadDealData.Start();

            DTUList = new List<HDModemInfoStruct>();

            m_timer.Elapsed += new ElapsedEventHandler(m_timer_Elapsed);
        }
        #endregion
        void m_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            int second = GetReceiveTimeOut();
            InvokeMessage(String.Format("系统接收数据时间超过{0}毫秒", second), "系统超时");
            if (this.ErrorReceived != null)
                this.ErrorReceived.Invoke(null, new ReceiveErrorEventArgs()
                {
                    Msg = String.Format("系统接收数据时间超过{0}秒", second)
                });
            if (null != this.GPRSTimeOut)
            {
                this.GPRSTimeOut(null, new ReceivedTimeOutEventArgs() { Second = second });
            }
            Debug.WriteLine("系统超时,停止计时器");
            m_timer.Stop();
        }
        #region 属性
        private List<CEntityStation> m_stationLists;
        public IUp Up { get; set; }
        public IDown Down { get; set; }
        public IUBatch UBatch { get; set; }
        public IFlashBatch FlashBatch { get; set; }
        public ISoil Soil { get; set; }

        public List<HDModemInfoStruct> DTUList { get; set; }

        public bool IsCommonWorkNormal { get; set; }
        private System.Timers.Timer tmrData;
        private System.Timers.Timer tmrDTU;
        private EChannelType m_channelType;
        private EListeningProtType m_portType;
        #endregion

        #region 日志记录
        public void InvokeMessage(string msg, string description)
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



        #region 事件
        public event EventHandler<BatchEventArgs> BatchDataReceived;
        public event EventHandler<BatchSDEventArgs> BatchSDDataReceived;
        public event EventHandler<DownEventArgs> DownDataReceived;
        public event EventHandler<ReceiveErrorEventArgs> ErrorReceived;
        public event EventHandler<SendOrRecvMsgEventArgs> MessageSendCompleted;
        public event EventHandler<ReceivedTimeOutEventArgs> GPRSTimeOut;
        public event EventHandler<CEventSingleArgs<CSerialPortState>> SerialPortStateChanged;
        public event EventHandler<CEventSingleArgs<CEntitySoilData>> SoilDataReceived;
        public event EventHandler<UpEventArgs> UpDataReceived;
        public event EventHandler<UpEventArgs_new> UpDataReceived_new;
        public event EventHandler<ModemDataEventArgs> ModemDataReceived;
        public event EventHandler HDModemInfoDataReceived;
        #endregion


        #region 用户列表维护
        private bool inDtuTicks = false;
        private void tmrDTU_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (inDtuTicks) return;
            inDtuTicks = true;
            try
            {
                Dictionary<string, HDModemInfoStruct> dtuList;
                if (this.getDTUList(out dtuList) == 0)
                {
                    this.DTUList.Clear();
                    foreach (var item in dtuList)
                    {
                        this.DTUList.Add(item.Value);
                    }

                    if (this.HDModemInfoDataReceived != null)
                        this.HDModemInfoDataReceived(this, null);
                }

            }
            catch (Exception eee)
            {
            }
            finally
            {
                inDtuTicks = false;
            }

        }

        #endregion
        public void Init()
        {
            InitMap();
            this.m_channelType = EChannelType.GPRS;
            this.m_portType = EListeningProtType.Port;
            if (tmrData == null)
                tmrData = new System.Timers.Timer(250);
            tmrData.Elapsed += new ElapsedEventHandler(tmrData_Elapsed);

            if (tmrDTU == null)
                tmrDTU = new System.Timers.Timer(2000);
            tmrDTU.Elapsed += new ElapsedEventHandler(tmrDTU_Elapsed);

            if (DTUList == null)
                DTUList = new List<HDModemInfoStruct>();
        }
        public void Close()
        {
            this.DSStopService(null);
        }

        public void InitInterface(IUp up, IDown down, IUBatch udisk, IFlashBatch flash, ISoil soil)
        {
            this.Up = up;
            this.Down = down;
            this.UBatch = udisk;
            this.FlashBatch = flash;
            this.Soil = soil;
        }

        public void InitStations(List<CEntityStation> stations)
        {
            this.m_stationLists = stations;
        }

        public void InitMap()
        {
            String[] rows = File.ReadAllLines("Config/map.txt");
            foreach (String row in rows)
            {
                String[] pieces = row.Split(',');
                if (pieces.Length == 2)
                    if (!HdProtocolMap.ContainsKey(pieces[0]))
                    {
                        HdProtocolMap.Add(pieces[0], pieces[1]);
                    }
                    else
                    {
                        HdProtocolMap[pieces[0]] = pieces[1];
                    }
            }
        }
        private CEntityStation FindStationBySID(string sid)
        {
            if (this.m_stationLists == null)
                throw new Exception("GPRS模块未初始化站点！");

            CEntityStation result = null;
            foreach (var station in this.m_stationLists)
            {
                if (station.StationID.Equals(sid))
                {
                    result = station;
                    break;
                }
            }
            return result;
        }

        public int DSStartService(ushort port, int protocol, int mode, string mess, IntPtr ptr)
        {
            bool flag = false;
            int started = DTUdll.Instance.StartService(port, protocol, mode, mess, ptr);
            if (started == 0)
            {
                tmrData.Start();
                tmrDTU.Start();
                flag = true;
            }
            if (SerialPortStateChanged != null)
                SerialPortStateChanged(this, new CEventSingleArgs<CSerialPortState>(new CSerialPortState()
                {
                    PortType = this.m_portType,
                    PortNumber = port,
                    BNormal = flag
                }));
            //InvokeMessage(String.Format("开启端口{0}   {1}!", port, started ? "成功" : "失败"), "初始化");
            return started;
        }

        public int DSStopService(string mess)
        {
            bool stoped = false;
            int ended = 0;
            ended = DTUdll.Instance.StopService(mess);
            if (ended == 0)
            {
                stoped = true;
            }
            tmrData.Stop();
            tmrDTU.Stop();
            int port = DTUdll.Instance.ListenPort;
            if (SerialPortStateChanged != null)
                SerialPortStateChanged(this, new CEventSingleArgs<CSerialPortState>(new CSerialPortState()
                {
                    PortType = this.m_portType,
                    PortNumber = port,
                    BNormal = stoped
                }));
            InvokeMessage(String.Format("关闭端口{0}   {1}!", port, stoped ? "成功" : "失败"), "      ");
            return ended;
        }

        public int sendHex(string userid, byte[] data, uint len, string mess)
        {
            int flag = 0;
            try
            {
                flag = DTUdll.Instance.SendHex(userid, data, len, null);
                return flag;

            }
            catch (Exception e)
            {
                return flag;
            }

        }

        public uint getDTUAmount()
        {
            return DTUdll.Instance.getDTUAmount();
        }
        public int getDTUInfo(string userid, out HDModemInfoStruct infoPtr)
        {
            infoPtr = new HDModemInfoStruct();
            return DTUdll.Instance.getDTUInfo(userid, out infoPtr);
        }
        public int getDTUByPosition(int index, out HDModemInfoStruct infoPtr)
        {
            infoPtr = new HDModemInfoStruct();
            return DTUdll.Instance.getDTUByPosition(index, out infoPtr);
        }
        public int getDTUList(out Dictionary<string, HDModemInfoStruct> dtuList)
        {
            return DTUdll.Instance.GetDTUList(out dtuList);
        }
        //帮助方法 20170602
        private int GetNextData(out HDModemDataStruct dat)
        {
            try
            {
                return DTUdll.Instance.GetNextData(out dat);
            }
            catch (Exception e)
            {
                dat = new HDModemDataStruct();
                return -1;
            }
        }
        private bool inDataTicks = false;
        private void tmrData_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (inDataTicks || inDtuTicks) return;
            inDataTicks = true;
            try
            {
                //读取数据
                HDModemDataStruct dat = new HDModemDataStruct();
                while (this.GetNextData(out dat) == 0)

                {

                    //byte[] bts = new byte[] { 84, 82, 85, 13, 10 };
                    String str = System.Text.Encoding.Default.GetString(dat.m_data_buf);
                    String strid = System.Text.Encoding.Default.GetString(dat.m_modemId);
                    String strTime = System.Text.Encoding.Default.GetString(dat.m_recv_time);
                    
                    m_mutexListDatas.WaitOne();
                    //Debug.WriteLine("协议接收数据: " + System.Text.Encoding.Default.GetString(dat.m_data_buf));
                    if ((strid.Substring(0, 1) != "/0")&&(strid.Substring(0, 1) != "\0"))
                    {
                        m_listDatas.Add(dat);
                    }
                    m_semaphoreData.Release(1);
                    m_mutexListDatas.ReleaseMutex();
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine("读取数据", ee.Message);
            }
            finally
            {
                inDataTicks = false;
            }
        }

        private void DealData()
        {
            while (true)
            {
                m_semaphoreData.WaitOne(); //阻塞当前线程，知道被其它线程唤醒
                // 获取对data内存缓存的访问权
                m_mutexListDatas.WaitOne();
                List<HDModemDataStruct> dataListTmp = m_listDatas;
                m_listDatas = new List<HDModemDataStruct>(); //开辟一快新的缓存区
                m_mutexListDatas.ReleaseMutex();
                for (int i = 0; i < dataListTmp.Count; ++i)
                {
                    try
                    {
                        HDModemDataStruct dat = dataListTmp[i];
                        string data = System.Text.Encoding.Default.GetString(dat.m_data_buf);
                        string temp = data.Trim();
                        WriteToFileClass writeClass = new WriteToFileClass("ReceivedLog");
                        Thread t = new Thread(new ParameterizedThreadStart(writeClass.WriteInfoToFile));
                        t.Start("GPRS： " + "长度：" + data.Length + " " + data + "\r\n");
                        if (temp.Contains("$"))
                        {

                            string[] dataList = temp.Split('$');
                            //数据解析
                            for (int j = 0; i < dataList.Length; j++)
                            {
                                if(dataList[j].Length < 20)
                                {
                                    continue;
                                }
                                string dataGram = dataList[j].Substring(0, dataList[j].IndexOf(';'));
                                CReportStruct report = new CReportStruct();
                                Protocol.Data.ZFXY.UpParse up = new Data.ZFXY.UpParse();
                                if (up.Parse(dataGram, out report)){
                                    report.ChannelType = EChannelType.GPRS;
                                    report.ListenPort = this.GetListenPort().ToString();
                                    if (this.UpDataReceived != null)
                                    {
                                        InvokeMessage(dataGram, "[GPRS]接收");
                                        this.UpDataReceived.Invoke(null, new UpEventArgs() { Value = report, RawData = temp });
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("" + e.Message);
                    }
                }
            }
        }
        #region 接口函数
        public ushort GetListenPort()
        {
            return DTUdll.Instance.ListenPort;
        }

        public bool FindByID(string userID, out byte[] dtuID)
        {
            dtuID = null;
            List<HDModemInfoStruct> DTUList_1 = DTUList;
            //foreach (var item in DTUList_1)
            for (int i = 0; i < DTUList_1.Count; i++)
            {
                HDModemInfoStruct item = DTUList_1[i];
                if (System.Text.Encoding.Default.GetString(item.m_modemId).Substring(0, 11) == userID)
                {
                    dtuID = item.m_modemId;
                    return true;
                }
            }
            return false;
        }

        public void SendDataTwice(string id, string msg)
        {
            m_timer.Interval = 600;
            SendData(id, msg);
            if (s_isFirstSend)
            {
                MyMessage myMsg = new MyMessage() { ID = id, MSG = msg };
                s_isFirstSend = false;
                Thread t = new Thread(new ParameterizedThreadStart(ResendRead))
                {
                    Name = "重新发送读取线程",
                    IsBackground = true
                };
                t.Start(myMsg);
            }
        }

        public void SendDataTwiceForBatchTrans(string id, string msg)
        {
            m_timer.Interval = 60000;
            SendData(id, msg);
            if (s_isFirstSend)
            {
                MyMessage myMsg = new MyMessage() { ID = id, MSG = msg };
                s_isFirstSend = false;
                Thread t = new Thread(new ParameterizedThreadStart(ResendRead))
                {
                    Name = "重新发送读取线程",
                    IsBackground = true
                };
                t.Start(myMsg);
            }
        }

        #endregion

        #region 帮助函数
        public bool SendData(string id, string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return false;
            }
            //      Debug.WriteLine("GPRS发送数据:" + msg);
            InvokeMessage(msg, "发送");
            //      Debug.WriteLine("先停止计时器，然后在启动计时器");
            //  先停止计时器，然后在启动计时器
            m_timer.Stop();
            m_timer.Start();
            byte[] bmesg = System.Text.Encoding.Default.GetBytes(msg);
            if (DTUdll.Instance.SendHex(id, bmesg, (uint)bmesg.Length, null) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ResendRead(object obj)
        {
            Debug.WriteLine(System.Threading.Thread.CurrentThread.Name + "休息1秒!");
            System.Threading.Thread.Sleep(1000);
            try
            {
                MyMessage myMsg = obj as MyMessage;
                if (null != myMsg)
                {
                    SendData(myMsg.ID, myMsg.MSG);
                }
            }
            catch (Exception exp) { Debug.WriteLine(exp.Message); }
            finally { s_isFirstSend = true; }
        }


        #endregion
    }
}
