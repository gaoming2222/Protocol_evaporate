/************************************************************************************
* Copyright (c) 2018 All Rights Reserved.
*命名空间：Protocol.Channel.Reservoir
*文件名： UpPaser
*创建人： XXX
*创建时间：2018-12-29 17:40:06
*描述
*=====================================================================
*修改标记
*修改时间：2018-12-29 17:40:06
*修改人：XXX
*描述：
************************************************************************************/
using Hydrology.Entity;
using System;
using System.Collections.Generic;
using Protocol.Data.Interface;

namespace Protocol.Channel.Reservoir
{
    public class UpPaser : IUp
    {
        public bool Parse(string msg, out CReportStruct report)
        {
            throw new NotImplementedException();
        }

        //23001G221812291500????12980064@@Z1001234Z2001234Z3001234Z4001234Z5001234Z6001234Z7001234Z8001234\r\n
        public bool Parse(String msg, out List<CReportStruct> reportList)
        {
            reportList = new List<CReportStruct>();
            CReportStruct report = new CReportStruct();
            try
            {
                //站号（4位）
                string StationId = msg.Substring(0, 4);
                //类别（2位）：1G
                string type = msg.Substring(4, 2);
                //报类（2位）：22-定时报
                string reportTypeString = msg.Substring(6, 2);

                EMessageType reportType;
                EStationType stationType;
                DateTime recvTime;
                Decimal Voltage = 0;
                Decimal rain = 0;

                switch (reportTypeString)
                {
                    case "21":
                    case "22":
                        reportType = ProtocolMaps.MessageTypeMap.FindKey(reportTypeString);
                        stationType = EStationType.EReservoir;
                        recvTime = new DateTime(
                            year: Int32.Parse("20" + msg.Substring(8, 2)),
                            month: Int32.Parse(msg.Substring(10, 2)),
                            day: Int32.Parse(msg.Substring(12, 2)),
                            hour: Int32.Parse(msg.Substring(14, 2)),
                            minute: Int32.Parse(msg.Substring(16, 2)),
                            second: 0
                        );
                        try
                        {
                            rain = Decimal.Parse(msg.Substring(18, 4));
                        }
                        catch (Exception e)
                        {
                            rain = -1;
                        }
                        try
                        {
                            Voltage = Decimal.Parse(msg.Substring(22, 4)) * (Decimal)0.01;
                        }catch(Exception e)
                        {
                            Voltage = 0;
                        }
                        
                        if (msg.Contains("@@"))
                        {
                            
                            int flagIndex = msg.IndexOf("@@");
                            if (flagIndex > 0)
                            {
                                //水位1
                                int stationId1 = int.Parse(StationId);
                                string waterStr1 = msg.Substring(flagIndex + 2, 8);
                                if(waterStr1 != "-2000000")
                                {
                                    Decimal water1 = decimal.Parse(waterStr1) * (Decimal)0.01;
                                    if(water1 < 0 || water1 > 655)
                                    {
                                        water1 = 0;
                                    }
                                    List<CReportData> datas = new List<CReportData>();
                                    CReportData data = new CReportData();
                                    data.Rain = rain;
                                    data.Voltge = Voltage;
                                    data.Water = water1;
                                    data.Time = recvTime;
                                    datas.Add(data);
                                    report = new CReportStruct()
                                    {
                                        Stationid = stationId1.ToString(),
                                        Type = type,
                                        ReportType = reportType,
                                        StationType = stationType,
                                        RecvTime = DateTime.Now,
                                        Datas = datas
                                    };
                                    reportList.Add(report);
                                }

                                //水位2
                                int stationId2 = int.Parse(StationId) + 1;
                                string waterStr2 = msg.Substring(flagIndex + 2 + 8*1, 8);
                                if (waterStr2 != "-2000000")
                                {
                                    Decimal water2 = decimal.Parse(waterStr2) * (Decimal)0.01;
                                    if (water2 < 0 || water2 > 655)
                                    {
                                        water2 = 0;
                                    }
                                    List<CReportData> datas = new List<CReportData>();
                                    CReportData data = new CReportData();
                                    data.Rain = rain;
                                    data.Voltge = Voltage;
                                    data.Water = water2;
                                    data.Time = recvTime;
                                    datas.Add(data);
                                    report = new CReportStruct()
                                    {
                                        Stationid = stationId2.ToString(),
                                        Type = type,
                                        ReportType = reportType,
                                        StationType = stationType,
                                        RecvTime = DateTime.Now,
                                        Datas = datas
                                    };
                                    reportList.Add(report);
                                }

                                //水位3
                                int stationId3 = int.Parse(StationId) + 2;
                                string waterStr3 = msg.Substring(flagIndex + 2 + 8*2, 8);
                                if (waterStr3 != "-2000000")
                                {
                                    Decimal water3 = decimal.Parse(waterStr3) * (Decimal)0.01;
                                    if (water3 < 0 || water3 > 655)
                                    {
                                        water3 = 0;
                                    }
                                    List<CReportData> datas = new List<CReportData>();
                                    CReportData data = new CReportData();
                                    data.Rain = rain;
                                    data.Voltge = Voltage;
                                    data.Water = water3;
                                    data.Time = recvTime;
                                    datas.Add(data);
                                    report = new CReportStruct()
                                    {
                                        Stationid = stationId3.ToString(),
                                        Type = type,
                                        ReportType = reportType,
                                        StationType = stationType,
                                        RecvTime = DateTime.Now,
                                        Datas = datas
                                    };
                                    reportList.Add(report);
                                }

                                //水位4
                                int stationId4 = int.Parse(StationId) + 3;
                                string waterStr4 = msg.Substring(flagIndex + 2 + 8 * 3, 8);
                                if (waterStr4 != "-2000000")
                                {
                                    Decimal water4 = decimal.Parse(waterStr4) * (Decimal)0.01;
                                    if (water4 < 0 || water4 > 655)
                                    {
                                        water4 = 0;
                                    }
                                    List<CReportData> datas = new List<CReportData>();
                                    CReportData data = new CReportData();
                                    data.Rain = rain;
                                    data.Voltge = Voltage;
                                    data.Water = water4;
                                    data.Time = recvTime;
                                    datas.Add(data);
                                    report = new CReportStruct()
                                    {
                                        Stationid = stationId4.ToString(),
                                        Type = type,
                                        ReportType = reportType,
                                        StationType = stationType,
                                        RecvTime = DateTime.Now,
                                        Datas = datas
                                    };
                                    reportList.Add(report);
                                }

                                //水位5
                                int stationId5 = int.Parse(StationId) + 4;
                                string waterStr5 = msg.Substring(flagIndex + 2 + 8 * 4, 8);
                                if (waterStr5 != "-2000000")
                                {
                                    Decimal water5 = decimal.Parse(waterStr5) * (Decimal)0.01;
                                    if (water5 < 0 || water5 > 655)
                                    {
                                        water5 = 0;
                                    }
                                    List<CReportData> datas = new List<CReportData>();
                                    CReportData data = new CReportData();
                                    data.Rain = rain;
                                    data.Voltge = Voltage;
                                    data.Water = water5;
                                    data.Time = recvTime;
                                    datas.Add(data);
                                    report = new CReportStruct()
                                    {
                                        Stationid = stationId5.ToString(),
                                        Type = type,
                                        ReportType = reportType,
                                        StationType = stationType,
                                        RecvTime = DateTime.Now,
                                        Datas = datas
                                    };
                                    reportList.Add(report);
                                }

                                //水位6
                                int stationId6 = int.Parse(StationId) + 5;
                                string waterStr6 = msg.Substring(flagIndex + 2 + 8 * 5, 8);
                                if (waterStr6 != "-2000000")
                                {
                                    Decimal water6 = decimal.Parse(waterStr6) * (Decimal)0.01;
                                    if (water6 < 0 || water6 > 655)
                                    {
                                        water6 = 0;
                                    }
                                    List<CReportData> datas = new List<CReportData>();
                                    CReportData data = new CReportData();
                                    data.Rain = rain;
                                    data.Voltge = Voltage;
                                    data.Water = water6;
                                    data.Time = recvTime;
                                    datas.Add(data);
                                    report = new CReportStruct()
                                    {
                                        Stationid = stationId6.ToString(),
                                        Type = type,
                                        ReportType = reportType,
                                        StationType = stationType,
                                        RecvTime = DateTime.Now,
                                        Datas = datas
                                    };
                                    reportList.Add(report);
                                }

                                //水位7
                                int stationId7 = int.Parse(StationId) + 6;
                                string waterStr7 = msg.Substring(flagIndex + 2 + 8 * 6, 8);
                                if (waterStr7 != "-2000000")
                                {
                                    Decimal water7 = decimal.Parse(waterStr7) * (Decimal)0.01;
                                    if (water7 < 0 || water7 > 655)
                                    {
                                        water7 = 0;
                                    }
                                    List<CReportData> datas = new List<CReportData>();
                                    CReportData data = new CReportData();
                                    data.Rain = rain;
                                    data.Voltge = Voltage;
                                    data.Water = water7;
                                    data.Time = recvTime;
                                    datas.Add(data);
                                    report = new CReportStruct()
                                    {
                                        Stationid = stationId7.ToString(),
                                        Type = type,
                                        ReportType = reportType,
                                        StationType = stationType,
                                        RecvTime = DateTime.Now,
                                        Datas = datas
                                    };
                                    reportList.Add(report);
                                }

                                //水位8
                                int stationId8 = int.Parse(StationId) + 7;
                                string waterStr8 = msg.Substring(flagIndex + 2 + 8 * 7, 8);
                                if (waterStr8 != "-2000000")
                                {
                                    Decimal water8 = decimal.Parse(waterStr8) * (Decimal)0.01;
                                    if (water8 < 0 || water8 > 655)
                                    {
                                        water8 = 0;
                                    }
                                    List<CReportData> datas = new List<CReportData>();
                                    CReportData data = new CReportData();
                                    data.Rain = rain;
                                    data.Voltge = Voltage;
                                    data.Water = water8;
                                    data.Time = recvTime;
                                    datas.Add(data);
                                    report = new CReportStruct()
                                    {
                                        Stationid = stationId8.ToString(),
                                        Type = type,
                                        ReportType = reportType,
                                        StationType = stationType,
                                        RecvTime = DateTime.Now,
                                        Datas = datas
                                    };
                                    reportList.Add(report);
                                }
                            }
                        }
                        break;
                    default:
                        break;

                }

            }
            catch (Exception e)
            {

            }
            return true;
        }

        public bool Parse_1(string msg, out CReportStruct report)
        {
            throw new NotImplementedException();
        }

        public bool Parse_2(string msg, out CReportStruct report)
        {
            throw new NotImplementedException();
        }

        public bool Parse_beidou(string sid, EMessageType type, string msg, out CReportStruct upReport)
        {
            throw new NotImplementedException();
        }
    }
}