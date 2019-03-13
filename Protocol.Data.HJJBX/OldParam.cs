using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Data.HJJBX
{
        public enum EOldParam
        {
            /// <summary>
            /// 01 雨量
            /// </summary>
            Rain,

            /// <summary>
            /// 02 水位 
            /// </summary>
            Water,

            /// <summary>
            /// 03 水位定时报 后加01、02
            /// </summary>
            WaterTimed,

            /// <summary>
            /// 04 12小时电压
            /// </summary>
            HalfVoltage,

            /// <summary>
            /// 05 电池电压
            /// </summary>
            Voltage,

            /// <summary>
            /// 06  24小时电压
            /// </summary>
            AllVoltage,

            /// <summary>
            /// 07  采样间隔
            /// </summary>
            Interval,

            /// <summary>
            /// 08 定时报次数
            /// </summary>
            ETimedNum,

            /// <summary>
            /// 09  平安报次数
            /// </summary>
            ESaveNum,

            /// <summary>
            /// 10 水位基值
            /// </summary>
            WaterBase,

            /// <summary>
            /// 11 人工水位
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
            /// 15 目的手机号码
            /// </summary>
            DestPhoneNum,

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
            /// 30 数据包数
            /// </summary>
            DataPackNum

        }
        public enum ChannelType
        {
            GPRS = 15,
            BeiDou = 3,
            GSM,
            PSTN,
            None,
            VHF,
            //  仅用于记录日志
            BeidouNormal,
            Beidou500
        };

}
