using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;
using SPBP.Connector.Abstract;
using SPBP.Connector.Class;
using SPBP.Handling;
using SPBP.Connector.Attributes;
using System.Threading;
using System.Threading.Tasks;

namespace SPBP.Connector.Manager
{
    public static class DBCommander
    {
        #region Constants
        private const string XmlDbAgents = "dbagents";
        private const string XmlDbAgent = "dbagent";
        private const string XmlName = "name";
        private const string XmlConStr = "conStr";
        private const string XmlState = "state";

        #endregion


        #region Fields
        private static SqlDataReader _reader;
        private static SqlDataAdapter _adapter;
        private static XmlDocument _manDoc = new XmlDocument();
        private static bool _docInitialised = false;
        static SqlConnection _conn = null;
        private static string _datafile;
        private static Dictionary<string, DbAgent> _agents = new Dictionary<string, DbAgent>();
        private static bool _shouldBeSync = false;

        #endregion


        #region Properties
        public static Dictionary<string, DbAgent> Agents { get { return _agents; } set { _agents = value; } }
        public static bool DocumentIsINitialised { get { return _docInitialised; } }
        #endregion

        #region Methods
        //add agent to temp  
        public static void AddAgent(DbAgent agent)
        {
            _agents.Add(agent.Name, agent);
            _shouldBeSync = true;
        }

        //set agents file path 
        public static bool LoadAgentsFromFile(string filepath)
        {
            if (File.Exists(filepath))
            {
                try
                {
                    _manDoc.Load(filepath);
                    _agents.Clear();
                    if (_manDoc.DocumentElement.HasChildNodes)
                    {
                        foreach (XmlNode node in _manDoc.DocumentElement.ChildNodes)
                        {
                            DbAgent agent = new DbAgent();
                            agent.GetAgentFromXml(node);
                            _agents.Add(agent.Name, agent);
                        }

                        return true;
                    }

                }
                catch
                {

                    return false;
                }
            }


            return false;
        }


        //for the full initialisation  you need to add the data destination of  agents 
        public static bool Initialise(string filepath = null)
        {
            string path = filepath ?? _datafile;

            return _docInitialised = LoadAgentsFromFile(path);

        }


        public static void SetDataFilepath(string path)
        {
            _datafile = path;
        }


        public static void SaveDoc()
        {
            _manDoc.Save(_datafile);
        }

        //syncronize  datafile with temp  agents 
        public static bool Syncronize()
        {
            try
            {
                _manDoc.DocumentElement.InnerText = string.Empty;  // remove all agents 

                foreach (DbAgent agent in _agents.Values)
                {
                    _manDoc.DocumentElement.AppendChild(agent.ToXmlAgent());

                }
                SaveDoc();
                return true;

            }
            catch
            {
                return false;

            }



        }

        public static bool Restart()
        {
            if (_shouldBeSync)
            {
                Syncronize();
            }

            return Initialise();
        }

        public static void CreateDocTemplate(string file)
        {

            XmlTextWriter writer = new XmlTextWriter(_datafile, Encoding.UTF8);
            writer.WriteStartDocument();
            writer.WriteComment(string.Format("This is  The data of letter : \"{0}\" ", file));
            writer.WriteStartElement(XmlDbAgents); //Document Element  
            writer.WriteEndElement();// End of Document Element 
            writer.WriteEndDocument();
            writer.Close();
        }


        #region  ExecutionMethods


        public static ExecResult ExecProcDataReadByInheritance<T>(DbAgent agent, DataSItem item, out IBag<T> container)
            where T : DbObject
        {

            if (agent != null && agent.State)
            {
                return ExecuteProcedureDR<T>(item, out container, agent.ConnectionString);
            }
            else
            {
                throw new Exception("Agent is  null  or the state  is false  ");
            }


        }
        public static ExecResult ExecuteProcedureDataReaderByRef<T>(DbAgent agent, DataSItem item, out IBag<T> container)
        {

            if (agent != null && agent.State)
            {
                return ExecuteProcedureDRByReflection<T>(item, out container, agent.ConnectionString);
            }
            else
            {
                throw new Exception("Agent is  null  or the state  is false  ");
            }
        }




        public static ExecResult ExecuteNonQueryProcedure(DbAgent agent, DataSItem itm)
        {
            if (agent != null && agent.State)
            {
                string cnt = agent.ConnectionString;

                return ExecuteProcedureNonQuery(itm, cnt);
            }
            else
            {
                throw new Exception("Agent is  null  or state  is false  ");
            }
        }

        public static ExecResult ExecuteProcedureDataSet(DbAgent agent, DataSItem item, out DataSet set)
        {
            if (agent != null && agent.State)
            {
                string cnt = agent.ConnectionString;

                return ExecuteDS(item, out set, cnt);
            }
            else
            {
                throw new Exception("Agent is  null  or state  is false  ");
            }
        }



        #region Extensions

        public static ExecResult ExecDataReadByInheritance<T>(this DataSItem item, DbAgent agent, out IBag<T> container)
            where T : DbObject
        {
            return ExecProcDataReadByInheritance(agent, item, out container);
        }

        public static ExecResult ExecuteDataReaderByRef<T>(this DataSItem item, DbAgent agent, out IBag<T> container)
        {
            return ExecuteProcedureDataReaderByRef(agent, item, out container);
        }

        public static ExecResult ExecuteNonQuery(this DataSItem itm, DbAgent agent, string constr = null)
        {
            return ExecuteNonQueryProcedure(agent, itm);

        }

        public static ExecResult ExecDataSet(this DataSItem item, DbAgent agent, out DataSet set)
        {
            return ExecuteProcedureDataSet(agent, item, out set);
        }

        #endregion


        #region PrivateMethods

        /// <summary>
        /// </summary>
        /// <typeparam name="T"> Class type  </typeparam>
        /// <param name="proc"> DataSettingItem </param>
        /// <param name="container">Bag container </param>
        /// <param name="datasource"></param>
        /// <returns>return value of  procedure </returns>
        private static ExecResult ExecuteProcedureDR<T>(DataSItem proc, out  IBag<T> container, string datasource = null) where T : DbObject
        {
            ExecResult result = new ExecResult();
            SqlCommand cmd = null;

            result.StartMeasure();

            container = new Bag<T>();
            string constr = datasource ?? proc.ConnectionString;

            try
            {
                // create and open a connection object "Data Source=FARID-PC;Initial Catalog=InsuranceFactory;Integrated Security=True"
                _conn = new
                    SqlConnection(constr);
                _conn.Open();
                cmd = new SqlCommand(proc.Value, _conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;

                foreach (var item in proc.Params.Values)
                {
                    param = new SqlParameter();
                    param.ParameterName = item.Name;
                    param.Value = item.Value;
                    param.SqlDbType = SettingsHelperManager.DetermineSqlDbTYpe(item.Type);
                    param.Direction = SettingsHelperManager.GetParametrDirection(item.Direction);
                    cmd.Parameters.Add(param);
                }
                _reader = cmd.ExecuteReader();
                while (_reader.Read())
                {
                    container.SetFromReader(ref _reader);
                }
                _reader.Close();

                //set outputparams values 
                if (proc.HasOutputParam)
                {
                    foreach (DataParam value in proc.OutputParams.Values)
                    {
                        value.Value = cmd.Parameters[value.Name].Value.ToString();
                    }
                }

                if (cmd.Parameters.Count > 0)
                {
                    if (cmd.Parameters[proc.GetparamsByDirection(ParamDirection.Return)[0].Name] != null)
                    {
                        result.SetCode((int)cmd.Parameters[proc.GetparamsByDirection(ParamDirection.Return)[0].Name].Value);
                    }

                }
                _conn.Close();


            }
            catch (Exception exc)
            {
                throw exc;
            }

            result.StopMeashure();
            return result;
        }

        private static ExecResult ExecuteProcedureDRByReflection<T>(DataSItem proc, out  IBag<T> container, string datasource = null)
        {
            ExecResult result = new ExecResult();  // when user doesn't set return  parameter 
            SqlCommand cmd = null;
            container = new RefBag<T>();
            string constr = datasource ?? proc.ConnectionString;

            result.StartMeasure();
            try
            {
                // create and open a connection object "Data Source=FARID-PC;Initial Catalog=InsuranceFactory;Integrated Security=True"
                _conn = new
                    SqlConnection(constr);
                _conn.Open();
                cmd = new SqlCommand(proc.Value, _conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;

                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var item in proc.Params.Values)
                {
                    param = new SqlParameter();
                    param.ParameterName = item.Name;
                    param.Value = item.Value;
                    param.SqlDbType = SettingsHelperManager.DetermineSqlDbTYpe(item.Type);
                    param.Direction = SettingsHelperManager.GetParametrDirection(item.Direction);
                    cmd.Parameters.Add(param);
                }
                _reader = cmd.ExecuteReader();
                while (_reader.Read())
                {
                    container.SetFromReader(ref _reader);
                }
                _reader.Close();

                //set outputparams values 
                if (proc.HasOutputParam)
                {
                    foreach (DataParam value in proc.OutputParams.Values)
                    {
                        value.Value = cmd.Parameters[value.Name].Value.ToString();
                    }
                }

                if (cmd.Parameters.Count > 0)
                {
                    List<DataParam> ret = proc.GetparamsByDirection(ParamDirection.Return);
                    if (ret.Count > 0)
                    {
                        if (cmd.Parameters[ret[0].Name] != null)
                        {
                            result.SetCode((int)cmd.Parameters[proc.GetparamsByDirection(ParamDirection.Return)[0].Name].Value);
                        }
                    }


                }

                _conn.Close();


            }
            catch (Exception exc)
            {
                throw exc;
            }

            result.StopMeashure();
            return result;
        }


        private static ExecResult ExecuteProcedureNonQuery(DataSItem itm, string datasource = null)
        {
            ExecResult retval = new ExecResult();
            string constr = datasource ?? itm.ConnectionString;
            _conn = new SqlConnection(constr);
            retval.StartMeasure();
            _conn.Open();
            using (SqlCommand cmd = new SqlCommand(itm.Value, _conn))
            {

                SqlParameter param;

                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var item in itm.Params.Values)
                {
                    param = new SqlParameter();
                    param.ParameterName = item.Name;
                    param.Value = item.Value;
                    param.SqlDbType = SettingsHelperManager.DetermineSqlDbTYpe(item.Type);
                    param.Direction = SettingsHelperManager.GetParametrDirection(item.Direction);
                    cmd.Parameters.Add(param);
                }


                cmd.ExecuteNonQuery();
                //set outputparams values 
                if (itm.HasOutputParam)
                {
                    foreach (DataParam value in itm.OutputParams.Values)
                    {
                        value.Value = cmd.Parameters[value.Name].Value.ToString();
                    }
                }

                if (cmd.Parameters.Count > 0)
                {
                    List<DataParam> returnparam = itm.GetparamsByDirection(ParamDirection.Return);

                    if (returnparam.Count > 0)
                    {


                        string name = returnparam[0].Name;


                        if (cmd.Parameters[name] != null)
                        {
                            retval.SetCode((int)cmd.Parameters[name].Value);
                        }
                    }
                }



            }
            _conn.Close();
            retval.StopMeashure();
            return retval;
        } // update ,delete //insert 

        private static ExecResult ExecuteDS(DataSItem settingItem, out DataSet ds, string datasource = null)
        {

            ExecResult retval = new ExecResult();
            string cstr = datasource ?? settingItem.ConnectionString;
            _conn = new SqlConnection(cstr);

            retval.StartMeasure();

            _conn.Open();
            ds = new DataSet();
            using (SqlCommand cmd = new SqlCommand(settingItem.Value, _conn))
            {

                SqlParameter param;

                cmd.CommandType = CommandType.StoredProcedure;
                foreach (DataParam item in settingItem.Params.Values)
                {
                    param = new SqlParameter();
                    param.ParameterName = item.Name;
                    param.Value = item.Value;
                    param.SqlDbType = SettingsHelperManager.DetermineSqlDbTYpe(item.Type);
                    param.Direction = SettingsHelperManager.GetParametrDirection(item.Direction);
                    cmd.Parameters.Add(param);
                }

                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = cmd;
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.Fill(ds);
                }

                //set outputparams values 
                if (settingItem.HasOutputParam)
                {
                    foreach (DataParam value in settingItem.OutputParams.Values)
                    {
                        value.Value = cmd.Parameters[value.Name].Value.ToString();
                    }
                }

                if (cmd.Parameters.Count > 0)
                {
                    List<DataParam> returnparam = settingItem.GetparamsByDirection(ParamDirection.Return);

                    if (returnparam.Count > 0)
                    {


                        string name = returnparam[0].Name;


                        if (cmd.Parameters[name] != null)
                        {
                            retval.SetCode((int)cmd.Parameters[name].Value);
                        }
                    }
                }

            }
            _conn.Close();

            retval.StopMeashure();
            return retval;

        }





        #endregion

        #endregion

        #region Extensions

        public static XmlNode ToXmlAgent(this DbAgent agent)
        {
            XmlElement node = _manDoc.CreateElement(XmlDbAgent);

            node.SetAttribute(XmlName, agent.Name);
            node.SetAttribute(XmlConStr, agent.ConnectionString);
            node.SetAttribute(XmlState, agent.State.ToString());
            return node;
        }
        public static void GetAgentFromXml(this DbAgent agent, XmlNode node)
        {
            //  agent = new DbAgent();
            agent.Name = node.Attributes[XmlName].Value;
            agent.SetConnectionString(node.Attributes[XmlConStr].Value);
            agent.GetState(Convert.ToBoolean(node.Attributes[XmlState].Value));



        }

        #endregion


        //not ready yet  
        #region Async

       
        #endregion


        #region  SettingMethods
        public static DataSet RunDsViaCommand(string command, string datasource)
        {
            string cstr = datasource;
            _conn = new SqlConnection(cstr);
            _conn.Open();
            DataSet ds = new DataSet();
            using (SqlCommand cmd = new SqlCommand(command, _conn))
            {

                SqlParameter param;

                cmd.CommandType = CommandType.Text;


                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = cmd;
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.Fill(ds);
                }



            }
            _conn.Close();
            return ds;
        }


        #endregion

        #endregion

    }
}
