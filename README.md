# SProcedureBasedPattern

This project is useful for developers who use MS SQL as their DataBase.
If during the work you need to execute a lot of Stored Procedures then possibly you need convenient tool for that.

## Overview
By means of this library you can get the list of all Stored Procedures from DataBase.
If you want execute one of them you should Load it and provide input parameters. After that SP is executed. In the context of OutPut, SP can be executed in 4 different ways:  return set of rows, return value, return nothing (ExecuteNonQuery), or you can MAP returned object to a class.

## Usage

Just add SPBP.DLL to your project. It may be Console, Web or Desktop application.

## Using example
Here is explanation of Console application code.

First of all you need to declare a DbAgent variable:
`DbAgent agent = new DbAgent(<name>, <connection string>.<state>);`
(If agent state  isn't true  it will throw an exception in  procedure execution . )

Then get the all stored procedures existing in that DataBase:

`ProcedureFactory fact = SqlManager.GetProceduresFactory(agent);`

Select the  Procedure:

`DataSItem procedure = fact.Procedures["procedurename"];`

Set values to parameters if procedure  needs :
```
  //fill params  values 
                    foreach (DataParam param in procedure.Params.Values)
                    {
                        if (param.Direction == ParamDirection.Input)
                        {
                            Console.Write(param.Name + " : ");
                            param.Value = Console.ReadLine();
                        }

                    }
```
Get output params after execution  :
If procedure has output params  it will  be  added  to DsItem Output params . 
``` 
 if (item.HasOutputParam)
                    {
                        foreach (DataParam prm in item.OutputParams.Values)
                        {
                            Console.WriteLine(prm.Name + " = " + prm.Value);
                        }
                    }
```
Get marked class collection  : 
```
IBag<Item> items;
ExecResult result = procedure.ExecuteDataReaderByRef(agent, out items);
```

OR
```
DataSet set = new DataSet();
procedure.ExecDataSet(agent, out set);
```
## Code sample 
Examples of  Procedures (has return value , output params , without rows , etc )
	1 . Add Employee stored procedure .  As we can see from the picture 1  it has  return param and  input params . 
	
***Picture1***
	![addemploye](https://user-images.githubusercontent.com/26925601/31275658-30d51872-aaa9-11e7-999c-4852d43d54e3.png)

2 . Delete Employee .  It  doesn't return any rows  , but has  return parameter .  
	
***Picture 2***

![deleteemployee](https://user-images.githubusercontent.com/26925601/31275768-7a2f7dd2-aaa9-11e7-905f-886c2cb40c76.png)
	3. Test procedure .  Has return  param , output params ,  and return rows 

***Picture 3***

![testproc](https://user-images.githubusercontent.com/26925601/31275819-c8e2e266-aaa9-11e7-861a-ed38abaebcab.png)

#### Execution Sample 

***First Step ***
```
DbAgent agent = new DbAgent("Employees", datasource, true); //create db agent 
ProcedureFactory fact = SqlManager.GetProceduresFactory(agent);  
fact .AddReturnValueToEachProcedure();
/*add return parametr to all procedure (by default the procedures doesn't have  return parameter )*/

```
***Execute AddEmployee ***
``` 
 DataSItem item = fact.Procedures[proc]; //select target procedure (AddEmployee)
 //fill input params  values 
 	 foreach (DataParam param in item.Params.Values)
                    {
                        if (param.Direction == ParamDirection.Input)
                        {
                            Console.Write(param.Name + " : ");
                            param.Value = Console.ReadLine();
                        }

                    }
```
***Execute equiped procedure***
``` 
//execute procedure 
ExecResult res = DBCommander.ExecuteNonQueryProcedure(agent, item);
```
**or**
```
 ExecResult res = item .ExecuteNonQuery(agent); //by the extension method 
```
When  the  procedure  was executed  the **ExecResult** get the return parameter  and  the time of execution . In order to display  you need just use **res.ToString()** method .


**Execute procedure in Picture  3**
Procedure  selection section is the same .

**Execution **

```
DataSet set ;
 ExecResult res = item .ExecDataSet(agent ,out set );
```
And Item willbe  filled with output params . 

## Get List of Marked items 
#### Overview 
   For example  we want to get  list of  marked  objects  that we declared  in our  project . 
#### How to declare marked  class ?
There are 2 way to mark  your class  as DbObject  . 
	1. Inherit from DbOBject  and  override Abstract methods in this .  
	2. Use Custom attributes . (DbObject attribute and  ColumnName)

#### Inheritance Example 
``` 
 public class Employe:DbObject
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedDate { get; set;  }
        public string Role { get; set;  }
        public decimal Salary { get; set;  }
        public string Department { get; set;  }

    public override void SetItemFromDb(ref SqlDataReader reader)
        {
            Id = Convert.ToInt32(reader["id"].ToString() );
            FullName = reader["FullName"].ToString();
            CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString());
            Role = reader["Role"].ToString();
            Salary = Convert.ToDecimal(reader["Salary"].ToString());
            Department = reader["Department"].ToString();

        }

        public override DbObject CreateInstance()
        {
            return new Employe(); 
        }
    }
```
Execution :

``` 
IBag<Employe> employes; //get employes fro inherited class
 ExecResult result = fact.Procedures[proc].ExecDataReadByInheritance(agent,out employes);
```
#### Attributed item example 
```
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
       /* 
		if property will  be attributed like that  The property name (Department)  should be the same as  the  procedure returns .  
		*/
        [ColumnName] 
        public string Department { get; set; }
    }
```
Execution  :

```
IBag<NewEmp> employes;
DataSItem item = fact.Procedures[procname];
ExecResult result = item.ExecuteDataReaderByRef(agent,out employes);
```
## Asynchronous execution

#### Async Result item "ExecAsyncResult"
```
 public class ExecAsyncResult
    {
        public ExecResult Result { get; set; }
        public DataSItem ExecutedProcedure { get; set; } //executed procedure 
        //it can be dictionary ,  data set  , and etc ... 
        public object Object { get; set; }
        private AsyncExecutionType _type = AsyncExecutionType.ExecNonQuery; // by default 
        public AsyncExecutionType ExecutionType { get { return _type; } set { _type = value; } }
        public override string ToString()
        {
            return string .Format("[{0}] - Type : {1}",Result.ToString(),ExecutionType.ToString());
        }

    }
```
Execution Methods:
DataSet
```
ExecAsyncResult res = await _selectedProcedure.ExecDataSetAsync(_currentAgent);
```
Non Query
```
ExecAsyncResult result = await _selectedProcedure.ExecuteNonQueryAsync(_currentAgent);
```
IBag Collection:
```
 ExecAsyncResult result = await _selectedProcedure.ExecuteDataReaderByRefAsync<Employee>(_currentAgent);

 IBag<NewEmp> employes = result.Object as IBag<Employee>;
```
Inheritance mode 

```
ExecAsyncResult result = await _selectedProcedure.ExecDataReadByInheritanceAsync<Employee>(_currentAgent);

 IBag<NewEmp> employes = result.Object as IBag<Employee>;

```

## History

***DLL added***

***Procedure executor added***

***Library update(added  asynchronous  methods  of execution )***



## License

Copyright 2017 Freeedy
Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
[](http://www.apache.org/licenses/LICENSE-2.0)
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

