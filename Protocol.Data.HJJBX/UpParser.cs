using Hydrology.Entity;
using Protocol.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Data.HJJBX
{
    public class UpParser : IUp
    {

        public bool Parse_Old(string msg, out List<CUpReport> upReport)
        {
            upReport = new List<CUpReport>();
            var msgSegs = msg.Split(CSpecialChars.BALNK_CHAR);
            foreach (var msgItem in msgSegs)
            {
                try
                {
                    CUpReport report = null;
                    string data = string.Empty;
                    if (!ProtocolHelpers.DeleteSpecialChar(msgItem, out data))
                    {
                        return false;
                    }

                    //  解析站点ID
                    String StationID = data.Substring(0, 4);
                    //  解析通信类别
                    String type = data.Substring(4, 2);
                    //  解析报文类别
                    EMessageType reportType = ProtocolMaps.MessageTypeMap.FindKey(data.Substring(6, 2));
                    //  解析站点类别
                    EStationType stationType = ProtocolHelpers.ProtoStr2StationType(data.Substring(8, 2));
                    // 解析接收时间
                    DateTime recvTime = new DateTime(
                                    year: Int32.Parse("20" + data.Substring(10, 2)),  //年
                                    month: Int32.Parse(data.Substring(12, 2)),        //月
                                    day: Int32.Parse(data.Substring(14, 2)),          //日
                                    hour: Int32.Parse(data.Substring(16, 2)),         //时
                                    minute: Int32.Parse(data.Substring(18, 2)),       //分
                                    second: 0
                                    );
                    //  解析包序号
                    string packageNum = data.Substring(20, 2);
                    //  解析数据
                    string item = data.Substring(22);
                    CUpData upData = new CUpData();

                    while (item.Length > 2)
                    {
                        try
                        {
                            //  数据分为两部分
                            //  2 Byte 指令  +  剩下的为数据，数据的长度>= 1
                            //  解析指令类型param
                            EOldParam param = OldTypeMaps.OldParamMap.FindKey(item.Substring(0, 2));
                            //  如果接收到的数据段长度大于2，表示对应的字段有值
                            //  默认为String.Empty
                            string info = string.Empty;
                            int length = Int32.Parse(OldTypeMaps.OldParamLengthMap.FindValue(param));
                            info = item.Substring(2);
                            if (String.IsNullOrEmpty(info) && ProtocolHelpers.isLegalCharacters(info))
                            {
                                continue;
                            }

                            switch (param)
                            {
                                case EOldParam.Rain:
                                    upData.Rain = Decimal.Parse(info.Substring(0, length)); break;

                                case EOldParam.Water:
                                    upData.Water1 = Decimal.Parse(info.Substring(0, length)) * (Decimal)0.01; break;

                                case EOldParam.WaterTimed:
                                    //  将水位定时报中的数据储存到list中
                                    var waterList = new Dictionary<DateTime, Decimal>();
                                    int roundNum = Int32.Parse(info.Substring(0, 2));
                                    var watersegs = info;
                                    if (roundNum == 01 || roundNum == 02)
                                    {
                                        roundNum = roundNum == 01 ? 12 : 24;
                                        length = roundNum == 24 ? 96 : 48;
                                        watersegs = info.Substring(2);
                                    }
                                    else
                                    {
                                        roundNum = 12;
                                    }
                                    recvTime = recvTime.AddMinutes(-5 * roundNum);

                                    for (int i = 1; i <= roundNum; i++)
                                    {
                                        DateTime WaterTime = recvTime.AddMinutes(5);
                                        Decimal TimedWater = Decimal.Parse(watersegs.Substring(0, 4)) * (Decimal)0.01;
                                        waterList.Add(WaterTime, TimedWater);
                                        watersegs = watersegs.Substring(4);
                                    }
                                    upData.WaterList = waterList;
                                    break;

                                case EOldParam.HalfVoltage:
                                    //  将电压值存入list中
                                    var halfVoltageList = new Dictionary<DateTime, Decimal>();
                                    DateTime HalfVoltageTime = recvTime;
                                    for (int i = 1; i <= 12; i++)
                                    {
                                        Decimal Voltage = Decimal.Parse(info.Substring(0, 4)) * (Decimal)0.01;
                                        HalfVoltageTime = recvTime.AddHours(-1 * (i - 1));
                                        halfVoltageList.Add(HalfVoltageTime, Voltage);
                                        info = info.Substring(4);
                                    }
                                    upData.VoltageList = halfVoltageList;
                                    break;

                                case EOldParam.Voltage:
                                    upData.Voltage = (Decimal.Parse(info.Substring(0, length)) * (Decimal)0.01); break;
                                case EOldParam.AllVoltage:
                                    //  将电压值存入list中
                                    var allVoltageList = new Dictionary<DateTime, Decimal>();
                                    DateTime AllVoltageTime = recvTime;
                                    for (int i = 1; i <= 24; i++)
                                    {
                                        Decimal Voltage = Decimal.Parse(info.Substring(0, 4)) * (Decimal)0.01;
                                        AllVoltageTime = recvTime.AddHours(-1 * (i - 1));
                                        allVoltageList.Add(AllVoltageTime, Voltage);
                                        info = info.Substring(4);
                                    }
                                    upData.VoltageList = allVoltageList;
                                    break;

                                case EOldParam.Interval:
                                    int Interval = Int32.Parse(info.Substring(0, length)); break;
                                case EOldParam.ESaveNum:
                                    int ESaveNum = Int32.Parse(info.Substring(0, length)); break;
                                case EOldParam.ETimedNum:
                                    int ETimedNum = Int32.Parse(info.Substring(0, length)); break;
                                case EOldParam.WaterBase:
                                    decimal WaterBase = Decimal.Parse(info.Substring(0, length)) * (Decimal)0.01; break;
                                case EOldParam.MannualWater:
                                    DateTime MannualWaterTime = new DateTime(
                                        year: Int32.Parse(info.Substring(0, 2)),
                                        month: Int32.Parse(info.Substring(2, 2)),
                                        day: Int32.Parse(info.Substring(4, 2)),
                                        hour: Int32.Parse(data.Substring(6, 2)),
                                        minute: Int32.Parse(data.Substring(8, 2)),
                                        second: 0
                                        );
                                    recvTime = MannualWaterTime;
                                    Decimal MannualWater = Decimal.Parse(info.Substring(10, 6)) * (Decimal)0.01;
                                    upData.WaterList.Add(MannualWaterTime, MannualWater);
                                    break;

                                case EOldParam.WaterAddRange:
                                    Decimal WaterAddRange = Decimal.Parse(info.Substring(0, length)); break;
                                case EOldParam.RainAddRange:
                                    Decimal RainAddRange = Decimal.Parse(info.Substring(0, length)); break;
                                case EOldParam.DestPhoneNum:
                                    string DestPhoneNum = info.Substring(0, length); break;
                                case EOldParam.TerminalNum:
                                    string TerminalNum = info.Substring(0, length); break;
                                case EOldParam.RespWave:
                                    string RespWave = info.Substring(0, length); break;
                                case EOldParam.WavePost:
                                    int WavePost = Int32.Parse(info.Substring(0, length)); break;
                                case EOldParam.WorkStatus:
                                    EWorkStatus WorkStatus = OldTypeMaps.WorkStatus4ProtoMap.FindKey(info.Substring(0, length)); break;
                                case EOldParam.StandbyChannel:
                                    ChannelType MainChannel = OldTypeMaps.ChannelType4ProtoMap.FindKey(info.Substring(0, 2));
                                    System.Diagnostics.Debug.Assert(MainChannel != ChannelType.None, "主用信道不能为NONE");
                                    ChannelType ViceChannel = OldTypeMaps.ChannelType4ProtoMap.FindKey(info.Substring(2, 2));
                                    break;
                                case EOldParam.GSMSignal:
                                    Decimal GSMSignal = Decimal.Parse(info.Substring(0, length)); break;
                                case EOldParam.MannualFlow:
                                    DateTime MannualFlowTime = new DateTime(
                                        year: Int32.Parse(info.Substring(0, 2)),
                                        month: Int32.Parse(info.Substring(2, 2)),
                                        day: Int32.Parse(info.Substring(4, 2)),
                                        hour: Int32.Parse(data.Substring(6, 2)),
                                        minute: Int32.Parse(data.Substring(8, 2)),
                                        second: 0
                                        );
                                    recvTime = MannualFlowTime;
                                    upData.WaterFlow = Decimal.Parse(info.Substring(10, 6)) * (Decimal)0.01;
                                    break;

                                case EOldParam.GSMElec:
                                    EGSMElec GSMElec = OldTypeMaps.GSMElec4ProtoMap.FindKey(info.Substring(0, length)); break;
                                case EOldParam.WaterTemp:
                                    upData.WaterTemp = Decimal.Parse(info.Substring(0, length)) * (Decimal)0.1; break;

                                case EOldParam.DataPackNum:
                                    int dataPackNum = Int32.Parse(info.Substring(0, length)); break;
                                default: break;
                            }
                            item = item.Substring(2 + length);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("数据:" + msg);
                            System.Diagnostics.Debug.WriteLine("下行指令解析数据不完整！" + e.Message);
                            break;
                        }

                        //填充基本数据
                        report = new CUpReport()
                        {
                            Stationid = StationID,
                            Type = type,
                            ReportType = reportType,
                            StationType = stationType,
                            RecvTime = recvTime,
                            PackageNum = packageNum,
                            Data = upData
                        };
                        upReport.Add(report);
                    }
                }
                catch (Exception exp)
                {
                    System.Diagnostics.Debug.WriteLine(exp.Message);
                    continue;
                }
            }
            return true;
        }

        public bool Parse_New(string msg, out List<CUpReport> upReport)
        {
            upReport = new List<CUpReport>();
            CUpReport report = null;
            var msgSegs = msg.Split(CSpecialChars.BALNK_CHAR);
            foreach (var msgItem in msgSegs)
            {
                try
                {
                    string data = string.Empty;
                    if (!ProtocolHelpers.DeleteSpecialChar(msgItem, out data))
                    {
                        return false;
                    }

                    //  解析站点ID
                    String stationID = data.Substring(0, 4);
                    //  解析通信类别
                    String type = data.Substring(4, 2);
                    //  解析报文类别
                    EMessageType reportType = ProtocolMaps.MessageTypeMap.FindKey(data.Substring(6, 2));
                    //  解析站点类别
                    EStationType stationType = ProtocolHelpers.ProtoStr2StationType(data.Substring(8, 2));
                    //  解析包序号
                    string packageNum = data.Substring(10, 4);
                    // 解析接收时间
                    DateTime recvTime = new DateTime(
                        year: Int32.Parse("20" + data.Substring(14, 2)),  //年
                        month: Int32.Parse(data.Substring(16, 2)),        //月
                        day: Int32.Parse(data.Substring(18, 2)),          //日
                        hour: Int32.Parse(data.Substring(20, 2)),         //时
                        minute: Int32.Parse(data.Substring(22, 2)),       //分
                        second: 0
                        );

                    //  解析数据
                    CUpData upData = new CUpData();

                    string item = data.Substring(24);

                    while (item.Length > 2)
                    {
                        try
                        {
                            //  数据分为两部分
                            //  2 Byte 指令  +  剩下的为数据，数据的长度>= 1
                            //  解析指令类型param
                            ENewParam param = NewTypeMaps.NewParamMap.FindKey(item.Substring(0, 2));
                            //  如果接收到的数据段长度大于2，表示对应的字段有值
                            //  默认为String.Empty
                            string info = string.Empty;
                            int length = Int32.Parse(NewTypeMaps.NewParamLengthMap.FindValue(param));
                            info = item.Substring(2, length);
                            item = item.Substring(2 + length);
                            if (String.IsNullOrEmpty(info) && ProtocolHelpers.isLegalCharacters(info))
                            {
                                continue;
                            }

                            switch (param)
                            {
                                case ENewParam.Rain:
                                    upData.Rain = Decimal.Parse(info); break;
                                case ENewParam.Water1:
                                    upData.Water1 = Decimal.Parse(info) * (Decimal)0.01; break;
                                case ENewParam.Water2:
                                    upData.Water2 = Decimal.Parse(info) * (Decimal)0.01; break;
                                case ENewParam.Clock:
                                    int year = Int32.Parse("20" + info.Substring(0, 2));
                                    int month = Int32.Parse(info.Substring(2, 2));
                                    int day = Int32.Parse(info.Substring(4, 2));
                                    int hour = Int32.Parse(info.Substring(6, 2));
                                    int minute = Int32.Parse(info.Substring(8, 2));
                                    int second = Int32.Parse(info.Substring(10, 2));
                                    upData.Time = new DateTime(year, month, day, hour, minute, second);
                                    break;
                                case ENewParam.Voltage:
                                    upData.Voltage = (Decimal.Parse(info) * (Decimal)0.01); break;
                                case ENewParam.WaterInterval:
                                    int WaterInterval = Int32.Parse(info); break;
                                case ENewParam.RainInterval:
                                    int RainInterval = Int32.Parse(info); break;
                                case ENewParam.ETimedNum:
                                    int ETimedNum = Int32.Parse(info); break;
                                case ENewParam.ESaveNum:
                                    int ESaveNum = Int32.Parse(info); break;
                                case ENewParam.WaterBase1:
                                    Decimal WaterBase1 = Decimal.Parse(info) * (Decimal)0.01; break;
                                case ENewParam.WaterBase2:
                                    Decimal WaterBase2 = Decimal.Parse(info) * (Decimal)0.01; break;
                                case ENewParam.MannualWater:
                                    DateTime MannualWaterTime = new DateTime(
                                        year: Int32.Parse(info.Substring(0, 2)),
                                        month: Int32.Parse(info.Substring(2, 2)),
                                        day: Int32.Parse(info.Substring(4, 2)),
                                        hour: Int32.Parse(data.Substring(6, 2)),
                                        minute: Int32.Parse(data.Substring(8, 2)),
                                        second: 0
                                        );
                                    Decimal MannualWater = Decimal.Parse(info.Substring(10, 6)) * (Decimal)0.01;
                                    upData.WaterList.Add(MannualWaterTime, MannualWater);
                                    break;
                                case ENewParam.WaterAddRange:
                                    Decimal WaterAddRange = Decimal.Parse(info); break;
                                case ENewParam.RainAddRange:
                                    Decimal RainAddRange = Decimal.Parse(info); break;
                                case ENewParam.DestPhoneNum1:
                                    string DestPhoneNum1 = info; break;
                                case ENewParam.DestPhoneNum2:
                                    string DestPhoneNum2 = info; break;
                                case ENewParam.TerminalNum:
                                    string TerminalNum = info; break;
                                case ENewParam.RespWave:
                                    string RespWave = info; break;
                                case ENewParam.WavePost:
                                    int WavePost = Int32.Parse(info); break;
                                case ENewParam.WorkStatus:
                                    EWorkStatus WorkStatus = OldTypeMaps.WorkStatus4ProtoMap.FindKey(info); break;
                                case ENewParam.StandbyChannel:
                                    ChannelType MainChannel = OldTypeMaps.ChannelType4ProtoMap.FindKey(info);
                                    System.Diagnostics.Debug.Assert(MainChannel != ChannelType.None, "主用信道不能为NONE");
                                    ChannelType ViceChannel = OldTypeMaps.ChannelType4ProtoMap.FindKey(info.Substring(2, 2));
                                    break;
                                case ENewParam.GSMSignal:
                                    Decimal GSMSignal = Decimal.Parse(info); break;
                                case ENewParam.MannualFlow:
                                    DateTime MannualFlowTime = new DateTime(
                                        year: Int32.Parse(info.Substring(0, 2)),
                                        month: Int32.Parse(info.Substring(2, 2)),
                                        day: Int32.Parse(info.Substring(4, 2)),
                                        hour: Int32.Parse(data.Substring(6, 2)),
                                        minute: Int32.Parse(data.Substring(8, 2)),
                                        second: 0
                                        );
                                    upData.Time = MannualFlowTime;
                                    upData.WaterFlow = Decimal.Parse(info.Substring(10, 6)) * (Decimal)0.01;
                                    break;
                                case ENewParam.GSMElec:
                                    EGSMElec GSMElec = OldTypeMaps.GSMElec4ProtoMap.FindKey(info); break;
                                case ENewParam.WaterTemp:
                                    upData.WaterTemp = Decimal.Parse(info) * (Decimal)0.1; break;
                                case ENewParam.FourWaterTemp:
                                    var waterTempList = new Dictionary<DateTime, Decimal>();
                                    DateTime WaterTempTime = recvTime;
                                    for (int i = 1; i <= 4; i++)
                                    {
                                        Decimal WaterTemp = Decimal.Parse(info.Substring(0, 4)) * (Decimal)0.1;
                                        WaterTempTime = recvTime.AddHours(-6 * (i - 1));
                                        info = info.Substring(4);
                                        waterTempList.Add(WaterTempTime, WaterTemp);
                                    }
                                    upData.TempList = waterTempList;
                                    break;
                                case ENewParam.DataPackNum:
                                    int DataPackNum = Int32.Parse(info); break;
                                default: break;
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("数据:" + msg);
                            System.Diagnostics.Debug.WriteLine("下行指令解析数据不完整！" + e.Message);
                            break;
                        }
                        //填充基本数据
                        report = new CUpReport()
                        {
                            Stationid = stationID,
                            Type = type,
                            ReportType = reportType,
                            StationType = stationType,
                            RecvTime = recvTime,
                            PackageNum = packageNum,
                            Data = upData
                        };
                        upReport.Add(report);
                    }
                }
                catch (Exception exp)
                {
                    System.Diagnostics.Debug.WriteLine(exp.Message);
                    continue;
                }
            }
            return true;
        }

        public bool Parse_Old_beidou(string msg, out List<CUpReport> upReport)
        {
            // 这里stationId和type暂时不能获取，写成固定值。
            string stationId = "9999";
            string type = "1G"; ;
            string appendMsg = "$" + stationId + type + ProtocolHelpers.dealBCD(msg) + CSpecialChars.ENTER_CHAR;
            return Parse_Old(appendMsg, out upReport);
        }

        public bool Parse_New_beidou(string msg, out List<CUpReport> upReport)
        {
            // 这里stationId和type暂时不能获取，写成固定值。
            string stationId = "9999";
            string type = "1G";
            string appendMsg = "$" + stationId + type + ProtocolHelpers.dealBCD(msg) + CSpecialChars.ENTER_CHAR;
            return Parse_New(msg, out upReport);
        }

        public bool Parse(string msg, out CReportStruct report)
        {
            throw new NotImplementedException();
        }

        public bool Parse_1(string msg, out CReportStruct report)
        {
            throw new NotImplementedException();
        }
        public bool Parse_2(string msg, out CReportStruct report)
        {
            throw new NotImplementedException();
        }


        public bool Parse_beidou(string msg, out CReportStruct upReport)
        {
            throw new NotImplementedException();
        }


        public bool Parse_beidou(string sid, EMessageType type, string msg, out CReportStruct upReport)
        {
            throw new NotImplementedException();
        }
    }
}
