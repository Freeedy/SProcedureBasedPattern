using SPBP.Connector.Attributes;
using System;

namespace ProcedureExecuter
{
    [DbObject]
    public class NewEmp
    {
        [ColumnName("id")]
        public int Id { get; set; }

        [ColumnName("FullName")]
        public string FullName { get; set; }

        [ColumnName("CreatedDate")]
        public DateTime CDate { get; set; }
        [ColumnName("Role")]
        public string Roles { get; set; }

        [ColumnName("Salary")]
        public decimal Money { get; set; }

        [ColumnName]
        public string Department { get; set; }




        public override string ToString()
        {
            return string.Format("Id : {0} \nName : {1} \nCreatedDate : {2} \nSalary : {3} \nRole : {4} \nDepartment : {5} \n -------------------------"
                                    , Id.ToString(), FullName, CDate.ToShortDateString(),
                                    Money.ToString(), Roles, Department);
        }
    }
}
