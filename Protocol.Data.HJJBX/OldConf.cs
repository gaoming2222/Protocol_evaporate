using Hydrology.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Data.HJJBX
{
    public class OldConf
    {
        /// <summary>
        /// 站点ID
        /// </summary>
        public string Stationid { get; set; }
        /// <summary>
        /// 通信类别
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 报文类别
        /// </summary>
        public EMessageType ReportType { get; set; }
        /// <summary>
        /// 站点类别
        /// </summary>
        public EStationType StationType { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime RecvTime { get; set; }
        /// <summary>
        /// 包序号
        /// </summary>
        public string PackageNum;
            /// <summary>
            /// 01 雨量
            /// </summary>
            public Nullable<Decimal> Rain;

            /// <summary>
            /// 02 水位 
            /// </summary>
            public Nullable<Decimal> Water;

            /// <summary>
            /// 03 水位定时报 后加01、02
            /// </summary>
            public Nullable<Int32> WaterInterval;
            public IList<Decimal> WaterTimed;

            /// <summary>
            /// 04 12小时电压
            /// </summary>
            public IList<Decimal> HalfVoltage;

            /// <summary>
            /// 05 电池电压
            /// </summary>
            public Nullable<Decimal> Voltage;

            /// <summary>
            /// 06  24小时电压
            /// </summary>
            public IList<Decimal> AllVoltage;

            /// <summary>
            /// 07  采样间隔
            /// </summary>
            public Nullable<Int32> Interval;

            /// <summary>
            /// 08 定时报次数
            /// </summary>
            public Nullable<Int32> ETimedNum;

            /// <summary>
            /// 09  平安报次数
            /// </summary>
            public Nullable<Int32> ESaveNum;

            /// <summary>
            /// 10 水位基值
            /// </summary>
            public Nullable<Decimal> WaterBase;

            /// <summary>
            /// 11 人工水位
            /// </summary>
            public Nullable<Decimal> MannualWater;
            public DateTime MannualWaterTime;

            /// <summary>
            /// 13 水位加报阈值
            /// </summary>
            public Nullable<Decimal> WaterAddRange;

            /// <summary>
            /// 14 雨量加报阈值
            /// </summary>
            public Nullable<Decimal> RainAddRange;

            /// <summary>
            /// 15 目的手机号码
            /// </summary>
            public string DestPhoneNum;

            /// <summary>
            /// 17 目的终端机号
            /// </summary>
            public string TerminalNum;

            /// <summary>
            /// 18 响应波束
            /// </summary>
            public string RespWave;

            /// <summary>
            /// 19 通波位置
            /// </summary>
            public Nullable<Int32> WavePost;

            /// <summary>
            /// 20 工作状态
            /// </summary>
            public Nullable<EWorkStatus> WorkStatus;

            /// <summary>
            /// 21 主备信道
            /// </summary>
            public Nullable<ChannelType> MainChannel;
            public Nullable<ChannelType> ViceChannel;

            /// <summary>
            /// 22 GSM信号场强
            /// </summary>
            public Nullable<Decimal> GSMSignal;

            /// <summary>
            /// 23 人工流量
            /// </summary>
            public Nullable<Decimal> MannualFlow;
            public DateTime MannualFlowTime;

            /// <summary>
            /// 24 GSM控电模式
            /// </summary>
            public Nullable<EGSMElec> GSMElec;

            /// <summary>
            /// 25 水温
            /// </summary>
            public Nullable<Decimal> WaterTemp;

            /// <summary>
            /// 30 数据包数
            /// </summary>
            public Nullable<Int32> DataPackNum;
    }
}
