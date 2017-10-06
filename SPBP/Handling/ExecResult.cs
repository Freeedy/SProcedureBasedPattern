using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPBP.Handling
{
    public class ExecResult
    {
        private Stopwatch _sw=new Stopwatch()  ;
        private int _resultCode = -1; //by default 
        private string _description = "Default result code";
        private long  _execTimeSecond = 0;

        public Stopwatch SW { get { return _sw; } }

        public int Code { get { return _resultCode; } }
        public long  ExecutionTime { get { return _execTimeSecond; } }
        public string Description { get { return _description; } }

        public ExecResult(int code, string description)
        {
            _resultCode = code;
            _description = description;
        }
        public ExecResult(int code) : this(code, string.Empty) { }
        public ExecResult() : this(-1, string.Empty) { }


        public void StartMeasure()
        {
          //  _sw = Stopwatch.StartNew();
            _sw.Start();
        }

        public void StopMeashure()
        {
           
            
            _sw.Stop();
            _execTimeSecond = _sw.Elapsed.Milliseconds;
        }

        public void SetCode(int code)
        {
            _resultCode = code;
        }

        public override string ToString()
        {
            return string.Format("Code : {0} - Execution Time :  {1} ms  ",Code.ToString( ),ExecutionTime.ToString( ));
        }

        public static implicit operator int(ExecResult result)
        {
            return result.Code;
        }

        public static implicit operator ExecResult(int code)
        {
            return new ExecResult(code);
        }

    }
}
