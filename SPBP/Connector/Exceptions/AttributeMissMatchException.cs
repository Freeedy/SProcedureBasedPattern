using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPBP.Connector.Exceptions
{
   public  class AttributeMissMatchException:Exception
   {
       public AttributeMissMatchException()
           : base("Member  doesn't have  required attribute")
       {
           
       }

   

   }
}
