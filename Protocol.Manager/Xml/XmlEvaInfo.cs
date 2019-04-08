using Hydrology.Entity.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Protocol.Manager
{
    public class XMLEvaInfo
    {
        private static string m_path = "Config/EvaSet.xml";
        private static string m_path_1 = "Config/ShowForm.xml";

        private static XMLEvaInfo instance;
        public static XMLEvaInfo Instance
        {
            get
            {
                if (instance == null)
                    instance = new XMLEvaInfo();
                return instance;
            }
        }

        public void Serialize(decimal kp, decimal ke, decimal dh, bool comP)
        {
            try
            {
                //获取根节点对象
                XDocument document = new XDocument();
                XElement root = new XElement("Eva");
                XElement ele = new XElement("Conf");
                ele.SetElementValue("kp", kp);
                ele.SetElementValue("ke", ke);
                ele.SetElementValue("dh", dh);
                ele.SetElementValue("comP", comP.ToString());
                root.Add(ele);
                root.Save(m_path);
            }
            catch (Exception e)
            {
                Debug.WriteLine("保存失败！");
            }
        }

        public void Serialize_ShowForm(bool bShow)
        {
            try
            {
                //获取根节点对象
                XDocument document = new XDocument();
                XElement root = new XElement("Eva");
                XElement ele = new XElement("Conf");
                ele.SetElementValue("bShow", bShow.ToString());
                root.Add(ele);
                root.Save(m_path_1);
            }
            catch (Exception e)
            {
                Debug.WriteLine("保存失败！");
            }
        }

        public Dictionary<string, string> DeSerialize()
        {
            Dictionary<string,string> evaConf = new Dictionary<string, string>();
            decimal kp = 1.000m;
            decimal ke = 1.000m;
            decimal dh = 0.000m;
            bool comP = false;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(m_path);
                XmlNode xn = doc.SelectSingleNode("Eva");
                XmlNodeList xnl = xn.ChildNodes;
                kp = decimal.Parse(xnl.Item(0).InnerText);
                ke = decimal.Parse(xnl.Item(1).InnerText);
                dh = decimal.Parse(xnl.Item(2).InnerText);
                comP = bool.Parse(xnl.Item(3).InnerText);
            }
            catch (Exception e)
            {
                Debug.WriteLine("读取蒸发参数XML文件失败！");
            }
            evaConf.Add("kp", kp.ToString());
            evaConf.Add("ke", ke.ToString());
            evaConf.Add("dh", dh.ToString());
            evaConf.Add("comP", comP.ToString());
            return evaConf;
        }

        public bool DeSerialize_FormShow()
        {
            bool bShow = true;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(m_path_1);
                XmlNode xn = doc.SelectSingleNode("Eva");
                XmlNodeList xnl = xn.ChildNodes;
                bShow = bool.Parse(xnl.Item(0).InnerText);
            }
            catch (Exception e)
            {
                Debug.WriteLine("读取界面显示参数XML文件失败！");
            }
            return bShow;
        }

    }
}
