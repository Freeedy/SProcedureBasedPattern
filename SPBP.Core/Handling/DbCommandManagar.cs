using System;
using System.Collections.Generic;
using System.Xml;
using SPBP.Modules.SQl;

namespace SPBP.Handling
{
    public static class DbCommandManagar
    {
        #region Fields

        //private static Dictionary<string, DataSItem> _procedures = new Dictionary<string, DataSItem>(); //list of  stroder procedures 

        //private static Dictionary<string, DataSItem> _views = new Dictionary<string, DataSItem>(); //optional  

        private static ProcedureFactory _factory = new ProcedureFactory();

        private static string _conf = string.Empty;
        private static bool _confIssetted = false;
        public static bool _isDocLoaded = false;
        private static XmlDocument _doc = new XmlDocument();

        #endregion


        #region Properties

        //public static Dictionary<string, DataSItem> Procedures { get { return _procedures; } }
        //public static Dictionary<string, DataSItem> Views { get { return _views; } }


        public static ProcedureFactory Factory { get { return _factory; } }
        public static string ConfigurationFilePath { get { return _conf; } }    // configuration file path 
        public static bool ConfIsSetted { get { return _confIssetted; } }       //State of conf 
        public static XmlDocument ConfigurationDocument { get { return _doc; } }//loaded configuration document 
        public static bool IsDocLoaded { get { return _isDocLoaded; } }         // is that document loaded?! 

        #endregion

        #region Methods

        #region PrivateMethods
        private static bool Save()
        {

            bool result = false;
            try
            {

                _doc.Save(_conf);

                result = true;
            }
            catch
            {
                result = false;
            }


            return result;
        }
        #endregion
        // set configuration path  1. stage 
        public static void SetConfiguration(string path)
        {
            _conf = path;
            _confIssetted = true;
        }

        // load configuration  document  2 . stage 
        public static void LoadDocument()
        {
            string ext = _conf.Substring(_conf.Length - 3);
            if (_confIssetted && ext == "xml")
            {
                _doc.Load(_conf);
                _isDocLoaded = true;
            }
        }

        // initiate manager 
        public static void Initialize()
        {
            LoadDocument();
            ReadAllProcedures();
            // ReadAllViews();
        }


        public static void RefreshProcedures()
        {
            if (IsDocLoaded)
            {
                ReadAllProcedures();
            }
        }
        public static void RefreshViews()
        {
            if (IsDocLoaded)
            {
                ReadAllViews();
            }
        }
        public static bool ReadConfiguration()
        {
            return ReadAllProcedures() && ReadAllViews();
        }

        public static bool ReadAllProcedures()
        {
            bool res = false;
            if (_isDocLoaded)
            {
                try
                {
                    XmlNodeList nodelist = _doc.SelectNodes(SettingsHelperManager.QueryOfListingAllProcedures());

                    if (nodelist != null)
                    {
                        _factory.Procedures.Clear();
                        int asd = 0;
                        foreach (XmlNode node in nodelist)
                        {
                            asd++;
                            DataSItem item = new DataSItem(node);
                            _factory.Procedures.Add(item.Value, item);
                        }


                        res = true;
                    }


                }
                catch
                {
                    return false;
                }
            }

            return res;
        }
        public static bool ReadAllViews()
        {
            bool res = false;
            if (_isDocLoaded)
            {
                try
                {
                    //XmlNodeList nodelist = _doc.SelectNodes(SettingsHelperManager.QueryOfListingAllViews());

                    //if (nodelist != null)
                    //{
                    //    _factory.Procedures.Clear(); //views 
                    //    foreach (XmlNode node in nodelist)
                    //    {
                    //        DataSItem view = new DataSItem(node);
                    //        _factory.Procedures.Add(view.Value, view);

                    //    }


                    //    res = true;
                    //}

                }
                catch
                {
                    return false;
                }
            }

            return res;
        }

        public static bool AddFactory(ProcedureFactory factory)
        {
            bool res = false;

            if (IsDocLoaded)
            {
                try
                {


                    XmlNode procParent = _doc.DocumentElement;

                    if (procParent != null)
                    {
                        procParent.AppendChild(factory.ToXmlNode());


                        if (Save())
                        {
                            res = true;
                            ReadAllProcedures();

                        }
                    }
                }
                catch
                {
                    return false;
                }
            }



            return res;
        }

        public static bool AddProcedure(DataSItem procedure)
        {
            bool res = false;

            if (IsDocLoaded)
            {
                try
                {


                    XmlNode procParent = _doc.DocumentElement.ChildNodes[0];

                    if (procParent != null)
                    {
                        procParent.AppendChild(procedure.ToXmlNode());


                        if (Save())
                        {
                            res = true;
                            ReadAllProcedures();

                        }
                    }
                }
                catch
                {
                    return false;
                }
            }



            return res;
        }

        public static bool DeleteProcedure(string procedureValue)
        {
            bool res = false;

            if (IsDocLoaded)
            {
                try
                {


                    XmlNode node =
                        ConfigurationDocument.SelectSingleNode(
                            SettingsHelperManager.QueryOfSelectinProcedureByValue(procedureValue));

                    if (node == null)
                    {
                        return false;
                    }

                    node.ParentNode.RemoveChild(node);

                    if (Save())
                    {
                        res = true;
                        ReadAllProcedures();

                    }


                }
                catch
                {

                }
            }
            return res;

        }

        public static bool UpdateProcedure(string oldProcedureVal, DataSItem newProc)
        {
            bool res = false;


            if (IsDocLoaded)
            {
                try
                {
                    string test = SettingsHelperManager.QueryOfSelectinProcedureByValue(oldProcedureVal);
                    XmlNode node =
                        ConfigurationDocument.SelectSingleNode(test);
                    if (node == null)
                    {
                        return false;
                    }

                    node.Attributes["name"].Value = newProc.Name;
                    node.Attributes["value"].Value = newProc.Value;
                    node.Attributes["constr"].Value = newProc.ConnectionString;


                    if (Save())
                    {
                        res = true;
                        ReadAllProcedures();
                    }




                }
                catch
                {
                    return false;
                }
            }

            return res;


        }

        //add param 
        public static bool AddParam(DataSItem parent, DataParam newparam)
        {
            bool res = false;
            if (IsDocLoaded)
            {
                try
                {


                    XmlNode procParent =
                        _doc.SelectSingleNode(SettingsHelperManager.QueryOfSelectinProcedureByValue(parent.Value));

                    if (procParent != null)
                    {
                        procParent.AppendChild(newparam.ToXmlNode());


                        if (Save())
                        {
                            res = true;
                            ReadAllProcedures();

                        }
                    }
                }
                catch
                {
                    return false;
                }
            }

            return res;
        }

        //delete param 
        public static bool DeleteParam(DataSItem parent, DataParam delParam)
        {
            bool res = false;

            if (IsDocLoaded)
            {
                try
                {


                    XmlNode node =
                        ConfigurationDocument.SelectSingleNode(
                            SettingsHelperManager.QueryOfSelectingParametrOfTheProcedureByPval(parent.Value, delParam.Name));

                    if (node == null)
                    {
                        return false;
                    }

                    node.ParentNode.RemoveChild(node);

                    if (Save())
                    {
                        res = true;
                        ReadAllProcedures();

                    }


                }
                catch
                {

                }
            }
            return res;
        }

        //update param       
        public static bool UpdateParam(DataSItem parent, string oldParamname, DataParam unewparam)
        {
            bool res = false;

            if (IsDocLoaded)
            {
                try
                {
                    string test = SettingsHelperManager.QueryOfSelectingParametrOfTheProcedureByPval(parent.Value, oldParamname);
                    XmlNode node =
                        ConfigurationDocument.SelectSingleNode(test);
                    if (node == null)
                    {
                        return false;
                    }
                    //<param name="@action_name" type="4" IsOutput="True" default="asdasd" />
                    node.Attributes["name"].Value = unewparam.Name;
                    node.Attributes["type"].Value = ((int)unewparam.Type).ToString();
                    node.Attributes["direction"].Value = ((int)unewparam.Direction).ToString();
                    node.Attributes["default"].Value = unewparam.Default;


                    if (Save())
                    {
                        res = true;
                        ReadAllProcedures();
                    }




                }
                catch
                {
                    return false;
                }
            }

            return res;
        }




        //<param name="" type="0" IsOutput="0"/>
        public static bool UpDateProcedureParametr(string procedureVal, string ParamName, DataParam newparam)
        {
            bool res = false;
            if (IsDocLoaded)
            {
                try
                {
                    XmlNode node =
                        ConfigurationDocument.SelectSingleNode(
                            SettingsHelperManager.QueryOfSelectingParametrOfTheProcedureByPval(procedureVal, ParamName));
                    if (node == null)
                    {
                        return false;
                    }

                    node.Attributes["name"].Value = newparam.Name;
                    node.Attributes["type"].Value = ((int)newparam.Type).ToString();
                    node.Attributes["direction"].Value = newparam.Direction.ToString();

                    if (Save())
                    {
                        res = true;
                        ReadAllProcedures();
                    }


                }
                catch
                {
                    return false;
                }
            }
            return res;
        }


        public static List<CustomSqlType> GetSqlTypes()
        {

            List<CustomSqlType> _customTypes = new List<CustomSqlType>();
            foreach (var customSqlType in Enum.GetNames(typeof(CustomSqlTypes)))
            {
                _customTypes.Add(new CustomSqlType((int)Enum.Parse(typeof(CustomSqlTypes), customSqlType), customSqlType));
            }


            return _customTypes;

        }



        #endregion



    }
}
