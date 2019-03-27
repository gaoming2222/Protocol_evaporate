/************************************************************************************
* Copyright (c) 2019 All Rights Reserved.
*命名空间：Protocol.Data.ZFXY
*文件名： DownParse
*创建人： XXX
*创建时间：2019-1-18 17:53:55
*描述
*=====================================================================
*修改标记
*修改时间：2019-1-18 17:53:55
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
    public class DownParse : IDown
    {
        public String BuildSet(string sid, IList<EDownParamEV> cmds, CDownConfEV down, EChannelType ctype)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("at+");//AT指令头
            foreach (var cmd in cmds)
            {
                switch (cmd)
                {
                    case EDownParamEV.Clock://时钟命令
                        sb.Append("cclk=\"");
                        sb.Append(down.Date);
                        sb.Append("\"");
                        break;

                    case EDownParamEV.TelephoneNum://目的手机卡号命令
                        sb.Append("cpbw=1,\"");
                        sb.Append(down.TelephoneNumD);
                        sb.Append("\",129,\"DestID\"");
                        break;

                    case EDownParamEV.ID://ID号命令
                        sb.Append("cpbw=2,\"");
                        sb.Append(down.ID);
                        sb.Append("\",129,\"ID\"");
                        break;

                    case EDownParamEV.HeightLimit://液位限制命令
                        sb.Append("cpbw=3,\"");
                        sb.Append(down.HeightLimit);
                        sb.Append("\",129,\"Hight\"");
                        break;
                }
            }
            return sb.ToString();
        }
        public string BuildQuery(string sid, IList<EDownParam> cmds, EChannelType ctype)
        {
            throw new NotImplementedException();
        }

        public string BuildQuery_Batch(string sid, ETrans trans, DateTime beginTime, EChannelType ctype)
        {
            throw new NotImplementedException();
        }

        public string BuildQuery_Flash(string sid, EStationType stationType, ETrans trans, DateTime beginTime, DateTime endTime, EChannelType ctype)
        {
            throw new NotImplementedException();
        }

        public string BuildQuery_SD(string sid, DateTime beginTime, EChannelType ctype)
        {
            throw new NotImplementedException();
        }

        public string BuildSet(string sid, IList<EDownParam> cmds, CDownConf down, EChannelType ctype)
        {
            throw new NotImplementedException();
        }

        public bool Parse(string resp, out CDownConf downConf)
        {
            throw new NotImplementedException();
        }

        public bool Parse_Batch(string msg, out CBatchStruct batch)
        {
            throw new NotImplementedException();
        }

        public bool Parse_Flash(string msg, EChannelType ctype, out CBatchStruct batch)
        {
            throw new NotImplementedException();
        }

        public bool Parse_SD(string msg, string id, out CSDStruct sd)
        {
            throw new NotImplementedException();
        }
    }
}