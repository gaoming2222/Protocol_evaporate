using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Data.HJJBX
{
    public class OldTypeMaps
    {
        public static CDictionary<EWorkStatus, String> WorkStatus4ProtoMap = new CDictionary<EWorkStatus, String>()
        {
                        //  需要修改
            { EWorkStatus.Debug,   "01" },
            { EWorkStatus.Normal,  "02" },
        };
        public static CDictionary<EWorkStatus, String> WorkStatus4UI = new CDictionary<EWorkStatus, String>()
        {
                        //  需要修改
            { EWorkStatus.Debug,   "调试状态" },
            { EWorkStatus.Normal,  "正常工作状态" },
        };
        public static CDictionary<EGSMElec, String> GSMElec4ProtoMap = new CDictionary<EGSMElec, String>()
        {
                        //  需要修改
            { EGSMElec.AllOpen,   "1" },
            { EGSMElec.HalfOpen,  "2" },
        };
        public static CDictionary<EGSMElec, String> GSMElec4UI = new CDictionary<EGSMElec, String>()
        {
                        //  需要修改
            { EGSMElec.AllOpen,   "全开" },
            { EGSMElec.HalfOpen,  "半开" },
        };
        public static CDictionary<EOldParam, String> OldParamMap = new CDictionary<EOldParam, String>()
        {
            { EOldParam.Rain ,          "01" },
            { EOldParam.Water ,         "02" },
            { EOldParam.WaterTimed,     "03" },
            { EOldParam.HalfVoltage ,   "04" },
            { EOldParam.Voltage ,       "05" },
            { EOldParam.AllVoltage ,    "06" },
            { EOldParam.Interval ,      "07" },
            { EOldParam.ETimedNum ,     "08" },
            { EOldParam.ESaveNum ,      "09" },
            { EOldParam.WaterBase,      "10" },
            { EOldParam.MannualWater ,  "11" },
            { EOldParam.WaterAddRange , "13" },
            { EOldParam.RainAddRange ,  "14" },
            { EOldParam.DestPhoneNum ,  "15" },
            { EOldParam.TerminalNum ,   "17" },
            { EOldParam.RespWave ,      "18" },
            { EOldParam.WavePost ,      "19" },
            { EOldParam.WorkStatus ,    "20" },
            { EOldParam.StandbyChannel ,"21" },
            { EOldParam.GSMSignal ,     "22" },
            { EOldParam.MannualFlow ,   "23" },
            { EOldParam.GSMElec ,       "24" },
            { EOldParam.WaterTemp ,     "25" },
            { EOldParam.DataPackNum ,   "30" }
        };

        /// <summary>
        /// -1 表示长度未定
        /// 正数表示相应字段对应字符串解析中的长度
        /// </summary>
        public static CDictionary<EOldParam, String> OldParamLengthMap = new CDictionary<EOldParam, String>()
        {
            { EOldParam.Rain ,          "04" },
            { EOldParam.Water ,         "04" },
            { EOldParam.WaterTimed,     "48" },
            { EOldParam.HalfVoltage ,   "48" },
            { EOldParam.Voltage ,       "04" },
            { EOldParam.AllVoltage ,    "96" },
            { EOldParam.Interval ,      "02" },
            { EOldParam.ETimedNum ,     "02" },
            { EOldParam.ESaveNum ,      "01" },
            { EOldParam.WaterBase,      "06" },
            { EOldParam.MannualWater ,  "16" },
            { EOldParam.WaterAddRange , "02" },
            { EOldParam.RainAddRange ,  "02" },
            { EOldParam.DestPhoneNum ,  "12" },
            { EOldParam.TerminalNum ,   "08" },
            { EOldParam.RespWave ,      "02" },
            { EOldParam.WavePost ,      "03" },
            { EOldParam.WorkStatus ,    "01" },
            { EOldParam.StandbyChannel ,"04" },
            { EOldParam.GSMSignal ,     "02" },
            { EOldParam.MannualFlow ,   "16" },
            { EOldParam.GSMElec ,       "01" },
            { EOldParam.WaterTemp ,     "04" },
            { EOldParam.DataPackNum ,   "02" }
        };

        public static CDictionary<EOldParam, String> OldParam4ChineseMap = new CDictionary<EOldParam, string>()
        {
            { EOldParam.Rain ,                  "雨量" },
            { EOldParam.Water ,                 "水位" },
            { EOldParam.WaterTimed,       "水位定时报" },
            { EOldParam.HalfVoltage ,     "12小时电压" },
            { EOldParam.Voltage ,           "电池电压" },
            { EOldParam.AllVoltage ,      "24小时电压" },
            { EOldParam.Interval ,          "采样间隔" },
            { EOldParam.ETimedNum ,       "定时报次数" },
            { EOldParam.ESaveNum ,        "平安报次数" },
            { EOldParam.WaterBase,          "水位基值" },
            { EOldParam.MannualWater ,      "人工水位" },
            { EOldParam.WaterAddRange , "水位加报阈值" },
            { EOldParam.RainAddRange ,  "雨量加报阈值" },
            { EOldParam.DestPhoneNum ,  "目的手机号码" },
            { EOldParam.TerminalNum ,   "目的终端机号" },
            { EOldParam.RespWave ,          "响应波束" },
            { EOldParam.WavePost ,          "通波位置" },
            { EOldParam.WorkStatus ,        "工作状态" },
            { EOldParam.StandbyChannel ,    "主备信道" },
            { EOldParam.GSMSignal ,      "GSM信号场强" },
            { EOldParam.MannualFlow ,       "人工流量" },
            { EOldParam.GSMElec ,        "GSM控电模式" },
            { EOldParam.WaterTemp ,             "水温" },
            { EOldParam.DataPackNum ,       "数据包数" }
        };
        public static CDictionary<ChannelType, String> ChannelType4ProtoMap = new CDictionary<ChannelType, String>()
        {
            { ChannelType.BeiDou,  "04" },
            { ChannelType.GPRS,    "06" },
            { ChannelType.GSM,     "05" },
            { ChannelType.None,    "00" },
            { ChannelType.PSTN,    "02" },
            { ChannelType.VHF,     "01" }
        };
        public static CDictionary<ChannelType, String> ChannelType4UIMap = new CDictionary<ChannelType, String>()
        {
            { ChannelType.BeiDou,  "北斗卫星" },
            { ChannelType.GPRS,    "GPRS" },
            { ChannelType.GSM,     "GSM" },
            { ChannelType.None,    "无" },
            { ChannelType.PSTN,    "PSTN" },
            { ChannelType.VHF,     "VHF" }
        };

        public class CDictionary<TKey, TValue> : Dictionary<TKey, TValue>
        {
            public TValue FindValue(TKey key)
            {
                if (this.ContainsKey(key))
                {
                    return this[key];
                }
                throw new Exception(String.Format("{0}不在映射字典中！", key));
            }
            public TKey FindKey(TValue value)
            {
                foreach (var item in this)
                {
                    if (item.Value.Equals(value))
                        return item.Key;
                }
                throw new Exception(String.Format("{0}不在映射字典中！", value));
            }
        }
    }
}
