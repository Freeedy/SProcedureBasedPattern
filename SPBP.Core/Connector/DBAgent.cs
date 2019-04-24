using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SPBP
{
    public enum AgentState
    {
        Connected=0 , 
        Disconnected=1


    }

    public enum ConnectionLevel
    {
        Single=0 , 
        AllInOne=1 
    }

    public enum TransactionState
    {
        ActiveTransaction=0 , 
        Compleated=1,
        Ignore =2

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

        public SqlConnection Connection { get { return _con; } }

        public SqlTransaction Transaction { get { return _tran; }  }

        public AgentState AgentState { get; private set; } = AgentState.Disconnected;
        public ConnectionLevel ConnectionLevel { get; private set; } = ConnectionLevel.Single;
        public TransactionState TransactionState { get; private set; } = TransactionState.Ignore;
        public int RunTimeout { get;  set; } = 30; 

        public DbAgent()
        {

        }
        public DbAgent(string name, string constr, bool state, ConnectionLevel level = ConnectionLevel.Single ,int timeout = 30)
        {
            Name = name;
            SetConnectionString(constr );
            SetState(state);
            ConnectionLevel = level;
            RunTimeout = timeout; 
        }
        public DbAgent(string constr):this (string .Empty ,constr ,true )
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
        public void SetState(bool state)
        {
            _state = state;
        }
      
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public void OpenConnection()
        {
            if (AgentState != AgentState.Connected)
            {
                _con = new SqlConnection(_connectionString);
                _con.Open();

                if (_con.State == System.Data.ConnectionState.Open)
                {
                    AgentState = AgentState.Connected;
                   
                }
            }
           
        }

        public async Task OpenConnectionAsync()
        {
            if (AgentState != AgentState.Connected)
            {
                _con = new SqlConnection(_connectionString);
                 await _con.OpenAsync().ConfigureAwait(false);

                if (_con.State == System.Data.ConnectionState.Open)
                {
                    AgentState = AgentState.Connected;
                   
                }
            }
        }


        public void CloseConnection()
        {
            _con.Close();
            if(_tran!=null)
            {
                _tran.Dispose();
                _tran = null; 
            }
            AgentState = AgentState.Disconnected; 
        }


        public void BeginTransaction()
        {
            if (_con!=null && _con.State==System.Data.ConnectionState.Open)
            {
                _tran = _con.BeginTransaction();
                TransactionState = TransactionState.ActiveTransaction; 
            }
        }
        public void CommitTransaction()
        {
            if(_tran != null)
            {
                _tran.Commit();
                TransactionState = TransactionState.Compleated;
            }
        }

        public void RollbackTransaction()
        {
            if(_tran !=null)
            {
                
                _tran.Rollback();
                TransactionState = TransactionState.Compleated; 
            }
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
                TransactionState = TransactionState.Ignore;
                ConnectionLevel = ConnectionLevel.Single;
                AgentState = AgentState.Disconnected;
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
