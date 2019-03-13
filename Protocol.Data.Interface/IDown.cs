using System;
using System.Collections.Generic;
using Hydrology.Entity;

namespace Protocol.Data.Interface
{
    /// <summary>
    /// 远地下行指令接口
    /// </summary>
    public interface IDown
    {
        /// <summary>
        /// 查询下行指令
        /// </summary>
        String BuildQuery(string sid, IList<EDownParam> cmds, EChannelType ctype);

        /// <summary>
        /// 批量查询board下行指令
        /// </summary>
        String BuildQuery_Batch(string sid, ETrans trans, DateTime beginTime, EChannelType ctype);
        /// <summary>
        /// 批量查询flash下行指令
        /// </summary>
        String BuildQuery_Flash(string sid, EStationType stationType, ETrans trans, DateTime beginTime, DateTime endTime, EChannelType ctype);

        /// <summary>
        /// 批量查询sd下行指令
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="beginTime"></param>
        /// <param name="ctype"></param>
        /// <returns></returns>
        String BuildQuery_SD(string sid, DateTime beginTime, EChannelType ctype);


        /// <summary>
        /// 设置命令
        /// </summary>
        String BuildSet(string sid, IList<EDownParam> cmds, CDownConf down, EChannelType ctype);

        /// <summary>
        /// 解析查询后的数据
        /// </summary>
        bool Parse(string resp, out CDownConf downConf);

        /// <summary>
        /// 解析批量flash的数据
        /// </summary>
        bool Parse_Flash(String msg, EChannelType ctype, out CBatchStruct batch);

        /// <summary>
        /// 解析批量主板的数据
        /// </summary>
        bool Parse_Batch(String msg, out CBatchStruct batch);

        bool Parse_SD(string msg,string id, out CSDStruct sd);
    }
}
