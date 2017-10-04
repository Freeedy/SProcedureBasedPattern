using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SPBP.Connector.Attributes
{
   public static  class AttributesHandlers
    {

       public static bool IsDbObjectAttributed<T>()
       {
           Attribute member = typeof(T).GetCustomAttribute<SPBP.Connector.Attributes.DbObjectAttribute>();

           if (member != null)
           {
               return true;
           }
           return false;
       }



    }
}
