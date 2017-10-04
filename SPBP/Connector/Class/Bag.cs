using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using SPBP.Connector.Abstract;
using SPBP.Connector.Attributes;
using SPBP.Connector.Exceptions;

namespace SPBP.Connector.Class
{
    public class Bag<T> : IBag<T> where T : DbObject
    {
        private List<T> _objects = new List<T>();

        public List<T> Objects { get { return _objects; } }

        private T _currentField;//= (T) Activator.CreateInstance(typeof(T));

        


        public T this[int index]
        {
            get { return _objects[index]; }
        }

        public void SetFromReader(ref SqlDataReader reader)
        {

            // _currentField .CreateInstance();
            _currentField = (T)Activator.CreateInstance(typeof(T));
            _currentField.SetItemFromDb(ref reader);
            _objects.Add(_currentField);

        }


        public void Clear()
        {
            _objects.Clear();
        }


    }


    public class RefBag<T> : IBag<T>
    {
        private List<T> _objects = new List<T>();

        public List<T> Objects { get { return _objects; } }

        private T _currentField;

        public T this[int index]
        {
            get { return _objects[index]; }
        }

        public void SetFromReader(ref SqlDataReader reader)
        {
            _currentField = (T)Activator.CreateInstance(typeof(T));

            Attribute member = typeof(T).GetCustomAttribute<SPBP.Connector.Attributes.DbObjectAttribute>();

            if (member != null)
            {

                IEnumerable<PropertyInfo> specialProps =
                   typeof(T).GetRuntimeProperties()
                                  .Where(
                                      p =>
                                      p.GetCustomAttributes<SPBP.Connector.Attributes.ColumnNameAttribute>(true).Any());

                foreach (PropertyInfo prop in specialProps)
                {

                    ColumnNameAttribute atr = prop.GetCustomAttribute(typeof(ColumnNameAttribute)) as ColumnNameAttribute;

                    if (atr != null && atr.HasValue)
                    {
                        prop.SetValue(_currentField, Convert.ChangeType(reader[atr.Value], prop.PropertyType));
                    }
                    else
                    {
                        prop.SetValue(_currentField, Convert.ChangeType(reader[prop.Name], prop.PropertyType));
                    }
                }

                _objects.Add(_currentField);

            }
            else
            {
                throw new AttributeMissMatchException();
            }

        }

        public void Clear()
        {
            _objects.Clear();
        }



    }


    public interface IBag<T>
    {
        List<T> Objects { get; } // list of entire  objects 

        T this[int index] { get; } // idexer 

        void SetFromReader(ref SqlDataReader reader); //set objects node from sql reader 
        
        void Clear(); // clear object list  
    }


}
