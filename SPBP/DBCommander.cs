﻿using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;
using SPBP.Connector;
using SPBP.Connector.Abstract;
using SPBP.Connector.Class;
using SPBP.Handling;
using System.Threading.Tasks;

namespace SPBP
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

        static object _lockObj = new object();
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
                return ExecuteProcedureDR<T>(item, out container, agent);
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
                return ExecuteProcedureDRByReflection<T>(item, out container, agent);
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

                return ExecuteProcedureNonQuery(itm, agent);
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


                return ExecuteDS(item, out set, agent);
            }
            else
            {
                throw new Exception("Agent is  null  or state  is false  ");
            }
        }

        #region PrivateMethods

        /// <summary>
        /// </summary>
        /// <typeparam name="T"> Class type  </typeparam>
        /// <param name="proc"> DataSettingItem </param>
        /// <param name="container">Bag container </param>
        /// <param name="datasource"></param>
        /// <returns>return value of  procedure </returns>
        private static ExecResult ExecuteProcedureDR<T>(DataSItem proc, out IBag<T> container, DbAgent datasource) where T : DbObject
        {
            ExecResult result = new ExecResult();
            SqlCommand cmd = null;

            result.StartMeasure();

            container = new Bag<T>();


            try
            {
                // create and open a connection object "Data Source=FARID-PC;Initial Catalog=InsuranceFactory;Integrated Security=True"

                SqlConnection con = null;
                if (datasource.ConnectionLevel == ConnectionLevel.Single)//&& datasource.AgentState != AgentState.Connected)
                {
                    con = datasource.CreateConnection();
                    con.Open();

                }
                else if (datasource.ConnectionLevel == ConnectionLevel.AllInOne)
                {
                    con = datasource.Connection;
                }

                // return if is not connected
                if (con == null || con.State != ConnectionState.Open)
                {
                    result.SetCode(-2, "Not connected ! ");
                    result.StopMeashure();
                    return result;
                }


                using (cmd = new SqlCommand(proc.Value, con))
                {
                    cmd.CommandTimeout = datasource.RunTimeout;
                    //set transaction
                    if (datasource.TransactionState == TransactionState.ActiveTransaction)
                    {
                        cmd.Transaction = datasource.Transaction;
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;

                    foreach (var item in proc.Params.Values)
                    {
                        param = new SqlParameter
                        {
                            ParameterName = item.Name,
                            Value = item.Value,
                            SqlDbType = SettingsHelperManager.DetermineSqlDbTYpe(item.Type),
                            Direction = SettingsHelperManager.GetParametrDirection(item.Direction)
                        };
                        cmd.Parameters.Add(param);
                    }
                    using (_reader = cmd.ExecuteReader())
                    {
                        while (_reader.Read())
                        {
                            container.SetFromReader(ref _reader);
                        }

                    }
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
                            result.SetCode(Convert.ToInt32(cmd.Parameters[proc.GetparamsByDirection(ParamDirection.Return)[0].Name].Value));
                        }

                    }
                    //_conn.Close();
                    // } end of using 
                    //&& datasource.AgentState != AgentState.Disconnected)
                    if (datasource.ConnectionLevel == ConnectionLevel.Single && con != null)//&& con.State != ConnectionState.Closed)
                    {
                        con.Dispose();
                    }


                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            result.StopMeashure();
            return result;
        }

        private static ExecResult ExecuteProcedureDRByReflection<T>(DataSItem proc, out IBag<T> container, DbAgent datasource)
        {
            ExecResult result = new ExecResult();  // when user doesn't set return  parameter 
            SqlCommand cmd = null;
            container = new RefBag<T>();


            result.StartMeasure();
            try
            {
                // create and open a connection object "Data Source=FARID-PC;Initial Catalog=InsuranceFactory;Integrated Security=True"
                //using (_conn = new SqlConnection(constr))
                //{

                SqlConnection con = null;
                if (datasource.ConnectionLevel == ConnectionLevel.Single)//&& datasource.AgentState != AgentState.Connected)
                {
                    con = datasource.CreateConnection();
                    con.Open();

                }
                else if (datasource.ConnectionLevel == ConnectionLevel.AllInOne)
                {
                    con = datasource.Connection;
                }

                // return if is not connected
                if (con == null || con.State != ConnectionState.Open)
                {
                    result.SetCode(-2, "Not connected ! ");
                    result.StopMeashure();
                    return result;
                }

                cmd = new SqlCommand(proc.Value, con)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = datasource.RunTimeout
                };
                SqlParameter param;

                //set transaction
                if (datasource.TransactionState == TransactionState.ActiveTransaction)
                {
                    cmd.Transaction = datasource.Transaction;
                }

                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var item in proc.Params.Values)
                {
                    param = new SqlParameter
                    {
                        ParameterName = item.Name,
                        Value = item.Value,
                        SqlDbType = SettingsHelperManager.DetermineSqlDbTYpe(item.Type),
                        Direction = SettingsHelperManager.GetParametrDirection(item.Direction)
                    };
                    cmd.Parameters.Add(param);
                }
                _reader = cmd.ExecuteReader();
                while (_reader.Read())
                {
                    container.SetFromReader(ref _reader);
                }
                // } end of using 
                if (datasource.ConnectionLevel == ConnectionLevel.Single && con != null)//&& con.State!=ConnectionState.Closed)
                {
                    con.Dispose();
                }

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
                            result.SetCode(Convert.ToInt32(cmd.Parameters[proc.GetparamsByDirection(ParamDirection.Return)[0].Name].Value));
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            result.StopMeashure();
            return result;
        }

        private static ExecResult ExecuteProcedureNonQuery(DataSItem itm, DbAgent datasource)
        {
            ExecResult retval = new ExecResult();

            //using (_conn = new SqlConnection(constr))
            //{


            retval.StartMeasure();

            SqlConnection con = null;
            if (datasource.ConnectionLevel == ConnectionLevel.Single)//&& datasource.AgentState != AgentState.Connected)
            {
                con = datasource.CreateConnection();
                con.Open();

            }
            else if (datasource.ConnectionLevel == ConnectionLevel.AllInOne)
            {
                con = datasource.Connection;
            }

            // return if is not connected
            if (con == null || con.State != ConnectionState.Open)
            {
                retval.SetCode(-2, "Not connected ! ");
                retval.StopMeashure();
                return retval;
            }

            // return if is not connected
            //if (datasource.AgentState != AgentState.Connected)
            //{
            //    retval.SetCode(-2, "Not connected ! ");
            //    retval.StopMeashure();
            //    return retval;
            //}


            using (SqlCommand cmd = new SqlCommand(itm.Value, con))
            {

                SqlParameter param;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = datasource.RunTimeout;
                //set transaction
                if (datasource.TransactionState == TransactionState.ActiveTransaction)
                {
                    cmd.Transaction = datasource.Transaction;
                }

                foreach (var item in itm.Params.Values)
                {
                    param = new SqlParameter();
                    param.ParameterName = item.Name;
                    param.Value = item.Value;
                    param.SqlDbType = SettingsHelperManager.DetermineSqlDbTYpe(item.Type);
                    param.Direction = SettingsHelperManager.GetParametrDirection(item.Direction);
                    cmd.Parameters.Add(param);
                }


                retval.AffectedRows = cmd.ExecuteNonQuery();
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
                            retval.SetCode(Convert.ToInt32(cmd.Parameters[name].Value));
                        }
                    }
                }



            }
            // } end of using 
            if (datasource.ConnectionLevel == ConnectionLevel.Single && con != null)
            {
                con.Dispose();
            }

            retval.StopMeashure();

            return retval;
        } // update ,delete //insert 

        private static ExecResult ExecuteDS(DataSItem settingItem, out DataSet ds, DbAgent datasource)
        {

            ExecResult retval = new ExecResult();
            // string cstr = datasource ?? settingItem.ConnectionString;
            //using (_conn = new SqlConnection(cstr))
            //{




            retval.StartMeasure();

            ds = new DataSet();

            SqlConnection con = null;
            if (datasource.ConnectionLevel == ConnectionLevel.Single)//&& datasource.AgentState != AgentState.Connected)
            {
                con = datasource.CreateConnection();
                con.Open();

            }
            else if (datasource.ConnectionLevel == ConnectionLevel.AllInOne)
            {
                con = datasource.Connection;
            }

            // return if is not connected
            if (con == null || con.State != ConnectionState.Open)
            {
                retval.SetCode(-2, "Not connected ! ");
                retval.StopMeashure();
                return retval;
            }





            using (SqlCommand cmd = new SqlCommand(settingItem.Value, con))
            {

                SqlParameter param;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = datasource.RunTimeout;
                //set transaction
                if (datasource.TransactionState == TransactionState.ActiveTransaction)
                {
                    cmd.Transaction = datasource.Transaction;
                }

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
                            retval.SetCode(Convert.ToInt32(cmd.Parameters[name].Value));
                        }
                    }
                }

            }
            //end of using }
            if (datasource.ConnectionLevel == ConnectionLevel.Single && con != null)
            {
                con.Dispose();
            }

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
            agent.SetState(Convert.ToBoolean(node.Attributes[XmlState].Value));



        }


        public static ExecResult ExecDataReadByInheritance<T>(this DataSItem item, DbAgent agent, out IBag<T> container)
            where T : DbObject
        {
            return ExecProcDataReadByInheritance(agent, item, out container);
        }

        public static ExecResult ExecuteDataReaderByRef<T>(this DataSItem item, DbAgent agent, out IBag<T> container)
        {
            return ExecuteProcedureDataReaderByRef(agent, item, out container);
        }

        public static ExecResult ExecuteNonQuery(this DataSItem itm, DbAgent agent)
        {
            return ExecuteNonQueryProcedure(agent, itm);

        }

        public static ExecResult ExecDataSet(this DataSItem item, DbAgent agent, out DataSet set)
        {
            return ExecuteProcedureDataSet(agent, item, out set);
        }


        #endregion



        #region Async

        public static async Task<ExecAsyncResult> ExecDataReadByInheritanceAsync<T>(this DataSItem item, DbAgent agent) where T : DbObject
        {
            //return Task.Factory.StartNew(() =>
            //    {

            //        ExecAsyncResult result = new ExecAsyncResult();
            //        IBag<T> container;
            //        ExecResult rs = ExecDataReadByInheritance<T>(item, agent, out container);

            //        result.ExecutedProcedure = item;
            //        result.Result = rs;
            //        result.Object = container;
            //        result.ExecutionType=AsyncExecutionType.ExecByINheritance;
            //        return result;
            //    });


            ExecAsyncResult result = new ExecAsyncResult();
            result.ExecutedProcedure = item;
            IBag<T> container;

            if (agent != null && agent.State)
            {
                //   return ExecuteProcedureDRByReflection<T>(item, out container, agent.ConnectionString);

                //---

                SqlCommand cmd = null;
                container = new Bag<T>();


                result.StartMeasure();
                try
                {
                    // create and open a connection object "Data Source=FARID-PC;Initial Catalog=InsuranceFactory;Integrated Security=True"
                    //using (_conn = new SqlConnection(constr))
                    //{

                    SqlConnection con = null;
                    if (agent.ConnectionLevel == ConnectionLevel.Single)
                    {
                        con = agent.CreateConnection();
                        await con.OpenAsync();

                    }
                    else if (agent.ConnectionLevel == ConnectionLevel.AllInOne)
                    {
                        con = agent.Connection;
                    }

                    // check state of connection 

                    cmd = new SqlCommand(item.Value, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = agent.RunTimeout;
                    if (agent.TransactionState == TransactionState.ActiveTransaction)
                    {
                        cmd.Transaction = agent.Transaction;
                    }
                    SqlParameter param;


                    foreach (var itm in item.Params.Values)
                    {
                        param = new SqlParameter();
                        param.ParameterName = itm.Name;
                        param.Value = itm.Value;
                        param.SqlDbType = SettingsHelperManager.DetermineSqlDbTYpe(itm.Type);
                        param.Direction = SettingsHelperManager.GetParametrDirection(itm.Direction);
                        cmd.Parameters.Add(param);
                    }
                    using (_reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {

                        while (await _reader.ReadAsync().ConfigureAwait(false))
                        {
                            container.SetFromReader(ref _reader);
                        }
                    }

                    //set outputparams values 

                    if (item.HasOutputParam)
                    {
                        foreach (DataParam value in item.OutputParams.Values)
                        {
                            value.Value = cmd.Parameters[value.Name].Value.ToString();
                        }
                    }

                    if (cmd.Parameters.Count > 0)
                    {
                        List<DataParam> ret = item.GetparamsByDirection(ParamDirection.Return);
                        if (ret.Count > 0)
                        {
                            if (cmd.Parameters[ret[0].Name] != null)
                            {
                                result.Result.SetCode(
                                    Convert.ToInt32(
                                    cmd.Parameters[item.GetparamsByDirection(ParamDirection.Return)[0].Name].Value));
                            }
                        }


                    }

                    // }end of using
                    if (agent.ConnectionLevel == ConnectionLevel.Single && con != null) //&& agent.AgentState != AgentState.Disconnected)
                    {
                        con.Dispose();
                    }
                    result.Object = container;
                }
                catch (Exception exc)
                {
                    throw exc;
                }

                result.StopMeasure();
                result.ExecutionType = AsyncExecutionType.ExecByINheritance;
                return result;

                //----
            }
            else
            {
                throw new Exception("Agent is  null  or the state  is false  ");
            }




        }

        public static async Task<ExecAsyncResult> ExecuteDataReaderByRefAsync<T>(this DataSItem item, DbAgent agent)
        {

            ExecAsyncResult result = new ExecAsyncResult();
            result.ExecutedProcedure = item;
            IBag<T> container;

            if (agent != null && agent.State)
            {
                //   return ExecuteProcedureDRByReflection<T>(item, out container, agent.ConnectionString);

                //---

                SqlCommand cmd = null;
                container = new RefBag<T>();


                result.StartMeasure();
                try
                {

                    //using (_conn = new SqlConnection(constr))
                    //{
                    SqlConnection con = null;
                    if (agent.ConnectionLevel == ConnectionLevel.Single)
                    {
                        con = agent.CreateConnection();
                        await con.OpenAsync();
                        //   con = agent.CreateConnection();
                        //_shouldBeSync use 
                    }




                    cmd = new SqlCommand(item.Value, con)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = agent.RunTimeout
                    };

                    if (agent.TransactionState == TransactionState.ActiveTransaction)
                    {
                        cmd.Transaction = agent.Transaction;
                    }

                    SqlParameter param;

                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var itm in item.Params.Values)
                    {
                        param = new SqlParameter();
                        param.ParameterName = itm.Name;
                        param.Value = itm.Value;
                        param.SqlDbType = SettingsHelperManager.DetermineSqlDbTYpe(itm.Type);
                        param.Direction = SettingsHelperManager.GetParametrDirection(itm.Direction);
                        cmd.Parameters.Add(param);
                    }
                    using (_reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await _reader.ReadAsync().ConfigureAwait(false))
                        {
                            container.SetFromReader(ref _reader);
                        }
                    }

                    //set outputparams values 

                    if (item.HasOutputParam)
                    {
                        foreach (DataParam value in item.OutputParams.Values)
                        {
                            value.Value = cmd.Parameters[value.Name].Value.ToString();
                        }
                    }

                    if (cmd.Parameters.Count > 0)
                    {
                        List<DataParam> ret = item.GetparamsByDirection(ParamDirection.Return);
                        if (ret.Count > 0)
                        {
                            if (cmd.Parameters[ret[0].Name] != null)
                            {
                                result.Result.SetCode(
                                    Convert.ToInt32(
                                    cmd.Parameters[item.GetparamsByDirection(ParamDirection.Return)[0].Name].Value));
                            }
                        }


                    }

                    //  } end of using 
                    if (agent.ConnectionLevel == ConnectionLevel.Single && con != null)
                    {
                        con.Dispose();
                    }

                    result.Object = container;
                }
                catch (Exception exc)
                {
                    throw exc;
                }

                result.StopMeasure();
                result.ExecutionType = AsyncExecutionType.ExecByRef;
                return result;

                //----
            }
            else
            {
                throw new Exception("Agent is  null  or the state  is false  ");
            }


        }

        /// <summary>
        /// Changed!
        /// </summary>
        /// <param name="item"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        public static async Task<ExecAsyncResult> ExecuteNonQueryAsync(this DataSItem item, DbAgent agent)
        {
            //return Task.Factory.StartNew(() =>
            //{

            //    ExecAsyncResult result = new ExecAsyncResult();

            //    ExecResult rs = ExecuteNonQuery(item, agent);

            //    result.ExecutedProcedure = item;
            //    result.Result = rs;
            //    return result;
            //});

            ExecAsyncResult result = new ExecAsyncResult();
            result.ExecutedProcedure = item;

            if (agent != null && agent.State)
            {



                ///Open Connection via  agent

                SqlConnection con = null;
                result.StartMeasure();
                if (agent.ConnectionLevel == ConnectionLevel.Single)
                {
                    con = agent.CreateConnection();
                    await con.OpenAsync();
                }



                using (SqlCommand cmd = new SqlCommand(item.Value, agent.Connection))
                {
                    if (agent.TransactionState == TransactionState.ActiveTransaction)
                    {
                        cmd.Transaction = agent.Transaction;
                    }


                    SqlParameter param;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = agent.RunTimeout;
                    foreach (var itm in item.Params.Values)
                    {
                        param = new SqlParameter();
                        param.ParameterName = itm.Name;
                        param.Value = itm.Value;
                        param.SqlDbType = SettingsHelperManager.DetermineSqlDbTYpe(itm.Type);
                        param.Direction = SettingsHelperManager.GetParametrDirection(itm.Direction);
                        cmd.Parameters.Add(param);
                    }


                    result.Result.AffectedRows = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    //set outputparams values 
                    if (item.HasOutputParam)
                    {
                        foreach (DataParam value in item.OutputParams.Values)
                        {
                            value.Value = cmd.Parameters[value.Name].Value.ToString();
                        }
                    }

                    if (cmd.Parameters.Count > 0)
                    {
                        List<DataParam> returnparam = item.GetparamsByDirection(ParamDirection.Return);

                        if (returnparam.Count > 0)
                        {


                            string name = returnparam[0].Name;


                            if (cmd.Parameters[name] != null)
                            {
                                result.Result.SetCode(Convert.ToInt32(cmd.Parameters[name].Value));
                            }
                        }
                    }



                }
                if (agent.ConnectionLevel == ConnectionLevel.Single && con != null)
                {
                    con.Dispose();
                }

                result.StopMeasure();


            }
            else
            {
                throw new Exception("Agent is  null  or state  is false  ");
            }


            return result;
        }


        public static async Task<ExecAsyncResult> ExecDataSetAsync(this DataSItem item, DbAgent agent)
        {
            return await Task.Factory.StartNew(() =>
            {

                ExecAsyncResult result = new ExecAsyncResult();

                DataSet rsSet;
                ExecResult rs = ExecDataSet(item, agent, out rsSet);

                result.ExecutedProcedure = item;
                result.Result = rs;
                result.Object = rsSet;
                result.ExecutionType = AsyncExecutionType.ExecDataSet;
                return result;
            });
        }


        #endregion

        #region previousVersion 
        /*
          public static async Task< ExecAsyncResult> ExecDataReadByInheritanceAsync<T>(this DataSItem item, DbAgent agent ) where T : DbObject
       {
           //return Task.Factory.StartNew(() =>
           //    {

           //        ExecAsyncResult result = new ExecAsyncResult();
           //        IBag<T> container;
           //        ExecResult rs = ExecDataReadByInheritance<T>(item, agent, out container);

           //        result.ExecutedProcedure = item;
           //        result.Result = rs;
           //        result.Object = container;
           //        result.ExecutionType=AsyncExecutionType.ExecByINheritance;
           //        return result;
           //    });


           ExecAsyncResult result = new ExecAsyncResult();
           result.ExecutedProcedure = item;
           IBag<T> container;

           if (agent != null && agent.State)
           {
               //   return ExecuteProcedureDRByReflection<T>(item, out container, agent.ConnectionString);

               //---

               SqlCommand cmd = null;
               container = new Bag<T>();
               string constr = agent.ConnectionString;

               result.StartMeasure();
               try
               {
                   // create and open a connection object "Data Source=FARID-PC;Initial Catalog=InsuranceFactory;Integrated Security=True"
                   using (_conn = new SqlConnection(constr))
                   {



                       await _conn.OpenAsync().ConfigureAwait(false);

                       cmd = new SqlCommand(item.Value, _conn);
                       cmd.CommandType = CommandType.StoredProcedure;
                       SqlParameter param;

                       cmd.CommandType = CommandType.StoredProcedure;
                       foreach (var itm in item.Params.Values)
                       {
                           param = new SqlParameter();
                           param.ParameterName = itm.Name;
                           param.Value = itm.Value;
                           param.SqlDbType = SettingsHelperManager.DetermineSqlDbTYpe(itm.Type);
                           param.Direction = SettingsHelperManager.GetParametrDirection(itm.Direction);
                           cmd.Parameters.Add(param);
                       }
                       _reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                       while (await _reader.ReadAsync().ConfigureAwait(false))
                       {
                           container.SetFromReader(ref _reader);
                       }
                       _reader.Close();

                       //set outputparams values 

                       if (item.HasOutputParam)
                       {
                           foreach (DataParam value in item.OutputParams.Values)
                           {
                               value.Value = cmd.Parameters[value.Name].Value.ToString();
                           }
                       }

                       if (cmd.Parameters.Count > 0)
                       {
                           List<DataParam> ret = item.GetparamsByDirection(ParamDirection.Return);
                           if (ret.Count > 0)
                           {
                               if (cmd.Parameters[ret[0].Name] != null)
                               {
                                   result.Result.SetCode(
                                       (int)
                                       cmd.Parameters[item.GetparamsByDirection(ParamDirection.Return)[0].Name].Value);
                               }
                           }


                       }

                   }

                   result.Object = container;
               }
               catch (Exception exc)
               {
                   throw exc;
               }

               result.StopMeasure();
               result.ExecutionType=AsyncExecutionType.ExecByINheritance;
               return result;

               //----
           }
           else
           {
               throw new Exception("Agent is  null  or the state  is false  ");
           }




       }

       public static async  Task<ExecAsyncResult> ExecuteDataReaderByRefAsync<T>(this DataSItem item, DbAgent agent)
       {

           ExecAsyncResult result = new ExecAsyncResult();
           result.ExecutedProcedure = item;
           IBag<T> container;

           if (agent != null && agent.State)
           {
            //   return ExecuteProcedureDRByReflection<T>(item, out container, agent.ConnectionString);

               //---

               SqlCommand cmd = null;
               container = new RefBag<T>();
               string constr = agent.ConnectionString;

               result.StartMeasure();
               try
               {
                   // create and open a connection object "Data Source=FARID-PC;Initial Catalog=InsuranceFactory;Integrated Security=True"
                   using (_conn = new SqlConnection(constr))
                   {



                       await  _conn.OpenAsync().ConfigureAwait(false);

                       cmd = new SqlCommand(item.Value, _conn);
                       cmd.CommandType = CommandType.StoredProcedure;
                       SqlParameter param;

                       cmd.CommandType = CommandType.StoredProcedure;
                       foreach (var itm in item.Params.Values)
                       {
                           param = new SqlParameter();
                           param.ParameterName = itm.Name;
                           param.Value = itm.Value;
                           param.SqlDbType = SettingsHelperManager.DetermineSqlDbTYpe(itm.Type);
                           param.Direction = SettingsHelperManager.GetParametrDirection(itm.Direction);
                           cmd.Parameters.Add(param);
                       }
                       _reader = await  cmd.ExecuteReaderAsync().ConfigureAwait(false);

                       while (  await _reader.ReadAsync().ConfigureAwait(false))
                       {
                           container.SetFromReader(ref _reader);
                       }
                       _reader.Close();

                       //set outputparams values 

                       if (item.HasOutputParam)
                       {
                           foreach (DataParam value in item.OutputParams.Values)
                           {
                               value.Value = cmd.Parameters[value.Name].Value.ToString();
                           }
                       }

                       if (cmd.Parameters.Count > 0)
                       {
                           List<DataParam> ret = item.GetparamsByDirection(ParamDirection.Return);
                           if (ret.Count > 0)
                           {
                               if (cmd.Parameters[ret[0].Name] != null)
                               {
                                   result.Result.SetCode(
                                       (int)
                                       cmd.Parameters[item.GetparamsByDirection(ParamDirection.Return)[0].Name].Value);
                               }
                           }


                       }

                   }

                   result.Object = container;
               }
               catch (Exception exc)
               {
                   throw exc;
               }

               result.StopMeasure();
               result.ExecutionType=AsyncExecutionType.ExecByRef;
               return result;

               //----
           }
           else
           {
               throw new Exception("Agent is  null  or the state  is false  ");
           }


       }

       public static async Task<ExecAsyncResult> ExecuteNonQueryAsync(this DataSItem item, DbAgent agent)
       {
           //return Task.Factory.StartNew(() =>
           //{

           //    ExecAsyncResult result = new ExecAsyncResult();

           //    ExecResult rs = ExecuteNonQuery(item, agent);

           //    result.ExecutedProcedure = item;
           //    result.Result = rs;
           //    return result;
           //});

           ExecAsyncResult result = new ExecAsyncResult();
           result.ExecutedProcedure = item;

           if (agent != null && agent.State)
           {
               string cnt = agent.ConnectionString;

               using (_conn = new SqlConnection(cnt))
               {



                   result.StartMeasure();
                   if (_conn.State == ConnectionState.Closed)
                   {
                        await   _conn.OpenAsync().ConfigureAwait(false);
                   }
                   using (SqlCommand cmd = new SqlCommand(item.Value, _conn))
                   {

                       SqlParameter param;

                       cmd.CommandType = CommandType.StoredProcedure;
                       foreach (var itm in item.Params.Values)
                       {
                           param = new SqlParameter();
                           param.ParameterName = itm.Name;
                           param.Value = itm.Value;
                           param.SqlDbType = SettingsHelperManager.DetermineSqlDbTYpe(itm.Type);
                           param.Direction = SettingsHelperManager.GetParametrDirection(itm.Direction);
                           cmd.Parameters.Add(param);
                       }


                      await  cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                       //set outputparams values 
                       if (item.HasOutputParam)
                       {
                           foreach (DataParam value in item.OutputParams.Values)
                           {
                               value.Value = cmd.Parameters[value.Name].Value.ToString();
                           }
                       }

                       if (cmd.Parameters.Count > 0)
                       {
                           List<DataParam> returnparam = item.GetparamsByDirection(ParamDirection.Return);

                           if (returnparam.Count > 0)
                           {


                               string name = returnparam[0].Name;


                               if (cmd.Parameters[name] != null)
                               {
                                   result.Result.SetCode((int)cmd.Parameters[name].Value);
                               }
                           }
                       }



                   }
               }
               result.StopMeasure();


           }
           else
           {
               throw new Exception("Agent is  null  or state  is false  ");
           }


           return result; 
       }

       public static async Task<ExecAsyncResult> ExecDataSetAsync(this DataSItem item, DbAgent agent)
       {
           return await  Task.Factory.StartNew(() =>
           {

               ExecAsyncResult result = new ExecAsyncResult();

               DataSet rsSet;
               ExecResult rs = ExecDataSet(item, agent, out rsSet);

               result.ExecutedProcedure = item;
               result.Result = rs;
               result.Object = rsSet; 
               result.ExecutionType=AsyncExecutionType.ExecDataSet;
               return result;
           });
       }

            */

        #endregion



        #region  SettingMethods
        public static DataSet RunDsViaCommand(string command, string datasource)
        {

            _conn = new SqlConnection(datasource);
            _conn.Open();
            DataSet ds = new DataSet();
            using (SqlCommand cmd = new SqlCommand(command, _conn))
            {

                // SqlParameter param;

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
