using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPBP.Handling
{
    public class ExecAsyncResult
    {

        public ExecResult Result { get; set; }
        public DataSItem ExecutedProcedure { get; set; }

        //it can be dictionary ,  data set  , and etc ... 
        public object Object { get; set; }

        private AsyncExecutionType _type = AsyncExecutionType.ExecNonQuery; // by default 
        public AsyncExecutionType ExecutionType { get { return _type; } set { _type = value; } }


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
