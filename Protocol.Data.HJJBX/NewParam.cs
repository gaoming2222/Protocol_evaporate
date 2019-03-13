using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Data.HJJBX
{
    public enum ENewParam
    {
        /// <summary>
        /// 01 雨量
        /// </summary>
        Rain,

        /// <summary>
        /// 02 水位1 
        /// </summary>
        Water1,

        /// <summary>
        /// 03 水位2 
        /// </summary>
        Water2,

        /// <summary>
        /// 04 时钟
        /// </summary>
        Clock,

        /// <summary>
        /// 05 电池电压
        /// </summary>
        Voltage,

        /// <summary>
        /// 06  水位采样间隔
        /// </summary>
        WaterInterval,

        /// <summary>
        /// 07  雨量采样间隔
        /// </summary>
        RainInterval,

        /// <summary>
        /// 08 定时报次数
        /// </summary>
        ETimedNum,

        /// <summary>
        /// 09  平安报次数
        /// </summary>
        ESaveNum,

        /// <summary>
        /// 10 水位基值1
        /// </summary>
        WaterBase1,

        /// <summary>
        /// 11 水位基值2
        /// </summary>
        WaterBase2,

        /// <summary>
        /// 12 人工水位
        /// </summary>
        MannualWater,

        /// <summary>
        /// 13 水位加报阈值
        /// </summary>
        WaterAddRange,

        /// <summary>
        /// 14 雨量加报阈值
        /// </summary>
        RainAddRange,

        /// <summary>
        /// 15 目的手机号码1
        /// </summary>
        DestPhoneNum1,

        /// <summary>
        /// 16 目的手机号码2
        /// </summary>
        DestPhoneNum2,

        /// <summary>
        /// 17 目的终端机号
        /// </summary>
        TerminalNum,

        /// <summary>
        /// 18 响应波束
        /// </summary>
        RespWave,

        /// <summary>
        /// 19 通波位置
        /// </summary>
        WavePost,

        /// <summary>
        /// 20 工作状态
        /// </summary>
        WorkStatus,

        /// <summary>
        /// 21 主备信道
        /// </summary>
        StandbyChannel,

        /// <summary>
        /// 22 GSM信号场强
        /// </summary>
        GSMSignal,

        /// <summary>
        /// 23 人工流量
        /// </summary>
        MannualFlow,

        /// <summary>
        /// 24 GSM控电模式
        /// </summary>
        GSMElec,

        /// <summary>
        /// 25 水温
        /// </summary>
        WaterTemp,

        /// <summary>
        /// 29 4组水温
        /// </summary>
        FourWaterTemp,

        /// <summary>
        /// 30 数据包数
        /// </summary>
        DataPackNum

    }
    public enum EWorkStatus
    {
        Debug,
        Normal
    }

    public enum EGSMElec
    {
        AllOpen,
        HalfOpen
    }

}
