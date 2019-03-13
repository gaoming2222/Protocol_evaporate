using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Hydrology.Entity;
using System.Threading;
using System.Diagnostics;


namespace Protocol.Channel.HDGprs
{
    internal class DTUdll
    {
        //C:\Users\codergaoming\Desktop\Hydro0620\Hydro\bin\Debug
        readonly string dllpath = System.AppDomain.CurrentDomain.BaseDirectory + "HDgprs.dll";
        #region 必用函数
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern int start_net_service(IntPtr intPtr, uint wMsg, int nServerPort, string mess);
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern int SetWorkMode(int nWorkMode);
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern int SelectProtocol(int nProtocol);
        [DllImport(@"HDgprs.dll")]
        private static extern void SetCustomIP(string ip);
        [DllImport(@"HDgprs.dll")]
        private static extern string GetCurrentIP();
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern int do_read_proc(ref HDModemDataStruct pDataStruct, string mess, bool isReply);
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern int stop_net_service(string mess);
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern int do_send_user_data(string userid, byte [] data, uint len,string mess);
        #endregion

        #region 关闭与下线
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern int do_close_one_user(int userid,string mess);
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern int do_close_one_user2(int userid, string mess);
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern int do_close_all_user(string mess);
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern int do_close_all_user2(string mess);
        #endregion

        #region 取消
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern void cancel_read_block();
        #endregion

        #region userinfo
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern uint  get_max_user_amount();
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern uint get_online_user_amount();
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern int get_user_info(string userid,ref HDModemInfoStruct infoPtr);
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern int get_user_at(int index, ref HDModemInfoStruct infoPtr);
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern int add_one_user(HDModemInfoStruct user);
        [DllImport(@"D:\lib\HDgprs.dll")]
        private static extern int delete_one_user(int userid,string mess);
        #endregion

        #region 参数配置函数
        #endregion

        #region 其他
        #endregion




        private DTUdll()
        {
            
        }

        private static DTUdll _instance;
        public static DTUdll Instance
        {
            get
            {
                if (_instance == null) _instance = new DTUdll();
                return _instance;
            }
        }


        private bool _started = false;
        public bool Started
        {
            private set
            {
                _started = value;
            }
            get
            {
                return _started;
            }
        }

        private ushort _listenPort = 0;
        public ushort ListenPort
        {
            private set
            {
                _listenPort = value;
            }
            get
            {
                return _listenPort;
            }
        }
        public int StartService(ushort port,int protocol,int mode,string mess,IntPtr ptr)
        {
            //string mess = null;
            IntPtr p = IntPtr.Zero;
            //默认采用非阻塞模式
            int a = SetWorkMode(mode);
            //默认采用udp协议
            int b = SelectProtocol(protocol);
            ListenPort = port;
            int flag = -1;
            try
            {
                //bool flag1 = DSStartService(port);
                flag = start_net_service(p, 0,port, mess);
                return flag;
            }
            catch (Exception ee)
            {
                return flag;
            }

        }

        public int GetNextData(out HDModemDataStruct dat)
        {
            string mess = null;
            dat = new HDModemDataStruct();
            int flag = -1;
            try
            {
                flag = do_read_proc(ref dat, mess, true);
                return flag;
            }
            catch (Exception ee2)
            {
                return flag;
            }

        }
        public int StopService(string mess)
        {
            int flag = -1;
            try
            {
                flag = stop_net_service(null);
                return flag;
            }
            catch (Exception ee)
            {
                return flag;
            }
        }
        //待检测的函数
        public int SendHex(string userid,byte [] data,uint leng,string mess)
        {
            int flag = -1;
            try
            {
                flag = do_send_user_data(userid, data,leng,mess);
                return flag;
            }
            catch(Exception e)
            {
                Debug.WriteLine("platform mess to instrument error");
                return flag;
            }
        }

        public uint getDTUAmount()
        {
            uint num = 0;
            try
            {
                num = get_online_user_amount();
                return num;
            }
            catch (Exception e)
            {
                Debug.WriteLine("获取在在线DTU数失败");
                return num;
            }
        }

        public int getDTUInfo(string  userid,out HDModemInfoStruct infoPtr)
        {
            int flag = -1;
            infoPtr = new HDModemInfoStruct();
            try
            {
                flag = get_user_info(userid, ref infoPtr);
                return flag;

            }catch(Exception e)
            {
                return flag;
            }
        }

        public int getDTUByPosition(int index, out HDModemInfoStruct infoPtr)
        {
            infoPtr = new HDModemInfoStruct();
            int flag = -1;
            try
            {
                flag = get_user_at(index, ref infoPtr);
                return flag;
            }catch(Exception e)
            {
                return flag;
            }
        }
        public int GetDTUList(out Dictionary<string, HDModemInfoStruct> dtuList)
        {
            int flag = -1;
            dtuList = new Dictionary<string, HDModemInfoStruct>();
            try
            {
                uint count = getDTUAmount();
                for (int pos = 0; pos < count; pos++)
                {
                    HDModemInfoStruct dtu = new HDModemInfoStruct();
                    flag = get_user_at(pos, ref dtu);
                    //string data = System.Text.Encoding.Default.GetString(dtu.m_sin_addr);
                    string userid = System.Text.Encoding.Default.GetString(dtu.m_modemId);
                    dtuList.Add(userid,dtu);
                   
                }
                return 0;
            }
            catch(Exception e)
            {
                return flag;
            }

        }


    }
}
