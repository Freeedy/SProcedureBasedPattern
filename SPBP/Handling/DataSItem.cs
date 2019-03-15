using System;
using System.Collections.Generic;
using System.Xml;

namespace SPBP.Handling
{
    public class DataSItem
    {
        private Dictionary<string, DataParam> _params = new Dictionary<string, DataParam>();

        private bool _hasReturnParam = false;
        
        private Dictionary<string, DataParam> _outputParams = new Dictionary<string, DataParam>();

        private string _name;
        private string _value;
        private string _schema; 


        #region  Properties
        public string Name { get { return _name; } set { _name = value;  } }
        public string Value { get { return _schema + '.' + _name; } }
        public string Schema { get { return _schema; } set { _schema = value;  } }
        public Dictionary<string, DataParam> Params
        {
            get { return _params; }
        }
        public string ConnectionString { get; set; }

        public bool HasReturnParam
        {
            get { return _hasReturnParam; }
        }
        public bool HasOutputParam { get { return _outputParams.Values.Count > 0; } }

        public Dictionary<string, DataParam> OutputParams { get { return _outputParams; } } 

        #endregion

        #region  Constructor
        public DataSItem()
        {

        }
        public DataSItem(XmlNode xmlItem)
        {
            ReadParamsFromXml(xmlItem);
        }
        #endregion

        #region  MEthods

        /*
         * <item name="" schema=""  constr="">
      <param name="" type="0" IsOutput="0"/>
    </item>*/
        public void ReadParamsFromXml(XmlNode xmlItem)
        {
            if (xmlItem != null && xmlItem.Name == "item")
            {
                _params.Clear();
                Name = xmlItem.Attributes["name"].Value;
              //  Value = xmlItem.Attributes["value"].Value;
                Schema = xmlItem.Attributes["schema"].Value;
                ConnectionString = xmlItem.Attributes["constr"].Value;

                if (xmlItem.HasChildNodes)
                {
                    foreach (XmlNode node in xmlItem.ChildNodes)
                    {
                        DataParam prm = new DataParam();
                        prm = DataParam.GetparamFromXmlNode(node);
                        prm.SetParent(this);

                        AddParam(prm);

                    }
                }

            }
        }

        public void AddParam(DataParam param)
        {

            if (_hasReturnParam && param.Direction == ParamDirection.Return)
            {

                throw new ReturnValueUniqueException();
            }

            if (!_hasReturnParam && param.Direction == ParamDirection.Return)
            {
                _hasReturnParam = true;
               
            }

            if (param.Direction == ParamDirection.Output)
            {
                _outputParams.Add(param.Name,param );
            }

            _params.Add(param.Name, param);
        }

        public void ClearParams()
        {
            _params.Clear();
        }

        public DataParam GetParamByName(string name)
        {
            DataParam param = null;
            foreach (DataParam prm in _params.Values)
            {
                if (prm.Name.Trim() == name.Trim())
                {
                    return prm;
                }
            }

            return param;
        }

        //<item name="Add_AccesList_table" schema="dbo"  constr="">
        public XmlNode ToXmlNode()
        {
            XmlNode result = null;

            if (DbCommandManagar.IsDocLoaded)
            {

                XmlElement element = DbCommandManagar.ConfigurationDocument.CreateElement("item");
                element.SetAttribute("name", Name);
                element.SetAttribute("schema", Schema);
                element.SetAttribute("constr", ConnectionString);
                foreach (DataParam param in Params.Values)
                {

                    element.AppendChild(param.ToXmlNode());
                }


                result = element;
            }

            return result;



        }

        public override string ToString()
        {
            return Value;
        }


        public List<DataParam> GetparamsByDirection(ParamDirection direction)
        {
            List<DataParam> result = new List<DataParam>();

            foreach (DataParam param in Params.Values)
            {
                if (param.Direction == direction)
                {
                    result.Add(param);

                }
            }


            return result;
        }

        public void AddReturnParam(CustomSqlTypes returntype )
        {
           AddParam(new DataParam("@return",returntype,ParamDirection.Return,"-1"));
        }

        #endregion

    }

    public class ReturnValueUniqueException : Exception
    {
        public ReturnValueUniqueException() : base("The  procedure has  more than  1 return value ! ") { }


    }

}
