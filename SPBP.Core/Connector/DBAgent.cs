using System;
using System.Data.SqlClient;

namespace SPBP.Connector
{
    public enum AgentState
    {
        Connected=0 , 
        HasTransaction

    }

    public class DbAgent:IDisposable
    {
        private string _connectionString=string .Empty ;
        SqlConnection _con;
        SqlTransaction _tran; 
        private bool _state = false;
        bool _disposed = false;

        public string Name { get; set;  }
        public string ConnectionString { get { return _connectionString; } }
        public bool State { get { return _state; } }

        public DbAgent()
        {

        }
        public DbAgent(string name, string constr, bool state)
        {
            Name = name;
            SetConnectionString(constr );
            GetState(state);
        }
        public DbAgent(string constr):this (string .Empty ,constr ,false )
        {
            
        }

        


        public void SetConnectionString(string constr)
        {
            _connectionString = constr;
        }

        public void Enable()
        {
            _state = true;
        }
        public void Disable()
        {
            _state = false;
        }
        public void GetState(bool state)
        {
            _state = state;
        }
      

        public void OpenConnection()
        {     
                _con = new SqlConnection(_connectionString);
                _con.Open();
                 if(_con.State==System.Data.ConnectionState.Open)
            {

            }
           
        }

        public void CloseConnection()
        {
            _con.Close();
        }


        public void BeginTransaction()
        {
            if()
        }
        public void CommitTransaction()
        {

        }

        public void RollbackTransaction()
        {

        }





        protected  virtual void Dispose(bool disposing)
        {
            if(!_disposed)
            {
               
                if(disposing)
                {
                    if(_tran!=null)
                    {
                        _tran.Dispose(); 
                    }

                    if(_con!=null)
                    {
                        _con.Dispose();
                        
                    }
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
