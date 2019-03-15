using System.Data.SqlClient;
using SPBP.Connector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPBP.Handling;
using System.Xml;

namespace SPBP.Modules.SQl
{
    public static class SqlManager
    {
        private static string _fileDestination;

        private static DbAgent _currentagent;

        private static XmlDocument _tempDoc = new XmlDocument();

        private static string _getAllProcedures = @"select schema_name(schema_id) as [schema],name from sys.procedures  where object_id not in(select major_id from sys.extended_properties)";

        private static string _getParamsOFProc = @"select PARAMETER_NAME , PARAMETER_MODE,DATA_TYPE from information_schema.parameters where specific_name='{0}'";


        public static DbAgent CurrentAgent { get { return _currentagent; } set { _currentagent = value; } }



        public static async Task<Dictionary<string, DataSItem>> GetAllSqlProceduresAsync(DbAgent agent = null)
        {
            string source;

            Dictionary<string, DataSItem> procedures = new Dictionary<string, DataSItem>();
            try
            {
                if (agent != null)
                {
                    source = agent.ConnectionString;
                }
                else
                {
                    source = _currentagent.ConnectionString;
                }

                using (SqlConnection con = new SqlConnection(source))
                {
                  await   con.OpenAsync().ConfigureAwait(false);

                    using (SqlCommand cmd = new SqlCommand(_getAllProcedures, con))
                    {
                        cmd.CommandType = CommandType.Text;

                        SqlDataReader reader = cmd.ExecuteReader();
                        while ( await reader.ReadAsync().ConfigureAwait(false))
                        {
                            DataSItem proc = new DataSItem();
                            proc.ConnectionString = "-1";
                            proc.Schema = reader[0].ToString();
                             proc.Name = reader[1].ToString();
                            procedures.Add(proc.Value, proc);
                        }

                    }
                }

            }
            catch (Exception exc)
            {

                throw exc;
            }

            return procedures;
        }
        public static Dictionary<string, DataSItem> GetAllSqlProcedures(DbAgent agent = null)
        {
            string source;

            Dictionary<string, DataSItem> procedures = new Dictionary<string, DataSItem>();
            try
            {
                if (agent != null)
                {
                    source = agent.ConnectionString;
                }
                else
                {
                    source = _currentagent.ConnectionString;
                }

                using (SqlConnection con = new SqlConnection(source))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(_getAllProcedures, con))
                    {
                        cmd.CommandType = CommandType.Text;

                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            DataSItem proc = new DataSItem();
                            proc.ConnectionString = "-1";
                            proc.Schema = reader[0].ToString(); 
                            proc.Name = reader[1].ToString();
                            procedures.Add(proc.Value, proc);
                        }

                    }
                }

            }
            catch (Exception exc)
            {

                throw exc;
            }

            return procedures;

        }

        public static bool WriteToFileFromServer(string filepath, DbAgent agent = null)
        {
            bool result = false;
            SettingsHelperManager.CreateDocumentTemplate(filepath);
            DbCommandManagar.SetConfiguration(filepath);
            DbCommandManagar.LoadDocument();
            _tempDoc.Load(filepath);



            ProcedureFactory factory = GetProceduresFactory(agent);

            result = DbCommandManagar.AddFactory(factory);

            return result;
        }

        //used 
        public static bool WriteFactoryFile(this ProcedureFactory factory , string filepath, DbAgent agent = null)
        {
            bool result = false;
            SettingsHelperManager.CreateDocumentTemplate(filepath);
            DbCommandManagar.SetConfiguration(filepath);
            DbCommandManagar.LoadDocument();
            _tempDoc.Load(filepath);

            
            result = DbCommandManagar.AddFactory(factory);

            return result;
        }

        //fill  procedure with param and return 
        public static void FillPRocedureParamsFromSQLAgent(this DataSItem procedure, DbAgent agent = null)
        {


            string source;

            if (agent != null)
            {
                source = agent.ConnectionString;
            }
            else
            {

                source = _currentagent.ConnectionString;
            }


            using (SqlConnection con = new SqlConnection(source))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(string.Format(_getParamsOFProc, procedure.Name), con))
                {
                    procedure.Params.Clear();

                    cmd.CommandType = CommandType.Text;

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DataParam prm = new DataParam();

                        prm.Name = reader[0].ToString();
                        prm.Default = "";
                        prm.Direction = GetDirection(reader[1].ToString());
                        prm.Type = GetType(reader[2].ToString());
                        prm.SetParent(procedure);
                        procedure.AddParam( prm);

                    }


                }

            }


        }

       

        public static ProcedureFactory GetProceduresFactory(DbAgent agent = null   )
        {
            ProcedureFactory fact = new ProcedureFactory();


            Dictionary<string, DataSItem> procedures = GetAllSqlProcedures(agent);

            foreach (DataSItem value in procedures.Values)
            {
                value.FillPRocedureParamsFromSQLAgent(agent);
                
            }


            fact.SetProcedures(procedures);

            return fact;




        }

       public static void AddReturnValueToEachProcedure(this ProcedureFactory factory)
       {
           foreach (DataSItem item  in factory .Procedures.Values)
           {
                item .AddReturnParam(CustomSqlTypes.Int);
           }
       }
       

        public static ParamDirection GetDirection(string direction)
        {
            switch (direction)
            {
                case "INOUT":
                    return ParamDirection.Output;
                    break;
                default:
                    return ParamDirection.Input;
            }
        }


        public static CustomSqlTypes GetType(string typename)
        {
            switch (typename)
            {
                case "nvarchar":
                    return CustomSqlTypes.String;
                    break;
                case "int":
                    return CustomSqlTypes.Int;
                    break;
                case "datetime":
                    return CustomSqlTypes.Datetime;
                    break;
                case "money":
                    return CustomSqlTypes.Money;
                    break;
                case "real":
                    return CustomSqlTypes.Double;
                    break;
                case "nchar":
                    return CustomSqlTypes.Char;
                    break;
                case "ntext":
                    return CustomSqlTypes.String;
                    break;
                case "smallint":
                    return CustomSqlTypes.SmallInt;
                    break;
                default:
                    return CustomSqlTypes.String;
                    break;
            }
        }


        #region  Async

        public static Task<bool > WriteToFileAsync(this ProcedureFactory factory,string filepath, DbAgent agent = null)
        {
            return Task.Factory.StartNew(() =>
                {
                    return WriteFactoryFile(factory, filepath, agent);
                });
        }

       public static async  Task<ProcedureFactory> GetProceduresFactoryAsync(DbAgent agent = null)
       {
           ProcedureFactory fact = new ProcedureFactory();


           Dictionary<string, DataSItem> procedures = await  GetAllSqlProceduresAsync(agent);

           foreach (DataSItem value in procedures.Values)
           {
               value.FillPRocedureParamsFromSQLAgent(agent);

           }


           fact.SetProcedures(procedures);

           return fact;
       }

       public static Task AddReturnValueToEachProcedureAsync(this ProcedureFactory factory)
        {
            return Task.Factory.StartNew(() => AddReturnValueToEachProcedure(factory));
        }


     //public static Task<ProcedureFactory> GetPRocedureFactoryAsync ()


        #endregion

    }
}
