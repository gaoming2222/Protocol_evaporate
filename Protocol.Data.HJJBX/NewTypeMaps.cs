using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Data.HJJBX
{
    public class NewTypeMaps
    {
        public static CDictionary<ENewParam, String> NewParamMap = new CDictionary<ENewParam, String>()
        {
            { ENewParam.Rain ,          "01" },
            { ENewParam.Water1 ,        "02" },
            { ENewParam.Water2,         "03" },
            { ENewParam.Clock ,         "04" },
            { ENewParam.Voltage ,       "05" },
            { ENewParam.WaterInterval , "06" },
            { ENewParam.RainInterval ,  "07" },
            { ENewParam.ETimedNum ,     "08" },
            { ENewParam.ESaveNum ,      "09" },
            { ENewParam.WaterBase1,     "10" },
            { ENewParam.WaterBase2 ,    "11" },
            { ENewParam.MannualWater ,  "12" },
            { ENewParam.WaterAddRange , "13" },
            { ENewParam.RainAddRange ,  "14" },
            { ENewParam.DestPhoneNum1 , "15" },
            { ENewParam.DestPhoneNum2 , "16" },
            { ENewParam.TerminalNum ,   "17" },
            { ENewParam.RespWave ,      "18" },
            { ENewParam.WavePost ,      "19" },
            { ENewParam.WorkStatus ,    "20" },
            { ENewParam.StandbyChannel ,"21" },
            { ENewParam.GSMSignal ,     "22" },
            { ENewParam.MannualFlow ,   "23" },
            { ENewParam.GSMElec ,       "24" },
            { ENewParam.WaterTemp ,     "25" },
            { ENewParam.FourWaterTemp , "29" },
            { ENewParam.DataPackNum ,   "30" }
        };

        /// <summary>
        /// -1 表示长度未定
        /// 正数表示相应字段对应字符串解析中的长度
        /// </summary>
        public static CDictionary<ENewParam, String> NewParamLengthMap = new CDictionary<ENewParam, String>()
        {
            { ENewParam.Rain ,          "04" },
            { ENewParam.Water1 ,        "06" },
            { ENewParam.Water2,         "06" },
            { ENewParam.Clock ,         "10" },
            { ENewParam.Voltage ,       "04" },
            { ENewParam.WaterInterval , "02" },
            { ENewParam.RainInterval ,  "02" },
            { ENewParam.ETimedNum ,     "02" },
            { ENewParam.ESaveNum ,      "01" },
            { ENewParam.WaterBase1,     "06" },
            { ENewParam.WaterBase2,     "06" },
            { ENewParam.MannualWater ,  "16" },
            { ENewParam.WaterAddRange , "02" },
            { ENewParam.RainAddRange ,  "02" },
            { ENewParam.DestPhoneNum1 , "12" },
            { ENewParam.DestPhoneNum2 , "12" },
            { ENewParam.TerminalNum ,   "08" },
            { ENewParam.RespWave ,      "02" },
            { ENewParam.WavePost ,      "03" },
            { ENewParam.WorkStatus ,    "01" },
            { ENewParam.StandbyChannel ,"04" },
            { ENewParam.GSMSignal ,     "02" },
            { ENewParam.MannualFlow ,   "16" },
            { ENewParam.GSMElec ,       "01" },
            { ENewParam.WaterTemp ,     "04" },
            { ENewParam.FourWaterTemp , "16" },
            { ENewParam.DataPackNum ,   "02" }
        };

        public static CDictionary<ENewParam, String> NewParam4ChineseMap = new CDictionary<ENewParam, string>()
        {
            { ENewParam.Rain ,                  "雨量" },
            { ENewParam.Water1 ,               "水位1" },
            { ENewParam.Water2,                "水位2" },
            { ENewParam.Clock ,                 "时钟" },
            { ENewParam.Voltage ,           "电池电压" },
            { ENewParam.WaterInterval , "水位采样间隔" },
            { ENewParam.RainInterval ,  "雨量采样间隔" },
            { ENewParam.ETimedNum ,       "定时报次数" },
            { ENewParam.ESaveNum ,        "平安报次数" },
            { ENewParam.WaterBase1,        "水位基值1" },
            { ENewParam.WaterBase2,        "水位基值2" },
            { ENewParam.MannualWater ,      "人工水位" },
            { ENewParam.WaterAddRange , "水位加报阈值" },
            { ENewParam.RainAddRange ,  "雨量加报阈值" },
            { ENewParam.DestPhoneNum1 ,"目的手机号码1" },
            { ENewParam.DestPhoneNum2 ,"目的手机号码2" },
            { ENewParam.TerminalNum ,   "目的终端机号" },
            { ENewParam.RespWave ,          "响应波束" },
            { ENewParam.WavePost ,          "通波位置" },
            { ENewParam.WorkStatus ,        "工作状态" },
            { ENewParam.StandbyChannel ,    "主备信道" },
            { ENewParam.GSMSignal ,      "GSM信号场强" },
            { ENewParam.MannualFlow ,       "人工流量" },
            { ENewParam.GSMElec ,        "GSM控电模式" },
            { ENewParam.WaterTemp ,             "水温" },
            { ENewParam.FourWaterTemp ,     "四组水温" },
            { ENewParam.DataPackNum ,       "数据包数" }
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
