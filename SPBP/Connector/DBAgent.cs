namespace SPBP.Connector
{
    public class DbAgent
    {
        private string _connectionString=string .Empty ;

        private bool _state = false; 

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
      

    }
}
