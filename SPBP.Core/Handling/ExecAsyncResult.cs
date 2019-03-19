using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPBP.Handling
{
    public class ExecAsyncResult
    {
        private Stopwatch _sw = new Stopwatch();
        public ExecResult Result { get; set; }
        public DataSItem ExecutedProcedure { get; set; }
       // public Stopwatch SW { get { return _sw;  } }

        //it can be dictionary ,  data set  , and etc ... 


        public ExecAsyncResult(ExecResult res , DataSItem itm )
        {
            Result = res;
            ExecutedProcedure = itm; 
        }

        public ExecAsyncResult(DataSItem itm ):this(new ExecResult(),itm)
        {
            
        }
        public ExecAsyncResult():this(new ExecResult( ),null)
        {
            
        }

        public object Object { get; set; }

        private AsyncExecutionType _type = AsyncExecutionType.ExecNonQuery; // by default 
        public AsyncExecutionType ExecutionType { get { return _type; } set { _type = value; } }


        public void StartMeasure()
        {
            Result.StartMeasure();
        }

        public void StopMeasure()
        {
            Result.StopMeashure();
        }


        public override string ToString()
        {
            return string .Format("[{0}] - Type : {1}",Result.ToString(),ExecutionType.ToString());
        }

    }

    public enum AsyncExecutionType
    {
        ExecByINheritance = 0, ExecByRef = 1, ExecNonQuery = 2, ExecDataSet = 3
    }


}
