using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydrology.Entity;
using Protocol.Data.Interface;
namespace Protocol.Data.XYJBX
{
    public class UpParser : IUp
    {
        /// <summary>
        /// 对下游局报讯报文进行处理
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool Parse(String msg, out CReportStruct report)
        {
            //$30151G22010201120326001297065535323906553532390655353239065535323906553532390655353239065535323906553532390655353239065535323906553532390655353239
            report = null;
            try
            {
                string data = string.Empty;
                //卫星信道报
                if (msg.StartsWith("$"))
                {
                    data = ProtocolHelpers.dealBCD(data);

                }
                //去除起始符'$'
                if (!ProtocolHelpers.DeleteSpecialChar(msg, out data))
                {
                    return false;
                }
                //站号（4位）
                string StationId = data.Substring(0, 4);
                //类别（2位）：1G
                string type = data.Substring(4, 2);
                //报类（2位）：22-定时报
                string reportTypeString = data.Substring(6, 2);
                //站类（2位）
                string stationTypeString = data.Substring(8, 2);
                ///0201120326001297065535323906553532390655353239065535323906553532390655353239065535323906553532390655353239065535323906553532390655353239
                //站类区别处理
                switch (reportTypeString)
                {
                    //定时报
                    case "22":
                        {
                            //获取报类
                            EMessageType reportType = ProtocolMaps.MessageTypeMap.FindKey(reportTypeString);
                            //获取站类
                            EStationType stationType = ProtocolHelpers.ProtoStr2StationType(stationTypeString);
                            //包序号暂不处理
                            string packageNum = data.Substring(10, 4);
                            //接收时间
                            DateTime recvTime = new DateTime(
                                year: Int32.Parse("20" + data.Substring(14, 2)),
                                month: Int32.Parse(data.Substring(16, 2)),
                                day: Int32.Parse(data.Substring(18, 2)),
                                hour: Int32.Parse(data.Substring(20, 2)),
                                minute: 0,
                                second: 0
                            );
                            //电压值:1297=12.97V
                            Decimal voltage = Decimal.Parse(data.Substring(22, 4)) * (Decimal)0.01;
                            //数据段
                            var lists = data.Substring(26).Replace(" ", "");
                            var datas = GetData(lists, recvTime, voltage, stationType);
                            report = new CReportStruct()
                            {
                                Stationid = StationId,
                                Type = type,
                                ReportType = reportType,
                                StationType = stationType,
                                RecvTime = recvTime,
                                Datas = datas
                            };
                            break;
                        }
                    //报讯系统加报
                    case "21":
                        {
                            if (data.Substring(8, 2) != "11")
                            {
                                //  解析报文类别
                                EMessageType reportType = ProtocolMaps.MessageTypeMap.FindKey(reportTypeString);
                                //  解析站点类别
                                EStationType stationType = ProtocolHelpers.ProtoStr2StationType(stationTypeString);
                                //  解析接收时间
                                DateTime recvTime = new DateTime(
                                    year: Int32.Parse("20" + data.Substring(10, 2)),  //年
                                    month: Int32.Parse(data.Substring(12, 2)),        //月
                                    day: Int32.Parse(data.Substring(14, 2)),          //日
                                    hour: Int32.Parse(data.Substring(16, 2)),         //时
                                    minute: Int32.Parse(data.Substring(18, 2)),       //分
                                    second: 0       //秒
                                    );

                                var lists = data.Substring(20).Split(CSpecialChars.BALNK_CHAR);
                                var datas = GetAddData(lists, recvTime, stationType);

                                report = new CReportStruct()
                                {
                                    Stationid = StationId,
                                    Type = type,
                                    ReportType = reportType,
                                    StationType = stationType,
                                    RecvTime = recvTime,
                                    Datas = datas
                                };
                            }
                            else
                            {
                                //1G2111为人工水位

                                //  解析报文类别
                                EMessageType reportType = ProtocolMaps.MessageTypeMap.FindKey(reportTypeString);
                                //  解析站点类别
                                EStationType stationType = EStationType.ERiverWater;
                                //  解析接收时间
                                DateTime recvTime = new DateTime(
                                    year: DateTime.Now.Year,  //年
                                    month: DateTime.Now.Month,        //月
                                    day: DateTime.Now.Day,          //日
                                    hour: Int32.Parse(data.Substring(10, 2)),         //时
                                    minute: Int32.Parse(data.Substring(12, 2)),       //分
                                    second: 0       //秒
                                    );

                                var lists = data.Substring(14).Split(CSpecialChars.BALNK_CHAR);
                                var datas = GetMannualData(lists, recvTime);

                                //处理datas为空情况
                                if (datas.Count == 0)
                                {
                                    return false;
                                }

                                report = new CReportStruct()
                                {
                                    Stationid = StationId,
                                    Type = type,
                                    ReportType = reportType,
                                    StationType = stationType,
                                    RecvTime = recvTime,
                                    Datas = datas
                                };
                            }
                            break;
                        }
                    //人工报
                    case "23":
                        {
                            //1G23为人工流量
                            //  解析报文类别
                            EMessageType reportType = EMessageType.EMannual;
                            //  解析站点类别
                            EStationType stationType = EStationType.ERiverWater;
                            //  解析接收时间
                            DateTime recvTime = new DateTime(
                                year: DateTime.Now.Year,  //年
                                month: DateTime.Now.Month,        //月
                                day: Int32.Parse(data.Substring(8, 2)),           //日
                                hour: Int32.Parse(data.Substring(10, 2)),         //时
                                minute: Int32.Parse(data.Substring(12, 2)),       //分
                                second: 0       //秒
                                );

                            var lists = data.Substring(14).Split(CSpecialChars.BALNK_CHAR);
                            var datas = GetWaterData(lists, recvTime);

                            report = new CReportStruct()
                            {
                                Stationid = StationId,
                                Type = type,
                                ReportType = reportType,
                                StationType = stationType,
                                RecvTime = recvTime,
                                Datas = datas
                            };
                            break;
                        }


                }
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("数据：" + msg);
                System.Diagnostics.Debug.WriteLine("数据协议解析不完整" + e.Message);
                return false;
            }
        }

        /// <summary>
        /// 对滁河报文进行处理
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="upReport"></param>
        /// <returns></returns>
        public bool Parse_Chuhe(string msg, out List<CUpReport> upReport)
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
                            EParam param = TypeMaps.TypeCodeMap.FindKey(item.Substring(0, 2));
                            //  如果接收到的数据段长度大于2，表示对应的字段有值
                            //  默认为String.Empty
                            string info = string.Empty;
                            int length = TypeMaps.TypeLengthMap.FindValue(param);
                            info = item.Substring(2);
                            if (String.IsNullOrEmpty(info) && ProtocolHelpers.isLegalCharacters(info))
                            {
                                continue;
                            }
                            switch (param)
                            {
                                //01：雨量加报
                                case EParam.Rain:
                                    upData.Rain = Decimal.Parse(info.Substring(0, length)); break;
                                //02：水位加报
                                case EParam.Water:
                                    upData.Water1 = Decimal.Parse(info.Substring(0, length)) * (Decimal)0.01; break;
                                //03：水位定时报
                                case EParam.WaterTimed:
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
                                //04：12小时电压
                                case EParam.HalfVoltage:
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
                                //05:电池电压
                                case EParam.Voltage:
                                    upData.Voltage = (Decimal.Parse(info.Substring(0, length)) * (Decimal)0.01); break;
                                //06：24小时电压
                                case EParam.AllVoltage:
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
                                //07：采样间隔
                                case EParam.Interval:
                                    int Interval = Int32.Parse(info.Substring(0, length)); break;
                                //08：定时报次数
                                case EParam.ETimedNum:
                                    int ETimedNum = Int32.Parse(info.Substring(0, length)); break;
                                //09：平安报次数
                                case EParam.ESafeNum:
                                    int ESaveNum = Int32.Parse(info.Substring(0, length)); break;
                                //10：水位基值
                                case EParam.WaterBase:
                                    Decimal WaterBase = Decimal.Parse(info.Substring(0, length)) * (Decimal)0.01; break;
                                //11:水位2
                                case EParam.Water2:
                                    Decimal Water2 = Decimal.Parse(info.Substring(0, length)) * (Decimal)0.01; break;
                                //12:人工水位
                                case EParam.ManualWater:
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
                                //13：水位加报阈值
                                case EParam.WaterAddRange:
                                    Decimal WaterAddRange = Decimal.Parse(info.Substring(0, length)); break;
                                //14：雨量加报阈值
                                case EParam.RainAddRange:
                                    Decimal RainAddRange = Decimal.Parse(info.Substring(0, length)); break;
                                //15：目的手机号
                                case EParam.DestPhoneNum:
                                    string DestPhoneNum = info.Substring(0, length); break;
                                //16：水位2基值
                                case EParam.Water2Base:
                                    Decimal Water2Base = Decimal.Parse(info.Substring(0, length)) * (Decimal)0.01; break;
                                //17：北斗目的终端号
                                case EParam.TerminalNum:
                                    string TerminalNum = info.Substring(0, length); break;
                                //18：相应波束
                                case EParam.RespWave:
                                    string RespWave = info.Substring(0, length); break;
                                //19：水位2定时报
                                case EParam.Water2Timed:
                                    var water2List = new Dictionary<DateTime, Decimal>();
                                    int roundNum2 = Int32.Parse(info.Substring(0, 2));
                                    var water2segs = info;
                                    if (roundNum2 == 01 || roundNum2 == 02)
                                    {
                                        roundNum2 = roundNum2 == 01 ? 12 : 24;
                                        length = roundNum2 == 24 ? 96 : 48;
                                        water2segs = info.Substring(2);
                                    }
                                    else
                                    {
                                        roundNum2 = 12;
                                    }
                                    recvTime = recvTime.AddMinutes(-5 * roundNum2);

                                    for (int i = 1; i <= roundNum2; i++)
                                    {
                                        DateTime Water2Time = recvTime.AddMinutes(5);
                                        Decimal TimedWater2 = Decimal.Parse(water2segs.Substring(0, 4)) * (Decimal)0.01;
                                        water2List.Add(Water2Time, TimedWater2);
                                        water2segs = water2segs.Substring(4);
                                    }
                                    upData.WaterList2 = water2List;
                                    break;
                                //20：工作状态
                                case EParam.WorkStatus:
                                    EWorkStatus WorkStatus = TypeMaps.WorkStatus4ProtoMap.FindKey(info.Substring(0, length)); break;
                                //21：主备信道
                                case EParam.StandbyChannel:
                                    ChannelType MainChannel = TypeMaps.ChannelType4ProtoMap.FindKey(info.Substring(0, 2));
                                    System.Diagnostics.Debug.Assert(MainChannel != ChannelType.None, "主用信道不能为NONE");
                                    ChannelType ViceChannel = TypeMaps.ChannelType4ProtoMap.FindKey(info.Substring(2, 2));
                                    break;
                                //22：GSM信号场强
                                case EParam.GSMSignal:
                                    Decimal GSMSignal = Decimal.Parse(info.Substring(0, length)); break;
                                //23：人工流量
                                case EParam.ManualFlow:
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
                                //24：GSM控电模式
                                case EParam.GSMElec:
                                    EGSMElec GSMElec = TypeMaps.GSMElec4ProtoMap.FindKey(info.Substring(0, length)); break;
                                //25：雨量定时报
                                case EParam.RainTimed:
                                    //将雨量列表存入report
                                    var rainList = new Dictionary<DateTime, Decimal>();
                                    var rainSegs = info;
                                    recvTime = recvTime.AddMinutes(-5 * 12);

                                    for (int i = 1; i <= 12; i++)
                                    {
                                        DateTime rainTime = recvTime.AddMinutes(5);
                                        Decimal rain = Decimal.Parse(rainSegs.Substring(0, 4)) * (Decimal)0.01;
                                        rainList.Add(rainTime, rain);
                                        rainSegs = rainSegs.Substring(4);
                                    }
                                    upData.WaterList = rainList;
                                    break;
                                //30：数据包数
                                case EParam.DataPackNum:
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
        /// <summary>
        /// 对卫星信道的滁河报文进行处理
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="upReport"></param>
        /// <returns></returns>
        public bool Parse_Chuhe_Beidou(string msg, out List<CUpReport> upReport)
        {
            // 这里stationId和type暂时不能获取，写成固定值。
            string stationId = "9999";
            string type = "1G"; ;
            string appendMsg = "$" + stationId + type + ProtocolHelpers.dealBCD(msg) + CSpecialChars.ENTER_CHAR;
            return Parse_Chuhe(msg, out upReport);
        }

        /// <summary>
        /// 针对后面的连续数据项，逐个数据项进行填充，最后将数据结果作为list返回。
        /// </summary>
        /// <param name="list">原数据字符串</param>
        /// <param name="recvtime">接收数据时间，作为数据项的起始时间逐个往前推算。</param>
        /// <param name="voltage">电压值，每个数据项都要填充上电压信息</param>
        /// <param name="stationType">站点类型，作为读取数据部分的依据：需要读取雨量还是水位信息。</param>
        /// <returns>列表形式的数据集</returns>
        private List<CReportData> GetData(string list, DateTime recvtime, Decimal voltage, EStationType stationType)
        {
            var result = new List<CReportData>();
            string tmp;
            for (int i = 0; i < 12; i++)
            {
                CReportData data = new CReportData();
                tmp = list.Substring(i * 10, 10);
                if (FillData(tmp, recvtime, voltage, stationType, out data))
                {
                    result.Add(data);
                }
                recvtime = recvtime.AddMinutes(-5);
            }
            return result;
        }
        /// <summary>
        /// 单个数据项读取函数
        /// </summary>
        /// <param name="data">原始数据信息</param>
        /// <param name="recvtime">接收时间</param>
        /// <param name="voltage">电压信息</param>
        /// <param name="stationType">站点类型，作为读取数据的依据</param>
        /// <param name="report">作为返回的结果</param>
        /// <returns>填充成功与否</returns>
        private bool FillData(string data, DateTime recvtime, Decimal voltage, EStationType stationType, out CReportData report)
        {
            report = new CReportData();
            try
            {
                //时间
                report.Time = recvtime;
                //电压
                report.Voltge = voltage;
                //根据站类读取相应的数据
                switch (stationType)
                {
                    //水位站只要读水位信息
                    case EStationType.ERiverWater:
                        report.Water = Decimal.Parse(data.Substring(0, 6)) * (Decimal)0.01;
                        break;
                    //雨量站只要读雨量信息
                    case EStationType.ERainFall:
                        report.Rain = Decimal.Parse(data.Substring(6, 4));
                        break;
                    //水文站要读水位信息和雨量信息
                    case EStationType.EHydrology:
                        report.Water = Decimal.Parse(data.Substring(0, 6)) * (Decimal)0.01;
                        report.Rain = Decimal.Parse(data.Substring(6, 4));
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("解析XYJBX信息中站点类别有误！");
                        break;
                }
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
        }
        /// <summary>
        /// 获取人工报数据信息，人工报报类：23
        /// </summary>
        /// <param name="dataSegs"></param>
        /// <param name="recvTime"></param>
        /// <returns></returns>
        private List<CReportData> GetWaterData(IList<string> dataSegs, DateTime recvTime)
        {
            var result = new List<CReportData>();
            foreach (var item in dataSegs)
            {
                CReportData data = new CReportData();
                //  解析时间
                data.Time = recvTime;
                //  解析电压  无电压设置为0
                data.Voltge = 0;
                //解析数据
                //  水位
                //  解析水位  4(整数位) + 2(小数位)  单位m
                Decimal water = Decimal.Parse(item.Substring(0, 6)) * (Decimal)0.01;
                data.Water = water;
                //  水势  暂时用不上
                Decimal waterPot = Decimal.Parse(item.Substring(6, 1));
                //  精度
                Int32 accuracy = Int32.Parse(item.Substring(7, 1));
                //  有效数
                Decimal EffectiveNum = Decimal.Parse(item.Substring(8, 3));
                //  流量  暂时不传
                Decimal waterflow = EffectiveNum * (10 ^ (accuracy - 3));
                //  测流方法
                String MeasureType = item.Substring(11, 1);

                result.Add(data);
            }
            return result;
        }
        /// <summary>
        /// 读取人工报报文信息，人工报报类：23
        /// </summary>
        /// <param name="dataSegs"></param>
        /// <param name="recvTime"></param>
        /// <returns></returns>
        private List<CReportData> GetMannualData(IList<string> dataSegs, DateTime recvTime)
        {
            var result = new List<CReportData>();
            foreach (var item in dataSegs)
            {
                CReportData data = new CReportData();
                //  解析时间
                data.Time = recvTime;
                //  解析电压  无电压设置为0
                data.Voltge = 0;
                //解析数据
                //  水位
                //  解析水位  4(整数位) + 2(小数位)  单位m
                Decimal water = Decimal.Parse(item.Substring(0, 6)) * (Decimal)0.01;
                data.Water = water;
                //  水势  暂时用不上
                Decimal waterPot = Decimal.Parse(item.Substring(6, 2));

                result.Add(data);
            }
            return result;
        }
        /// <summary>
        /// 读取加报报文的数据内容，加报报类：21
        /// 加报仅有1条数据
        /// </summary>
        /// <param name="dataSegs"></param>
        /// <param name="recvTime"></param>
        /// <param name="stationType"></param>
        /// <returns></returns>
        private List<CReportData> GetAddData(IList<string> dataSegs, DateTime recvTime, EStationType stationType)
        {
            var result = new List<CReportData>();
            foreach (var item in dataSegs)
            {
                CReportData data = new CReportData();
                //  解析时间
                data.Time = recvTime;
                //  根据站点类型解析数据
                switch (stationType)
                {
                    case EStationType.ERainFall:
                        {
                            //  雨量
                            //  解析雨量                         单位mm，未乘以精度
                            Decimal rain = Decimal.Parse(item.Substring(6, 4));
                            data.Rain = rain;
                        }
                        break;
                    case EStationType.EHydrology:
                        {
                            //  水文
                            //  解析雨量                         单位mm，未乘以精度
                            Decimal rain = Decimal.Parse(item.Substring(6, 4));
                            //  解析水位  4(整数位) + 2(小数位)  单位m
                            Decimal water = Decimal.Parse(item.Substring(0, 6)) * (Decimal)0.01;
                            data.Rain = rain;
                            data.Water = water;
                        }
                        break;
                    case EStationType.ERiverWater:
                        {
                            //  水位
                            //  解析水位  4(整数位) + 2(小数位)  单位m
                            Decimal water = Decimal.Parse(item.Substring(0, 6)) * (Decimal)0.01;
                            data.Water = water;
                            break;
                        }
                    default: break;
                }
                //解析电压 2（整数位）+ 2（小数位） 单位 V
                Decimal voltage = Decimal.Parse(item.Substring(10, 4)) * (Decimal)0.01;
                data.Voltge = voltage;
                result.Add(data);
            }
            return result;
        }

        public bool Parse_1(string msg, out CReportStruct report)
        {
            throw new NotImplementedException();
        }

        public bool Parse_2(string msg, out CReportStruct report)
        {
            throw new NotImplementedException();
        }

        public bool Parse_Old(string msg, out List<CUpReport> upReport)
        {
            throw new NotImplementedException();
        }

        public bool Parse_New(string msg, out List<CUpReport> upReport)
        {
            throw new NotImplementedException();
        }

        public bool Parse_Old_beidou(string msg, out List<CUpReport> upReport)
        {
            throw new NotImplementedException();
        }

        public bool Parse_New_beidou(string msg, out List<CUpReport> upReport)
        {
            throw new NotImplementedException();
        }


        public bool Parse_beidou(string sid, EMessageType type, string msg, out CReportStruct upReport)
        {
            throw new NotImplementedException();
        }
    }
}
