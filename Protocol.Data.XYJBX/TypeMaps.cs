using Hydrology.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Data.XYJBX
{
    public class TypeMaps
    {
        public static CDictionary<EParam, string> TypeCodeMap = new CDictionary<EParam, string>()
        {
            {EParam.Rain,"01"},
            {EParam.Water,"02"},
            {EParam.WaterTimed,"03"},
            {EParam.HalfVoltage,"04"},
            {EParam.Voltage,"05"},
            {EParam.AllVoltage,"06"},
            {EParam.Interval,"07"},
            {EParam.ETimedNum,"08"},
            {EParam.ESafeNum,"09"},
            {EParam.WaterBase,"10"},
            {EParam.Water2,"11"},
            {EParam.ManualWater,"12"},
            {EParam.WaterAddRange,"13"},
            {EParam.RainAddRange,"14"},
            {EParam.DestPhoneNum,"15"},
            {EParam.Water2Base,"16"},
            {EParam.TerminalNum,"17"},
            {EParam.RespWave,"18"},
            {EParam.Water2Timed,"19"},
            {EParam.WorkStatus,"20"},
            {EParam.StandbyChannel,"21"},
            {EParam.GSMSignal,"22"},
            {EParam.ManualFlow,"23"},
            {EParam.GSMElec,"24"},
            {EParam.RainTimed,"25"},
            {EParam.DataPackNum,"30"},
        };
        public static CDictionary<EParam, int> TypeLengthMap = new CDictionary<EParam, int>()
        {
            {EParam.Rain, 4},
            {EParam.Water,4},
            {EParam.WaterTimed,48},
            {EParam.HalfVoltage,48},
            {EParam.Voltage,4},
            {EParam.AllVoltage,96},
            {EParam.Interval,2},
            {EParam.ETimedNum,2},
            {EParam.ESafeNum,1},
            {EParam.WaterBase,6},
            {EParam.Water2,4},
            {EParam.ManualWater,16},
            {EParam.WaterAddRange,2},
            {EParam.RainAddRange,2},
            {EParam.DestPhoneNum,12},
            {EParam.Water2Base,6},
            {EParam.TerminalNum,8},
            {EParam.RespWave,2},
            {EParam.Water2Timed,48},
            {EParam.WorkStatus,1},
            {EParam.StandbyChannel,4},
            {EParam.GSMSignal,2},
            {EParam.ManualFlow,16},
            {EParam.GSMElec,1},
            {EParam.RainTimed,48},
            {EParam.DataPackNum,2},
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
    }
}
