using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SPBP.Handling
{
    public  class ProcedureFactory
    {
        private const string Name = "Procedures";

        private Dictionary<string, DataSItem> _procedures = new Dictionary<string, DataSItem>();
        public Dictionary<string, DataSItem> Procedures { get { return _procedures; } }

        public bool HasProcedures { get { return _procedures.Values.Count > 0; } }


        public void SetProcedures(Dictionary<string, DataSItem> procedures)
        {
            _procedures = procedures;
        }

        public XmlNode ToXmlNode()
        {
            XmlNode result = null;

            if (DbCommandManagar.IsDocLoaded)
            {

                XmlElement element = DbCommandManagar.ConfigurationDocument.CreateElement(Name);
                foreach (DataSItem param in _procedures.Values)
                {
                    element.AppendChild(param.ToXmlNode());
                }


                result = element;
            }

            return result;



        }


        public static XmlNode XmlTemplate()
        {
            XmlNode result = null; 

            if (DbCommandManagar.IsDocLoaded)
            {
                XmlElement element = DbCommandManagar.ConfigurationDocument.CreateElement(Name);

                result = element;
            }


            return result;
        }





    }
}
