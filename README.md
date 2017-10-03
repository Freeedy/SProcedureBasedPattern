# SProcedureBasedPattern

This project is useful for developers who use MS SQL as their DataBase.
If during the work you need to execute a lot of Stored Procedures then possibly you need convenient tool for that.

## Overview
By means of this library you can get the list of all Stored Procedures from DataBase.
If you want execute one of them you should Load it and provide input parameters. After that SP is executed. In the context of OutPut, SP can be executed in 4 different ways:  return set of rows, return value, return nothing (ExecuteNonQuery), or you can MAP returned object to a class.

## Usage

Just add SPBP.DLL to your project. It may be Console, Web or Desktop application.

## Code example
Here is explanation of Console application code.

First of all you need to declare a DbAgent variable:
`DbAgent agent = new DbAgent(<name>, <connection string>);`

Then get the all stored procedures existing in that DataBase:

`ProcedureFactory fact = SqlManager.GetProceduresFactory(agent);`

Select the  Procedure:

`DataSItem procedure = fact.Procedures["procedurename"];`

Set values to parameters:
```
foreach (DataParam param in procedure.Params.Values)
            {
                Console.Write(param.Name+ " = ");
                param.Value = Console.ReadLine();
            }
```

Execute it in one of four ways:
```
IBag<Item> items;
ExecResult result = procedure.ExecuteDataReaderByRef(agent, out items);
```

OR
```
DataSet set = new DataSet();
procedure.ExecDataSet(agent, out set);
```


## History

DLL added


## License

Copyright 2017 Freeedy
Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
[](http://www.apache.org/licenses/LICENSE-2.0)
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

