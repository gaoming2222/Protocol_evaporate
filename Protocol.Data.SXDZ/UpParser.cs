using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydrology.Entity;
using Protocol.Data.Interface;

namespace Protocol.Data.SXDZ
{
    public class UpParser : IUp
    {

        public static Dictionary<String, CEntityPackage> cEntityPackage = new Dictionary<string, CEntityPackage>();
        public bool Parse(String msg, out CReportStruct report)
        {
            //$30011G2512010030yymmddhhmm1297001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100001100;
            report = null;
            try
            {
                string data = string.Empty;
                if (!ProtocolHelpers.DeleteSpecialChar(msg, out data))
                    return false;

                //  解析站号
                String StationId = data.Substring(0, 4);
                //  解析通信类别
                String type = data.Substring(4, 2);
                //定义报文类别
                String reportTypeString = data.Substring(6, 2);
                //定义站点类别
                String stationTypeString = data.Substring(8, 2);
                //  根据报文类别进行区别处理
                switch (reportTypeString)
                {
                    case "25":
                        {
                            //  解析报文类别
                            reportTypeString = "22";
                            EMessageType reportType = ProtocolMaps.MessageTypeMap.FindKey(reportTypeString);
                            //  解析站点类别
                            EStationType stationType = ProtocolHelpers.ProtoStr2StationType(stationTypeString);
                            //解析间隔分钟
                            int intervalMin = Int32.Parse(data.Substring(10, 2));
                            //解析数据个数
                            int dataNum = Int32.Parse(data.Substring(12, 4));
                            //解析接收时间
                            DateTime recvTime = new DateTime(
                                year: Int32.Parse("20" + data.Substring(16, 2)),  //年
                                month: Int32.Parse(data.Substring(18, 2)),        //月
                                day: Int32.Parse(data.Substring(20, 2)),          //日
                                hour: Int32.Parse(data.Substring(22, 2)),         //时
                                minute: Int32.Parse(data.Substring(24, 2)),       //分
                                second: 0
                                );
                            //解析电压 2（整数位）+ 2（小数位） 单位 V
                            Decimal Voltage = Decimal.Parse(data.Substring(26, 4)) * (Decimal)0.01;
                            //获取数据段，不包括间隔分钟、数据个数、电压等
                            var lists = data.Substring(30).Split(CSpecialChars.BALNK_CHAR);
                            var datas = GetProData(intervalMin, dataNum, lists, recvTime, Voltage);

                            report = new CReportStruct()
                            {
                                Stationid = StationId,
                                Type = type,
                                ReportType = reportType,
                                StationType = stationType,
                                RecvTime = recvTime,
                                Datas = datas
                            };
                        } break;
                    case "22":
                        {
                            //  解析报文类别
                            EMessageType reportType = ProtocolMaps.MessageTypeMap.FindKey(reportTypeString);
                            //  解析站点类别
                            EStationType stationType = ProtocolHelpers.ProtoStr2StationType(stationTypeString);
                            //解析包序号
                            String PackageNum = data.Substring(10, 4);
                            //解析接收时间
                            DateTime recvTime = new DateTime(
                                year: Int32.Parse("20" + data.Substring(14, 2)),  //年
                                month: Int32.Parse(data.Substring(16, 2)),        //月
                                day: Int32.Parse(data.Substring(18, 2)),          //日
                                hour: Int32.Parse(data.Substring(20, 2)),         //时
                                minute: 0,       //分
                                second: 0       //秒
                                );
                            //把包序号写入缓存
                            CEntityPackage package = new CEntityPackage()
                            {
                                StrStationID = StationId,
                                PackageNum = PackageNum,
                                time = recvTime
                            };
                            if (cEntityPackage.ContainsKey(StationId))
                            {

                                cEntityPackage[StationId] = package;
                            }
                            else
                            {
                                cEntityPackage.Add(StationId, package);
                            }
                            //解析电压 2（整数位）+ 2（小数位） 单位 V
                            Decimal Voltage = Decimal.Parse(data.Substring(22, 4)) * (Decimal)0.01;
                            //获取数据段，不包括间隔分钟、数据个数、电压等
                            var lists = data.Substring(26).Split(CSpecialChars.BALNK_CHAR);
                            var datas = GetData(lists, recvTime, Voltage, stationType);

                            report = new CReportStruct()
                            {
                                Stationid = StationId,
                                Type = type,
                                ReportType = reportType,
                                StationType = stationType,
                                RecvTime = recvTime,
                                Datas = datas
                            };
                        } break;
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
                                EMessageType reportType = EMessageType.EMannual;
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
                        } break;
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
                        } break;
                    case "29":
                        {
                            string doubleMsg = data.Substring(8);
                            Parse(doubleMsg, out report);
                        } break;
                }
                return true;
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine("数据：" + msg);
                System.Diagnostics.Debug.WriteLine("数据协议解析不完整" + exp.Message);
            }
            return false;
        }

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
                        } break;
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

        private List<CReportData> GetData(IList<string> lists, DateTime recvtime, decimal voltage, EStationType stationType)
        {
            var result = new List<CReportData>();
            foreach (var item in lists)
            {
                string itemData = item;
                string itemProcess = item;
                for (int i = 1; i <= 12; i++)
                {
                    CReportData data = new CReportData();
                    itemProcess = itemData.Substring(0, 10);
                    if (GetData(itemProcess, recvtime, voltage, stationType, out data))
                    {
                        result.Add(data);
                    }
                    recvtime = recvtime.AddMinutes(-5);
                    itemData = itemData.Substring(10);
                }
            }
            return result;
        }

        private List<CReportData> GetProData(int intervalMin, int dataNum, IList<string> lists, DateTime recvtime, decimal voltage)
        {
            var result = new List<CReportData>();
            foreach (var item in lists)
            {
                string itemData = item;
                string itemProcess = itemData;
                for (int i = 1; i <= dataNum; i++)
                {
                    CReportData data = new CReportData();
                    itemProcess = itemData.Substring(0, 6);
                    if (GetProData(intervalMin, itemProcess, recvtime, voltage, out data))
                    {
                        result.Add(data);
                    }
                    recvtime = recvtime.AddMinutes(-1);
                    itemData = itemData.Substring(6);
                }
            }
            return result;
        }

        //解析数据段
        private bool GetProData(int intervalMin, string data, DateTime recvtime, decimal voltage, out CReportData report)
        {

            report = new CReportData();
            try
            {
                //解析数据相关信息
                //解析时间
                report.Time = recvtime;
                //解析电压 2（整数位）+ 2（小数位） 单位 V
                report.Voltge = voltage;
                //解析水位 4（整数位）+ 2（小数位） 单位m
                //因为是定制水位定时报，所以只用解析水位数据
                Decimal water = Decimal.Parse(data) * (Decimal)0.01;
                report.Water = water;
                return true;
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.Message);
            }
            return false;
        }

        //解析数据段
        private bool GetData(string data, DateTime recvTime, decimal voltage, EStationType stationType, out CReportData report)
        {

            report = new CReportData();
            try
            {//  解析时间
                report.Time = recvTime;

                //  解析电压  2(整数位) + 2(小数位)  单位V
                report.Voltge = voltage;

                // 此处根据站点类型类判断是否需要解析相应的数据，避免因为非必要字段导致的异常信息
                //  解析水位  4(整数位) + 2(小数位)  单位m
                //  解析雨量                         单位mm，未乘以精度
                //  初始化雨量，水位，电压值
                //  雨量  包含雨量Rain
                //  水文  包含雨量Rain，水位Water
                //  水位  包含水位Water
                switch (stationType)
                {
                    case EStationType.ERainFall:
                        {
                            //  雨量
                            //  解析雨量                         单位mm，未乘以精度
                            Decimal rain = Decimal.Parse(data.Substring(6, 4));
                            report.Rain = rain;
                        } break;
                    case EStationType.EHydrology:
                        {
                            //  水文
                            //  解析雨量                         单位mm，未乘以精度
                            Decimal rain = Decimal.Parse(data.Substring(6, 4));
                            //  解析水位  4(整数位) + 2(小数位)  单位m
                            Decimal water = Decimal.Parse(data.Substring(0, 6)) * (Decimal)0.01;
                            report.Rain = rain;
                            report.Water = water;
                        }
                        break;
                    case EStationType.ERiverWater:
                        {
                            //  水位
                            //  解析水位  4(整数位) + 2(小数位)  单位m
                            Decimal water = Decimal.Parse(data.Substring(0, 6)) * (Decimal)0.01;
                            report.Water = water;
                            break;
                        }
                    default: break;
                }

                return true;
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.Message);
            }
            return false;
        }


        public bool Parse_1(string msg, out CReportStruct report)
        {
            throw new NotImplementedException();
        }

        public bool Parse_2(string msg, out CReportStruct report)
        {
            throw new NotImplementedException();
        }

        //  北斗信道数据解析
        public bool Parse_beidou(string sid, EMessageType type, string msg, out CReportStruct report)
        {
            // 这里stationId和type暂时不能获取，写成固定值。
            //string stationId = "9999";
            //string type = "1G";
            string appendMsg = "$" + sid + type + ProtocolHelpers.dealBCD(msg) + CSpecialChars.ENTER_CHAR;
            return Parse(msg, out report);
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
    }
}
