using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protocol.Data.Interface;

namespace Protocol.Data.ZYJBX
{
    class SoilParser : ISoil
    {
        public string BuildQuery()
        {
            throw new NotImplementedException();
        }

        public bool Parse(string resp, out Hydrology.Entity.CEntitySoilData soil, out Hydrology.Entity.CReportStruct report)
        {
            throw new NotImplementedException();
        }
    }
}
