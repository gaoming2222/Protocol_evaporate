using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protocol.Data.Interface;
using Hydrology.Entity;

namespace Protocol.Data.ZYJBX
{
    public class FlashBatchTrans : IFlashBatch
    {

        public string BuildQuery(string sid, EStationType stationType, ETrans trans, DateTime beginTime, DateTime endTime, EChannelType ctype)
        {
            throw new NotImplementedException();
        }

        public bool Parse(string data, out CBatchStruct batch)
        {
            throw new NotImplementedException();
        }
    }
}
