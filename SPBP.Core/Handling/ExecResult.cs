using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPBP
{
    public class ExecResult
    {
        private Stopwatch _sw=new Stopwatch()  ;
        private int _resultCode = -1; //by default  -2 error 
        private string _description = "Default result code";
        private double  _execTimeSecond = 0;
        int _affectedRows = 0; 


        public Stopwatch SW { get { return _sw; } }

        public int Code { get { return _resultCode; } }
        public double  ExecutionTime { get { return _execTimeSecond; } }
        public string Description { get { return _description; } }
        public int AffectedRows { get { return _affectedRows; } set { _affectedRows = value; }  }


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
            _execTimeSecond = _sw.Elapsed.TotalMilliseconds;
        }

        public void SetCode(int code ,string desc = null)
        {
            _resultCode = code;
            if(_description!=null)
            {
                _description = desc;
            }
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
