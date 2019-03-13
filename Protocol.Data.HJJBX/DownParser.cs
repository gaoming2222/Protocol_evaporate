using Hydrology.Entity;
using Protocol.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Protocol.Data.HJJBX
{
    public class DownParser
    {

        //  数据下行读取
        public String BuildQuery(string sid, IList<EDownParam> cmds, EChannelType ctype)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ProtocolMaps.ChannelProtocolStartCharMap.FindValue(ctype));   //添加首字符
            sb.Append(String.Format("{0:D4}", Int32.Parse(sid.Trim())));            //添加四位站点ID号
            sb.Append("0G");                                                         //添加指令类型

            int length = 6; //  查询后指令的长度
            foreach (var cmd in cmds)
            {
                length = length + 3 + Int32.Parse(ProtocolMaps.DownParamLengthMap[cmd]);

                sb.Append(CSpecialChars.BALNK_CHAR);
                sb.Append(ProtocolMaps.DownParamMap.FindValue(cmd));
            }
            sb.Append(CSpecialChars.ENTER_CHAR);    //  添加结束符

            if (ctype == EChannelType.None)
            {
                //  短信一包最大字节数为160
                return length > 160 ? string.Empty : sb.ToString();
            }
            else if (ctype == EChannelType.BeiDou)
            {
                //  北斗卫星最大字节数为98
                return length > 98 ? string.Empty : sb.ToString();
            }
            else if (ctype == EChannelType.GPRS || ctype == EChannelType.GSM)
            {
                return sb.ToString();
            }
            throw new Exception("信道协议未编写！");
        }

        //  数据下行设置
        public String BuildSet(string sid, IList<EDownParam> cmds, CDownConf down, EChannelType ctype)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ProtocolMaps.ChannelProtocolStartCharMap.FindValue(ctype));   //添加首字符
            sb.Append(String.Format("{0:D4}", Int32.Parse(sid.Trim())));            //添加四位站点ID号
            sb.Append("0S");                                                         //添加指令类型

            int length = 6; //  查询后指令的长度
            foreach (var cmd in cmds)
            {
                length = length + 3 + Int32.Parse(ProtocolMaps.DownParamLengthMap[cmd]);

                sb.Append(CSpecialChars.BALNK_CHAR);
                sb.Append(ProtocolMaps.DownParamMap.FindValue(cmd));
                switch (cmd)
                {
                    case EDownParam.Clock: sb.Append(down.Clock.Value.ToString("yyMMddHHmmss")); break;
                    case EDownParam.NormalState: sb.Append(ProtocolMaps.NormalState4ProtoMap.FindValue(down.NormalState.Value)); break;
                    case EDownParam.Voltage: sb.Append(String.Format("{0:D4}", (int)down.Voltage.Value)); break;
                    case EDownParam.StationCmdID: sb.Append(down.StationCmdID); break;
                    case EDownParam.TimeChoice: sb.Append(ProtocolMaps.TimeChoice4ProtoMap.FindValue(down.TimeChoice.Value)); break;
                    case EDownParam.TimePeriod: sb.Append(ProtocolMaps.TimePeriodMap.FindValue(down.TimePeriod.Value)); break;
                    case EDownParam.WorkStatus: sb.Append(ProtocolMaps.WorkStatus4ProtoMap.FindValue(down.WorkStatus.Value)); break;
                    case EDownParam.VersionNum: sb.Append(down.VersionNum); break;
                    case EDownParam.StandbyChannel:
                        sb.Append(ProtocolMaps.ChannelType4ProtoMap.FindValue(down.MainChannel.Value));
                        sb.Append(ProtocolMaps.ChannelType4ProtoMap.FindValue(down.ViceChannel.Value));
                        break;
                    case EDownParam.TeleNum: sb.Append(down.TeleNum); break;
                    case EDownParam.RingsNum: sb.Append(String.Format("{0:D2}", (int)down.RingsNum.Value)); break;
                    case EDownParam.DestPhoneNum: sb.Append(down.DestPhoneNum); break;
                    case EDownParam.TerminalNum: sb.Append(down.TerminalNum); break;
                    case EDownParam.RespBeam: sb.Append(down.RespBeam); break;
                    case EDownParam.AvegTime: sb.Append(String.Format("{0:D2}", (int)down.AvegTime.Value)); break;
                    case EDownParam.KC: sb.Append(down.KC); break;
                    case EDownParam.RainPlusReportedValue: sb.Append(String.Format("{0:D2}", (int)down.RainPlusReportedValue.Value)); break;
                    case EDownParam.Rain: sb.Append(String.Format("{0:D4}", (int)down.Rain.Value)); break;
                    case EDownParam.Water: sb.Append(String.Format("{0:D6}", (int)down.Water.Value)); break;
                    case EDownParam.WaterPlusReportedValue: sb.Append(String.Format("{0:D2}", (int)down.WaterPlusReportedValue.Value)); break;
                    case EDownParam.SelectCollectionParagraphs: sb.Append(ProtocolMaps.SelectCollectionParagraphs4ProtoMap.FindValue(down.SelectCollectionParagraphs.Value)); break;
                    case EDownParam.StationType: sb.Append(ProtocolHelpers.StationType2ProtoStr_set(down.StationType.Value)); break;
                    case EDownParam.UserName: sb.Append(down.UserName); break;
                    case EDownParam.StationName: sb.Append(down.StationName); break;
                    default:
                        throw new Exception("设置下行指令参数错误");
                }
            }
            sb.Append(CSpecialChars.ENTER_CHAR);    //添加结束符

            if (ctype == EChannelType.None)
            {
                //  短信一包最大字节数为160
                return length > 160 ? string.Empty : sb.ToString();
            }
            else if (ctype == EChannelType.BeiDou)
            {
                //  北斗卫星最大字节数为98
                return length > 98 ? string.Empty : sb.ToString();
            }
            else if (ctype == EChannelType.GPRS || ctype == EChannelType.GSM)
            {
                return sb.ToString();
            }
            throw new Exception("信道协议未编写！");
        }

        //  数据下行解析
        public bool Parse(string msg, out CDownConf downConf)
        {
            downConf = new CDownConf();
            int mgb = 0;
            try
            {
                string data = string.Empty;
                if (!ProtocolHelpers.DeleteSpecialChar(msg, out data))
                    return false;

                //  解析站点ID
                String stationID = data.Substring(0, 4);
                //  解析通信类别
                String type = data.Substring(4, 2);
                data = data.Substring(6);
                var segs = data.Split(CSpecialChars.BALNK_CHAR);
                foreach (var item in segs)
                {
                    try
                    {
                        //  数据分为两部分
                        //  2 Byte 指令  +  剩下的为数据，数据的长度>= 2
                        //  解析指令类型param
                        EDownParam param = ProtocolMaps.DownParamMap.FindKey(item.Substring(0, 2));
                        //  如果接收到的数据段长度大于2，表示对应的字段有值
                        //  默认为String.Empty
                        string info = string.Empty;
                        if (item.Length > 2)
                        {
                            int length = Int32.Parse(ProtocolMaps.DownParamLengthMap.FindValue(param));
                            //  如果接收到的数据段的长度不等于规定长度,或者规定长度为-1（即长度不定）
                            //        截取剩下的所有字符串
                            //  否则，截取规定长度的字符串
                            info = (item.Length - 2 != length || length == -1) ? item.Substring(2) : item.Substring(2, length);
                        }
                        if (String.IsNullOrEmpty(info))
                        {
                            continue;
                        }

                        switch (param)
                        {
                            case EDownParam.Clock:
                                int year = Int32.Parse("20" + info.Substring(0, 2));
                                int month = Int32.Parse(info.Substring(2, 2));
                                int day = Int32.Parse(info.Substring(4, 2));
                                int hour = Int32.Parse(info.Substring(6, 2));
                                int minute = Int32.Parse(info.Substring(8, 2));
                                int second = Int32.Parse(info.Substring(10, 2));
                                downConf.Clock = new DateTime(year, month, day, hour, minute, second);
                                break;
                            case EDownParam.NormalState: downConf.NormalState = ProtocolMaps.NormalState4ProtoMap.FindKey(info); break;
                            case EDownParam.Voltage: downConf.Voltage = (Decimal.Parse(info) * (Decimal)0.01); break;
                            case EDownParam.StationCmdID: downConf.StationCmdID = info; break;
                            case EDownParam.TimeChoice: downConf.TimeChoice = ProtocolMaps.TimeChoice4ProtoMap.FindKey(info == "01" ? info : "02"); break;
                            case EDownParam.TimePeriod: downConf.TimePeriod = ProtocolMaps.TimePeriodMap.FindKey(info); break;
                            case EDownParam.WorkStatus: downConf.WorkStatus = ProtocolMaps.WorkStatus4ProtoMap.FindKey(info); break;
                            case EDownParam.VersionNum: downConf.VersionNum = info; break;
                            case EDownParam.StandbyChannel:
                                downConf.MainChannel = ProtocolMaps.ChannelType4ProtoMap.FindKey(info.Substring(0, 2));
                                System.Diagnostics.Debug.Assert(downConf.MainChannel != EChannelType.None, "主用信道不能为NONE");
                                downConf.ViceChannel = ProtocolMaps.ChannelType4ProtoMap.FindKey(info.Substring(2, 2));
                                break;
                            case EDownParam.TeleNum: downConf.TeleNum = info; break;
                            case EDownParam.RingsNum: downConf.RingsNum = Decimal.Parse(info); break;
                            case EDownParam.DestPhoneNum: downConf.DestPhoneNum = info; break;
                            case EDownParam.TerminalNum: downConf.TerminalNum = info; break;
                            case EDownParam.RespBeam: downConf.RespBeam = info; break;
                            case EDownParam.AvegTime: downConf.AvegTime = Decimal.Parse(info); break;
                            case EDownParam.RainPlusReportedValue: downConf.RainPlusReportedValue = Decimal.Parse(info); break;
                            case EDownParam.KC: downConf.KC = info; break;
                            case EDownParam.Rain: downConf.Rain = Decimal.Parse(info); break;
                            //  单位为米    case EDownParam.Water: downConf.Water = (Decimal.Parse(info) * (Decimal)0.01); break;
                            //  默认单位为厘米
                            case EDownParam.Water: downConf.Water = Decimal.Parse(info); break;
                            case EDownParam.WaterPlusReportedValue: downConf.WaterPlusReportedValue = Decimal.Parse(info); break;
                            case EDownParam.SelectCollectionParagraphs: downConf.SelectCollectionParagraphs = ProtocolMaps.SelectCollectionParagraphs4ProtoMap.FindKey(info); break;
                            case EDownParam.StationType:
                                var stype = ProtocolHelpers.ProtoStr2StationType(info);
                                downConf.StationType = stype; break;
                            case EDownParam.UserName:
                                downConf.UserName = info;
                                break;
                            case EDownParam.StationName:
                                downConf.StationName = info;
                                break;
                            default:
                                mgb--;
                                break;
                        }
                        mgb++;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("数据:" + msg);
                        System.Diagnostics.Debug.WriteLine("下行指令解析数据不完整！" + e.Message);
                    }
                }
            }
            catch (Exception exp)
            {
                downConf = null;
                System.Diagnostics.Debug.WriteLine(exp.Message);
            }
            if (mgb == 0)
            {
                downConf = null;
                return false;
            }
            return true;
        }

        //  批量数据UBatch下行
        public String BuildQuery_UBatch(string sid, EStationType stationType, ETrans trans, DateTime beginTime, EChannelType ctype)
        {

            //  构建发送指令的字符串
            StringBuilder sb = new StringBuilder();
            sb.Append(ProtocolMaps.ChannelProtocolStartCharMap.FindValue(ctype));
            sb.Append(String.Format("{0:D4}", Int32.Parse(sid.Trim())));
            sb.Append("0K");

            //   stationtype :  01为水位 02为雨量        
            sb.Append(ProtocolHelpers.StationType2ProtoStr_1(stationType));

            //  时间
            sb.Append(beginTime.ToString("yyMMddHH"));

            sb.Append('\r');
            return sb.ToString();
        }

        //  批量数据UBatch下行解析
        public bool Parse(String msg, out CBatchStruct batch)
        {
            batch = new CBatchStruct();
            try
            {
                string data = string.Empty;
                if (!ProtocolHelpers.DeleteSpecialChar(msg, out data))
                    return false;

                //  解析站点ID ， 4位     0001
                batch.StationID = data.Substring(0, 4);
                //  解析命令指令 ，2位     1K 
                batch.Cmd = data.Substring(4, 2);
                //  解析站点类型， 2位     01 
                batch.StationType = ProtocolHelpers.ProtoStr2StationType(data.Substring(6, 2));
                int DataYear = Int32.Parse(data.Substring(8,2));
                int DataMonth = Int32.Parse(data.Substring(10,2));
                int DataDay = Int32.Parse(data.Substring(12,2));
                //创建返回的数据对象
                var datas = new List<CTimeAndData>();
                var lists = data.Substring(14).Split(CSpecialChars.BALNK_CHAR);

                foreach (var item in lists)
                {
                    string itemData = item;
                    while(itemData.Length > 0)
                    {
                        int DataHour = Int32.Parse(itemData.Substring(0, 2));
                        int DataMinute = Int32.Parse(itemData.Substring(2, 2));
                        //  解析出数据时间
                        DateTime recvTime = new DateTime(
                            year    :   DataYear,
                            month   :   DataMonth,
                            day     :   DataDay,
                            hour    :   DataHour,
                            minute  :   DataMinute,
                            second  :   0
                            );
                        //  解析出16进制的数据
                        string itemDatas = itemData.Substring(4, 4);
                        //  将16进制数据转化为10进制
                        itemDatas = int.Parse(itemDatas, System.Globalization.NumberStyles.AllowHexSpecifier).ToString();

                        //  创建CTimeAndData对象
                        CTimeAndData result = new CTimeAndData();
                        result.Time = recvTime;
                        result.Data = itemDatas;
                        //  将对像填充至list中
                        datas.Add(result);

                        //构成循环
                        itemData = itemData.Substring(10);
                    }
                }
                
                batch.Datas = datas;
                return true;
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine("数据：" + msg);
                System.Diagnostics.Debug.WriteLine("批量数据解析不完整" + exp.Message);
            }
            return false;
        }

        public string BuildQuery_Batch(string sid, ETrans trans, DateTime beginTime, EChannelType ctype)
        {
            throw new NotImplementedException();
        }
    }
}
