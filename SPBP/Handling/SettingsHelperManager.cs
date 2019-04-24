using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;

namespace SPBP.Handling
{
    public enum CustomSqlTypes : int
    {
        Int = 0,
        Long = 1,
        Datetime = 2,
        String = 3,
        Double = 4,
        Money = 5,
        Bool = 6,
        Char = 7,
        SmallInt = 8,
        Real = 9
    }

    public enum ParamDirection : int
    {
        Input = 0,
        Output = 1,
        Return = 2,
        InOut = 3
    }

    public class CustomSqlType
    {
        public int Key { get; set; }
        public string Value { get; set; }

        public CustomSqlType(int key, string value)
        {
            Key = key;
            Value = value;
        }
        public CustomSqlType()
        {

        }
        public override string ToString()
        {
            return Key.ToString();
        }



    }


    public static class SettingsHelperManager
    {
        private static FileStream _stream;
        private static SqlConnection _connection;
        private static SqlDataAdapter _adapter;
        

        public static DataTable FillTable(string connection, DataSItem item)
        {
            DataTable dt;
            using (_connection = new SqlConnection(connection))
            {
                using (SqlCommand cmd = new SqlCommand(item.Value, _connection))
                {
                    cmd.CommandType = CommandType.Text;
                    SqlParameter prm;
                    foreach (DataParam param in item.Params.Values)
                    {
                        prm = new SqlParameter();
                    }


                    using (_adapter = new SqlDataAdapter(cmd))
                    {
                        using (dt = new DataTable())
                        {
                            _adapter.Fill(dt);

                        }
                    }
                }
            }

            return dt;
        }
        public static ParameterDirection GetParametrDirection(ParamDirection direction)
        {
            switch (direction)
            {
                case ParamDirection.Input:
                    return ParameterDirection.Input;
                   
                case ParamDirection.Output:
                    return ParameterDirection.Output;
                   
                case ParamDirection.Return:
                    return ParameterDirection.ReturnValue;
                   
                case ParamDirection.InOut:
                    return ParameterDirection.InputOutput;
                
                default:
                    return ParameterDirection.Input;
                   

            }
        }
        public static SqlDbType DetermineSqlDbTYpe(CustomSqlTypes type)
        {
            switch (type)
            {
                case CustomSqlTypes.Int:
                    return SqlDbType.Int;
                  
                case CustomSqlTypes.Long:
                    return SqlDbType.BigInt;
                   
                case CustomSqlTypes.String:
                    return SqlDbType.NVarChar;
                   
                case CustomSqlTypes.Datetime:
                    return SqlDbType.DateTime;
                   
                case CustomSqlTypes.Double:
                    return SqlDbType.Decimal;
                  
                case CustomSqlTypes.Money:
                    return SqlDbType.Money;
                  
                case CustomSqlTypes.SmallInt:
                    return SqlDbType.SmallInt;
                   
                case CustomSqlTypes.Char:
                    return SqlDbType.NChar;
                  
                case CustomSqlTypes.Real:
                    return SqlDbType.Real;
                  
                default:
                    return SqlDbType.NVarChar;
                   

            }
        }
        public static DataTable ExecuteQuery(string connection, string query)
        {
            DataTable dt;
            using (_connection = new SqlConnection(connection))
            {
                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (_adapter = new SqlDataAdapter(cmd))
                    {
                        using (dt = new DataTable())
                        {
                            _adapter.Fill(dt);

                        }
                    }
                }
            }

            return dt;
        }
        public static string QueryOfSelectingProcedureByName(string name)
        {
            return string.Format(@"/DbSettings/Procedures/item[@name='{0}']", name);
        }
        public static string QueryOfSelectinProcedureByValue(string value)
        {
            string A = string.Format(@"/DbSettings/Procedures/item[@value='{0}']", value);
            return string.Format(A);
        }
        public static string QueryOfListingAllProcedures()
        {
            return string.Format(@"/DbSettings/Procedures/item");
        }
        public static string QueryOfSelectingParametrOfTheProcedureByPval(string pval, string parametrVal)
        {
            return string.Format(@"/DbSettings/Procedures/item[@value='{0}']/param[@name='{1}']", pval, parametrVal);
        }
        public static string QueryOfListingAllViews()
        {
            return string.Format(@"/DbSettings/Views/item");
        }
        public static string QueryOfSelectingViewByName(string name)
        {
            return string.Format(@"/DbSettings/Views/item[@name='{0}']", name);
        }
        public static string QueryOfSelectingViewByValue(string value)
        {
            return string.Format(@"/DbSettings/Procedures/item[@value='{0}']", value);
        }
        public static string QueryOfSelectingProceduresParent()
        {
            return string.Format(@"/DbSettings/Procedures");
        }
        public static string QueryOfselectingViewsParent()
        {
            return string.Format(@"DbSettings/Views");
        }


        public static void CreateDocumentTemplate(string filepath)
        {
            XmlTextWriter writer = new XmlTextWriter(filepath, Encoding.UTF8);

            writer.WriteStartDocument();
            writer.WriteComment(string.Format("Template of  document "));
            writer.WriteStartElement("DbSettings"); //Document Element  

            writer.WriteEndElement();// End of Document Element 
            writer.WriteEndDocument();
            writer.Close();

            
        }


        #region Serialization
        //is not used yet 
        public static void BinnarySerialize(string filepath, object obj)
        {
            _stream = File.Create(filepath);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(_stream, obj);
            _stream.Close();

        }

       

        #endregion

        #region StringhHelper
        public static string GetAfterChar(string input, char sign)
        {
            string tempString = " ";
            int index = input.IndexOf(sign);
            if (index != -1)
            {
                tempString = input.Substring(index + 1);

            }
            return tempString;
        }
        public static string GetBeforeChar(string input, char sign)
        {
            string tempString = "";
            int index = input.IndexOf(sign);
            if (index != -1)
            {
                tempString = input.Substring(0, index);

            }
            return tempString;
        }
        #endregion
    }


}
