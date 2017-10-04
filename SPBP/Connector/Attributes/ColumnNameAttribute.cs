using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPBP.Connector.Attributes
{
    public class ColumnNameAttribute:Attribute
    {
        private string _value;



        public string Value { get { return _value; } }

        public bool HasValue { get { return !string.IsNullOrWhiteSpace(_value); } }


        public ColumnNameAttribute(string columnName)
        {
            _value = columnName;
        }

        public ColumnNameAttribute()
        {
            
        }
        
    }
}
