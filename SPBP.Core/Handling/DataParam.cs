using System;
using System.Xml;

namespace SPBP.Handling
{
    public class DataParam
    {
        private string _value = "";


        #region Properties
        public string Name { get; set; }
        public CustomSqlTypes Type { get; set; }
        public ParamDirection Direction { get; set; }
        public string Default { get; set; }
        public DataSItem Parent { get; private set; }
        public string Value
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_value))
                {
                    return Default;
                }
                return _value;

            }
            set { _value = value; }
        }
        #endregion

        #region  Constructor
        public DataParam(string name, CustomSqlTypes type,ParamDirection direction , string defval)
        {
            Name = name;
            Type = type;
            Default = defval;
            Direction = direction;
        }
        public DataParam(string name, CustomSqlTypes type,ParamDirection direction )
            : this(name, type, direction ,string.Empty)
        {

        }
        public DataParam(string name ,CustomSqlTypes type ):this (name ,type,ParamDirection.Input){}

        public DataParam()
            : this(string.Empty, CustomSqlTypes.String,ParamDirection.Input, string.Empty)
        {

        }

        #endregion

        #region  Methods
        public void SetParent(DataSItem parent)
        {
            Parent = parent;
        }


        //<param name="" type="0" IsOutput="0"/>
        public static DataParam GetparamFromXmlNode(XmlNode node)
        {
            DataParam res = new DataParam();
            res.Name = node.Attributes["name"].Value;
            int type = Convert.ToInt16(node.Attributes["type"].Value);
            res.Type = (CustomSqlTypes)type;
            int a = Convert.ToInt32(node.Attributes["direction"].Value);
            res.Direction = (ParamDirection) a;
            res.Default = node.Attributes["default"].Value;
            return res;

        }

        public override string ToString()
        {
            return Name;
        }

        public XmlNode ToXmlNode()
        {
            XmlNode result = null;
            if (DbCommandManagar.IsDocLoaded)
            {
                XmlElement element = DbCommandManagar.ConfigurationDocument.CreateElement("param");
                element.SetAttribute("name", Name);
                element.SetAttribute("type", ((int)Type).ToString());
                element.SetAttribute("direction", ((int)Direction).ToString());
                element.SetAttribute("default", Default);


                result = element;
            }

            return result;
        }

        #endregion
    }
}
