
using SPBP.Connector.Interfaces;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace SPBP.Connector.Abstract
{

    public delegate void SetItem(SqlDataReader obj);

    public delegate void SetValue(object var);



   public abstract  class DbObject:IDBObject
   {
      
       public abstract void SetItemFromDb( ref  SqlDataReader reader);
       
       public abstract DbObject CreateInstance();


       public  virtual void SetFromDbByReflection(ref SqlDataReader reader)
       {
           
       }

   }
}
