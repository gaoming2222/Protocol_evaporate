using Hydrology.Entity;
using System;
using System.Collections.Generic;
using Protocol.Data.Interface;

namespace Protocol.Data.ZYJBX
{
    public class UpParser : IUp
    {
        //用于保存包序号，并写入缓存
        public static Dictionary<String, CEntityPackage> cEntityPackage = new Dictionary<string, CEntityPackage>();

        /// <summary>
        /// 非卫星报文数据解析过程
        /// </summary>
        /// <param name="msg">原始报文数据</param>
        /// <param name="report">报文最终解析出的结果数据结构</param>
        /// <returns>是否解析成功</returns>
        public bool Parse(String msg, out CReportStruct report)
        {
            //$80001G22130102181205011327002847069300284706930028460693002847069300284706930028470693002847069300284706930028460693002843069300284606930028430693
            //$30151G22010201120326001297065535323906553532390655353239065535323906553532390655353239065535323906553532390655353239065535323906553532390655353239
            report = null;
            try
            {
                string data = string.Empty;
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

                EMessageType reportType;
                EStationType stationType;
                string packageNum;
                DateTime recvTime;
                Decimal Voltage;
                string lists;
                List<CReportData> datas;
                ///0201120326001297065535323906553532390655353239065535323906553532390655353239065535323906553532390655353239065535323906553532390655353239
                ///$66001G2513010118120500001251002945103100294410310029451031002945103100294510310029441031
                //站类区别处理
                switch (reportTypeString)
                {
                    //定时报新增48段次定时报类
                    case "25":
                        //获取报类
                        reportTypeString = "22";
                        reportType = ProtocolMaps.MessageTypeMap.FindKey(reportTypeString);
                        //获取站类
                        stationType = ProtocolHelpers.ProtoStr2StationType(stationTypeString);
                        //包序号暂不处理
                        packageNum = data.Substring(10, 4);
                        //接收时间
                        recvTime = new DateTime(
                            year: Int32.Parse("20" + data.Substring(14, 2)),
                            month: Int32.Parse(data.Substring(16, 2)),
                            day: Int32.Parse(data.Substring(18, 2)),
                            hour: Int32.Parse(data.Substring(20, 2)),
                            minute: Int32.Parse(data.Substring(22, 2)),
                            second: 0
                        );
                        //电压值:1297=12.97V
                        Voltage = Decimal.Parse(data.Substring(24, 4)) * (Decimal)0.01;
                        //数据段
                        lists = data.Substring(28).Replace(" ", "");
                        datas = GetData_1(lists, recvTime, Voltage, stationType);
                        report = new CReportStruct()
                        {
                            Stationid = StationId,
                            Type = type,
                            ReportType = reportType,
                            StationType = stationType,
                            RecvTime = DateTime.Now,
                            Datas = datas
                        };
                        break;
                    //定时报
                    case "22":
                        //获取报类
                        reportType = ProtocolMaps.MessageTypeMap.FindKey(reportTypeString);
                        //获取站类
                        stationType = ProtocolHelpers.ProtoStr2StationType(stationTypeString);
                        //包序号
                        packageNum = data.Substring(10, 4);
                        //接收时间
                        recvTime = new DateTime(
                            year: Int32.Parse("20" + data.Substring(14, 2)),
                            month: Int32.Parse(data.Substring(16, 2)),
                            day: Int32.Parse(data.Substring(18, 2)),
                            hour: Int32.Parse(data.Substring(20, 2)),
                            minute: 0,
                            second: 0
                        );
                        //把包序号写入缓存
                        CEntityPackage package = new CEntityPackage()
                        {
                            StrStationID = StationId,
                            PackageNum = packageNum,
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
                        //电压值:1297=12.97V
                        Voltage = Decimal.Parse(data.Substring(22, 4)) * (Decimal)0.01;
                        //数据段
                        lists = data.Substring(26).Replace(" ", "");
                        datas = GetData(lists, recvTime, Voltage, stationType);
                        report = new CReportStruct()
                        {
                            Stationid = StationId,
                            Type = type,
                            ReportType = reportType,
                            StationType = stationType,
                            RecvTime = DateTime.Now,
                            Datas = datas
                        };
                        break;
                    //报讯系统加报
                    case "21":
                        {
                            if (data.Substring(8, 2) != "11")
                            {
                                //  解析报文类别
                                reportType = ProtocolMaps.MessageTypeMap.FindKey(reportTypeString);
                                //  解析站点类别
                                stationType = ProtocolHelpers.ProtoStr2StationType(stationTypeString);
                                //  解析接收时间
                                recvTime = new DateTime(
                                    year: Int32.Parse("20" + data.Substring(10, 2)),  //年
                                    month: Int32.Parse(data.Substring(12, 2)),        //月
                                    day: Int32.Parse(data.Substring(14, 2)),          //日
                                    hour: Int32.Parse(data.Substring(16, 2)),         //时
                                    minute: Int32.Parse(data.Substring(18, 2)),       //分
                                    second: 0       //秒
                                    );

                                var dataLists = data.Substring(20).Split(CSpecialChars.BALNK_CHAR);
                                datas = GetAddData(dataLists, recvTime, stationType);

                                report = new CReportStruct()
                                {
                                    Stationid = StationId,
                                    Type = type,
                                    ReportType = reportType,
                                    StationType = stationType,
                                    RecvTime = DateTime.Now,
                                    Datas = datas
                                };
                            }
                            break;
                        }
                }
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("数据：" + msg);
                System.Diagnostics.Debug.WriteLine("中游局协议解析不完整" + e.Message);
                //return false;
            }
            return false;
        }

        /// <summary>
        /// 卫星报文数据解析过程，是在非卫星报文数据的基础上增加了BCD编码。
        /// </summary>
        /// <param name="msg">原始报文数据</param>
        /// <param name="upReport">报文最终解析出的结果数据结构</param>
        /// <returns>是否解析成功</returns>
        public bool Parse_beidou(string sid, EMessageType msgType, string msg, out CReportStruct upReport)
        {
            // 这里stationId和type暂时不能获取，写成固定值。
            //string stationId = "9999";
            string type = "1G"; ;
            string appendMsg = "$" + sid + type + ProtocolHelpers.dealBCD(msg) + CSpecialChars.ENTER_CHAR;
            return Parse(appendMsg, out upReport);
        }

        /// <summary>
        /// 针对后面的连续数据项，逐个数据项进行填充，最后将数据结果作为list返回。
        /// </summary>
        /// <param name="list">原数据字符串</param>
        /// <param name="recvtime">接收数据时间，作为数据项的起始时间逐个往前推算。</param>
        /// <param name="voltage">电压值，每个数据项都要填充上电压信息</param>
        /// <param name="stationType">站点类型，作为读取数据部分的依据：需要读取雨量还是水位信息。</param>
        /// <returns>列表形式的数据集</returns>
        private List<CReportData> GetData(string list, DateTime recvtime, decimal voltage, EStationType stationType)
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
            // 将结果中的数据顺序逆置
            result.Reverse();
            return result;
        }
        private List<CReportData> GetData_1(string list, DateTime recvtime, decimal voltage, EStationType stationType)
        {
            var result = new List<CReportData>();
            string tmp;
            
            for (int i = 0; i < 6; i++)
            {
                CReportData data = new CReportData();
                if(list.Length >= (i*10 + 10))
                {
                    tmp = list.Substring(i * 10, 10);
                    if (FillData(tmp, recvtime, voltage, stationType, out data))
                    {
                        result.Add(data);
                    }
                    recvtime = recvtime.AddMinutes(-5);
                }
            }
            // 将结果中的数据顺序逆置
            result.Reverse();
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
                            try
                            {
                                Decimal rain = Decimal.Parse(item.Substring(6, 4));
                                data.Rain = rain;
                            }
                            catch (Exception e)
                            {
                                data.Rain = -1;
                            }
                        }
                        break;
                    case EStationType.EHydrology:
                        {
                            //  水文
                            //  解析雨量                         单位mm，未乘以精度
                            try
                            {
                                Decimal rain = Decimal.Parse(item.Substring(6, 4));
                                data.Rain = rain;
                            }
                            catch (Exception e)
                            {
                                data.Rain = -1;
                            }
                            //  解析水位  4(整数位) + 2(小数位)  单位m
                            try
                            {
                                Decimal water = Decimal.Parse(item.Substring(0, 6)) * (Decimal)0.01;
                                data.Water = water;
                            }
                            catch (Exception e)
                            {
                                data.Water = -200;
                            }
                        }
                        break;
                    case EStationType.ERiverWater:
                        {
                            //  水位
                            //  解析水位  4(整数位) + 2(小数位)  单位m
                            try
                            {
                                Decimal water = Decimal.Parse(item.Substring(0, 6)) * (Decimal)0.01;
                                data.Water = water;
                            }
                            catch (Exception e)
                            {
                                data.Water = -200;
                            }
                            break;
                        }
                    default: break;
                }
                //解析电压 2（整数位）+ 2（小数位） 单位 V
                try
                {
                    Decimal voltage = Decimal.Parse(item.Substring(10, 4)) * (Decimal)0.01;
                    data.Voltge = voltage;
                }
                catch (Exception e)
                {
                    data.Voltge = -20;
                }
                result.Add(data);
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
        private bool FillData(string data, DateTime recvtime, decimal voltage, EStationType stationType, out CReportData report)
        {
            report = new CReportData();
            try
            {
                //时间
                report.Time = recvtime;
                //电压
                if (report.Time.Minute != 0)
                { report.Voltge = 0; }
                else { report.Voltge = voltage; }
                //根据站类读取相应的数据
                switch (stationType)
                {
                    //水位站只要读水位信息
                    case EStationType.ERiverWater:
                        try
                        {
                            Decimal water = Decimal.Parse(data.Substring(0, 6)) * (Decimal)0.01;
                            report.Water = water;
                        }
                        catch (Exception e)
                        {
                            report.Water = -20000;
                        }
                        break;
                    //雨量站只要读雨量信息
                    case EStationType.ERainFall:
                        try
                        {
                            Decimal Rain = Decimal.Parse(data.Substring(6, 4));
                            report.Rain = Rain;
                        }
                        catch (Exception e)
                        {
                            report.Rain = -1;
                        }
                        break;
                    //水文站要读水位信息和雨量信息
                    case EStationType.EHydrology:
                        try
                        {
                            Decimal water = Decimal.Parse(data.Substring(0, 6)) * (Decimal)0.01;
                            report.Water = water;
                        }
                        catch (Exception e)
                        {
                            report.Water = -20000;
                        }
                        try
                        {
                            Decimal Rain = Decimal.Parse(data.Substring(6, 4));
                            report.Rain = Rain;
                        }
                        catch (Exception e)
                        {
                            report.Rain = -1;
                        }
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("解析中游局信息中站点类别有误！");
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

        
        public bool Parse_1(string msg, out CReportStruct report)
        {
            throw new NotImplementedException();
        }

        public bool Parse_2(string msg, out CReportStruct report)
        {
            throw new NotImplementedException();
        }
    }
}
