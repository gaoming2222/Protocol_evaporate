/************************************************************************************
* Copyright (c) 2019 All Rights Reserved.
*命名空间：Protocol.Data.ZFXY
*文件名： UpParse
*创建人： XXX
*创建时间：2019-1-18 17:47:12
*描述
*=====================================================================
*修改标记
*修改时间：2019-1-18 17:47:12
*修改人：XXX
*描述：
************************************************************************************/
using Protocol.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;

namespace Protocol.Data.ZFXY
{
    public class UpParse : IUp
    {
        /// <summary>
        /// 解析上行报文数据
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool Parse(string msg, out CReportStruct report)
        {
            ///$12345678+140331080400+1257+0251+0078+990+PP
            report = null;
            try
            {
                string data = string.Empty;
                //去除$
                if (!ProtocolHelpers.DeleteSpecialChar(msg, out data))
                {
                    return false;
                }
                string[] dataList = data.Split('+');
                
                DateTime dataTime;
                string evpType = string.Empty;
                string stationId = string.Empty;
                List<CReportData> datas = new List<CReportData>();
                CReportData reportData = new CReportData();
                if (dataList.Length != 6 && dataList.Length != 7)
                {
                    return false;
                }
                
                for (int i = 0; i < dataList.Length; i++)
                {
                    if (i == 0)
                    {
                        if (dataList[i].Length == 8)
                        {
                            stationId = dataList[i];
                            continue;
                        }
                        else
                        {
                            System.Diagnostics.Debug.Write("数据：" + msg);
                            System.Diagnostics.Debug.Write("站号格式不对");
                            return false;
                        }
                        
                    }
                    if(i == 1)
                    {
                        if (dataList[i].Length == 12)
                        {
                            dataTime = new DateTime(
                                year: Int32.Parse("20" + dataList[i].Substring(0, 2)),
                                month: Int32.Parse(dataList[i].Substring(2, 2)),
                                day: Int32.Parse(dataList[i].Substring(4, 2)),
                                hour: Int32.Parse(dataList[i].Substring(6, 2)),
                                minute: Int32.Parse(dataList[i].Substring(8, 2)),
                                second: Int32.Parse(dataList[i].Substring(10, 2))
                            );
                            reportData.Time = dataTime;
                            continue;
                        }
                        else
                        {
                            System.Diagnostics.Debug.Write("数据：" + msg);
                            System.Diagnostics.Debug.Write("站号格式不对");
                            return false;
                        }
                    }
                    if (i == 2)
                    {
                        reportData.Voltge = Decimal.Parse(dataList[i]) * (Decimal)0.01;
                    }
                    if (i == 3)
                    {
                        reportData.Rain = Decimal.Parse(dataList[i]) * (Decimal)0.01;
                    }
                    if(i == 4)
                    {
                        reportData.Evp = Decimal.Parse(dataList[i]) * (Decimal)0.01;
                    }
                    if (i == 5)
                    {
                        reportData.Temperature = Decimal.Parse(dataList[i]) * (Decimal)0.01;
                    }
                    if(i == 6)
                    {
                        reportData.EvpType = dataList[i];
                    }
                }
                datas.Add(reportData);
                report = new CReportStruct()
                {
                    Stationid = stationId,
                    Type = string.Empty,
                    ReportType = EMessageType.Evp,
                    StationType = EStationType.EHydrology,
                    RecvTime = DateTime.Now,
                    Datas = datas
                };
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.Write("数据：" + msg);
                System.Diagnostics.Debug.Write("格式不对");
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