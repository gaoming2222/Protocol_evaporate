using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Hydrology.Entity;

namespace Protocol.Manager
{
    /// <summary>
    /// 站点数据序列化类
    /// </summary>
    public class XmlStationDataSerializer
    {
        #region 单件模式
        private static XmlStationDataSerializer m_sInstance;   //实例指针
        private XmlStationDataSerializer()
        {

        }
        public static XmlStationDataSerializer Instance
        {
            get { return GetInstance(); }
        }
        public static XmlStationDataSerializer GetInstance()
        {
            if (m_sInstance == null)
            {
                m_sInstance = new XmlStationDataSerializer();
            }
            return m_sInstance;
        }
        #endregion ///<单件模式

        private string m_path = "Config/StationData.xml";
        private CDictionary<string, CEntityStation> m_mapStationData;        //站点数据
        private CDictionary<string, string> m_mapStationbeidouID;        //站点ID与北斗ID
        private CDictionary<string, string> m_mapStationgprsID;        //站点ID与gprsID
        /// <summary>
        /// 序列化站点数据xml
        /// </summary>
        public void Serialize(List<CEntityStation> lists)
        {
            try
            {
                //return;
                // 判断Config文件夹是否存在
                if (!Directory.Exists("Config"))
                {
                    // 创建文件夹
                    Directory.CreateDirectory("Config");
                }
                var infos = new CEntityStationCollection()
                {
                    Items = lists
                };
                using (Stream fileStream = new FileStream(m_path, FileMode.Create, FileAccess.ReadWrite))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CEntityStationCollection));
                    serializer.Serialize(fileStream, infos);
                }
                System.Diagnostics.Debug.WriteLine(string.Format("写入站点数据表完成, 文件名：\"{0}\"", m_path));
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("写入站点数据表异常, 文件名：\"{0}\"\r\n{1}", m_path, exp.ToString()));
            }
        }
        /// <summary>
        /// 反序列化站点数据xml
        /// </summary>
        public List<CEntityStation> Deserialize()
        {
            try
            {
                //return new List<CEntityRealTime>();
                CEntityStationCollection result = null;
                if (Directory.Exists(m_path))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("找到文件, 文件名：\"{0}\"", m_path));
                }
                using (Stream fileStream = new FileStream(m_path, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CEntityStationCollection));
                    result = (CEntityStationCollection)serializer.Deserialize(fileStream);
                }
                if (result != null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("读取站点数据表完成, 文件名：\"{0}\"", m_path));
                    return result.Items;
                }
            }
            catch (Exception exp)
            {
               System.Diagnostics.Debug.WriteLine(string.Format("读取站点数据表异常, 文件名：\"{0}\"\r\n{1}", m_path, exp.ToString()));
            }
            return null;
        }

        /// <summary>
        /// 删除站点数据文件，如果遇到异常情况退出，删除，重新加载
        /// </summary>
        public void DeleteFile()
        {
            try
            {
                File.Delete(m_path);
                System.Diagnostics.Debug.WriteLine(string.Format("删除站点数据表文件\"{0}\"", m_path), false);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("删除站点数据表文件\"{0}\"异常\r\n{1}", m_path, ex.ToString()), false);
            }
        }

        public string GetStationByBDID(string BDId)
        {
            if(m_mapStationbeidouID == null)
            {
                ReadSIDXml();
            }

            string sid = m_mapStationbeidouID.FindKey(BDId);

            return sid;
        }

        public string GetStationByGprsID(string gprs)
        {
            if (m_mapStationgprsID == null)
            {
                ReadSIDXml();
            }

            string sid = m_mapStationgprsID.FindKey(gprs);

            return sid;
        }

        private void ReadSDXml()
        {
            m_mapStationData = new CDictionary<string, CEntityStation>();
            // 读取XML,初始化站点数据表
            List<CEntityStation> listSD = Deserialize();
            if (null == listSD)
            {
                return;
            }
            for (int i = 0; i < listSD.Count; ++i)
            {
                if (!m_mapStationData.ContainsKey(listSD[i].StationID))
                {
                    // 通知界面
                    m_mapStationData.Add(listSD[i].StationID, listSD[i]);
                }
                else
                {
                    // 位置站点，读取站点数据文件不匹配
                    System.Diagnostics.Debug.WriteLine(string.Format("站点数据中站点\"{0}\"已存在", listSD[i].StationID));
                }
            }
        }

        /// <summary>
        /// 创建站号与北斗卫星号的对应关系
        /// </summary>
        private void ReadSIDXml()
        {
            m_mapStationbeidouID = new CDictionary<string, string>();
            m_mapStationgprsID = new CDictionary<string, string>();
            // 读取XML,初始化站点数据表
            List<CEntityStation> listSD = Deserialize();
            if (null == listSD)
            {
                return;
            }
            for (int i = 0; i < listSD.Count; ++i)
            {
                if (!m_mapStationbeidouID.ContainsKey(listSD[i].StationID))
                {
                    // 通知界面
                    m_mapStationbeidouID.Add(listSD[i].StationID, listSD[i].BDSatellite);
                    m_mapStationgprsID.Add(listSD[i].StationID, listSD[i].GPRS);
                }
                else
                {
                    // 位置站点，读取站点数据文件不匹配
                    System.Diagnostics.Debug.WriteLine(string.Format("站点数据中站点\"{0}\"已存在", listSD[i].StationID));
                }
            }
        }
    }
}
