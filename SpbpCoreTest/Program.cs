using SPBP;
using SPBP.Connector;
using SPBP.Connector.Abstract;
using SPBP.Connector.Attributes;
using SPBP.Connector.Class;
using SPBP.Handling;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SpbpCoreTest
{
    [DbObject]
    public class Img
    {
        [ColumnName()]
        public int Id { get; set; }

        [ColumnName]
        public string Path { get; set; }



    }


    public class ImgInh : DbObject
    {
        public int Id { get; set; }

        public string Path { get; set; }

        public override void SetItemFromDb(ref SqlDataReader reader)
        {
            Id = Convert.ToInt32(reader[0].ToString());
            Path = reader[1].ToString();
        }


        public override DbObject CreateInstance()
        {
            return new ImgInh();
        }
    }

    class Program
    {
        #region  Methods 

        public static async Task RunLimitedNumberAtATime<T>(int numberOfTasksConcurrent, IEnumerable<T> inputList, Func<T, Task> asyncFunc)
        {
            Queue<T> inputQueue = new Queue<T>(inputList);
            List<Task> runningTasks = new List<Task>(numberOfTasksConcurrent);
            for (int i = 0; i < numberOfTasksConcurrent && inputQueue.Count > 0; i++)
                runningTasks.Add(asyncFunc(inputQueue.Dequeue()));

            while (inputQueue.Count > 0)
            {
                Task task = await Task.WhenAny(runningTasks);
                runningTasks.Remove(task);
                runningTasks.Add(asyncFunc(inputQueue.Dequeue()));
            }

            await Task.WhenAll(runningTasks);
        }

        /// <summary>
        /// Concurrency : 100 - RunCount : 100 - DurationOfOneRun : 100
        //Execution Time : 1309

        //  Concurrency : 10 - RunCount : 100 - DurationOfOneRun : 100
        //Execution Time : 1491

        //Concurrency : 10 - RunCount : 1000 - DurationOfOneRun : 100
        //Execution Time : 13339

        //Concurrency : 100 - RunCount : 1000 - DurationOfOneRun : 100
        //Execution Time : 13498


        //  Concurrency : 100 - RunCount : 5000 - DurationOfOneRun : 1000
        //Execution Time : 94211

        //Concurrency : 500 - RunCount : 5000 - DurationOfOneRun : 1000
        //Execution Time : 98116

        /// </summary>
        static async void TEst()
        {
            int concurrencyLevel = 50;
            int runcount = 100;
            int methodWait = 10000;
            Console.WriteLine(string.Format("Concurrency : {0} - RunCount : {1} - DurationOfOneRun : {2}", concurrencyLevel, runcount, methodWait));
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Task task = RunLimitedNumberAtATime(concurrencyLevel, Enumerable.Range(1, runcount), async x =>
                Task.Factory.StartNew(() =>
                {

                    //  Console.WriteLine(string.Format("Starting task {0}", x));
                    // await Task.Delay(1000);
                    //Thread.Sleep(methodWait);
                    //  Console.WriteLine(string.Format("Finishing task {0}", x));
                }, TaskCreationOptions.LongRunning));
            await task;
            sw.Stop();
            Console.WriteLine(string.Format("Execution Time : {0}", sw.ElapsedMilliseconds));
        }

        static async Task InheritanceGEtAsync(DbAgent agent)
        {

            DataSItem itm = new DataSItem();
            itm.Schema = "general";

            itm.Name = "Images_Get";
            // itm.AddParam(new DataParam("@path", CustomSqlTypes.String));
            itm.AddReturnParam(CustomSqlTypes.Int);

            ExecAsyncResult res = await itm.ExecDataReadByInheritanceAsync<ImgInh>(agent);
            IBag<ImgInh> images = res.Object as IBag<ImgInh>;

            Console.WriteLine("Item Count : " + images.Objects.Count.ToString() + "  " + res.ToString());
        }


        static async Task ReflectionGetAsync(DbAgent agent)
        {

            DataSItem itm = new DataSItem();
            itm.Schema = "general";

            itm.Name = "Images_Get";
            // itm.AddParam(new DataParam("@path", CustomSqlTypes.String));
            itm.AddReturnParam(CustomSqlTypes.Int);

            ExecAsyncResult res = await itm.ExecuteDataReaderByRefAsync<Img>(agent);
            IBag<Img> images = res.Object as IBag<Img>;

            Console.WriteLine("Item Count : " + images.Objects.Count.ToString() + "  " + res.ToString());
        }

        static async Task ImagesGigImagesGET(int count, DbAgent agent)
        {
            DataSItem itm = new DataSItem();
            itm.Schema = "general";

            itm.Name = "Images_Get";
            // itm.AddParam(new DataParam("@path", CustomSqlTypes.String));
            itm.AddReturnParam(CustomSqlTypes.Int);
            //  itm.FillPRocedureParamsFromSQLAgent(agent);
            // itm.Params.Add("@path",new DataParam());

            Task compleateT = RunLimitedNumberAtATime(10, Enumerable.Range(1, count), async x =>
                Task.Factory.StartNew(async () =>
                {
                    Console.WriteLine("Start! ");
                    // itm.Params["@path"].Value = Guid.NewGuid().ToString();
                    ExecAsyncResult res = await itm.ExecuteDataReaderByRefAsync<Img>(agent);
                    IBag<Img> images = res.Object as IBag<Img>;
                    Console.WriteLine("Item Count : " + images.Objects.Count.ToString() + "  " + res.ToString());
                }, TaskCreationOptions.LongRunning));

            await compleateT;
        }

        static async void NewImagesViaAgent(DbAgent agent )
        {
            DataSItem itm = new DataSItem
            {
                //  itm.Schema = "general";
                Schema = "dbo",
                Name = "Images_NewINstance"
            };
            itm.AddParam(new DataParam("@path", CustomSqlTypes.String));
            itm.AddReturnParam(CustomSqlTypes.Int);

            itm.Params["@path"].Value = Guid.NewGuid().ToString();

            await agent.OpenConnectionAsync();

            ExecAsyncResult res = await itm.ExecuteNonQueryAsync(agent);
            Console.WriteLine(res.ToString());

             res = await itm.ExecuteNonQueryAsync(agent);
            Console.WriteLine(res.ToString());
            agent.Dispose(); 
            

        }

        static void NewImagesViaAgentSync(DbAgent agent )
        {
            DataSItem itm = new DataSItem
            {
                //  itm.Schema = "general";
                Schema = "general",
                Name = "Images_NewINstance"
            };
            itm.AddParam(new DataParam("@path", CustomSqlTypes.String));
            itm.AddReturnParam(CustomSqlTypes.Int);

            itm.Params["@path"].Value = Guid.NewGuid().ToString();

             agent.OpenConnection();

            ExecResult res =  itm.ExecuteNonQuery(agent);
            Console.WriteLine(res.ToString());

            res = itm.ExecuteNonQuery(agent);
            Console.WriteLine(res.ToString());
            agent.Dispose();
        }

        static async void NewImages(int count, DbAgent agent)
        {
            //Images_NewINstance

            DataSItem itm = new DataSItem();
            //  itm.Schema = "general";
            itm.Schema = "dbo";
            itm.Name = "Images_NewINstance";
            itm.AddParam(new DataParam("@path", CustomSqlTypes.String));
            itm.AddReturnParam(CustomSqlTypes.Int);
            //  itm.FillPRocedureParamsFromSQLAgent(agent);
            // itm.Params.Add("@path",new DataParam());

            Task compleateT = RunLimitedNumberAtATime(10, Enumerable.Range(1, count), async x =>
                Task.Factory.StartNew(async () =>
                {
                    itm.Params["@path"].Value = Guid.NewGuid().ToString();
                    ExecAsyncResult res = await itm.ExecuteNonQueryAsync(agent);
                    Console.WriteLine(res.ToString());
                    //  Console.WriteLine(string.Format("Starting task {0}", x));
                    // await Task.Delay(1000);
                    //Thread.Sleep(methodWait);
                    //  Console.WriteLine(string.Format("Finishing task {0}", x));
                }, TaskCreationOptions.LongRunning));

            await compleateT;








        }




        static void GetImgsync(DbAgent agent)
        {
            DataSItem itm = new DataSItem();
            itm.Schema = "general";

            itm.Name = "Images_Get";
            // itm.AddParam(new DataParam("@path", CustomSqlTypes.String));
            itm.AddReturnParam(CustomSqlTypes.Int);
            IBag<Img> images;

            ExecResult res = itm.ExecuteDataReaderByRef(agent, out images);


            foreach (Img img in images.Objects)
            {
                Console.WriteLine(string.Format("Id : {0} - Path : {1}", img.Id.ToString(), img.Path));
            }


            Console.WriteLine("Item Count : " + images.Objects.Count.ToString() + "  " + res.ToString());


        }

        static async void ImagesGet(int count, DbAgent agent)
        {
            //Images_NewINstance

            DataSItem itm = new DataSItem();
            itm.Schema = "general";

            itm.Name = "Images_Get";
            // itm.AddParam(new DataParam("@path", CustomSqlTypes.String));
            itm.AddReturnParam(CustomSqlTypes.Int);
            //  itm.FillPRocedureParamsFromSQLAgent(agent);
            // itm.Params.Add("@path",new DataParam());

            Task compleateT = RunLimitedNumberAtATime(10, Enumerable.Range(1, count), async x =>
                Task.Factory.StartNew(async () =>
                {
                    // itm.Params["@path"].Value = Guid.NewGuid().ToString();
                    ExecAsyncResult res = await itm.ExecuteDataReaderByRefAsync<Img>(agent);
                    IBag<Img> images = res.Object as IBag<Img>;


                    Console.WriteLine("Item Count : " + images.Objects.Count.ToString() + "  " + res.ToString());
                    //  Console.WriteLine(string.Format("Starting task {0}", x));
                    // await Task.Delay(1000);
                    //Thread.Sleep(methodWait);
                    //  Console.WriteLine(string.Format("Finishing task {0}", x));
                }, TaskCreationOptions.LongRunning));

            await compleateT;
        }


        static void ModelsGET(DbAgent agent)
        {
            DataSItem _selectedProcedure = new DataSItem();
            _selectedProcedure.Schema = "auto";
            _selectedProcedure.Name = "Models_Get";

            DataSet ds = new DataSet();

            //  _selectedProcedure.FillPRocedureParamsFromSQLAgent(agent); 
            _selectedProcedure.ExecDataSet(agent, out ds);

            DataTable dt = ds.Tables[0];


            foreach (DataRow rw in dt.Rows)
            {
                foreach (var s in rw.ItemArray)
                {
                    Console.Write(s.ToString() + " - ");
                }
                Console.WriteLine();
                // Console.WriteLine(rw[0].ToString());

            }
        }

        #endregion






        static void Main(string[] args)
        {
         
            DbAgent agent = new DbAgent("marsDb", "Data Source=FREEDY;Initial Catalog=Mars_db;Integrated Security=True",
                                       true ,ConnectionLevel.AllInOne);

            //  DbAgent mrAgent = new DbAgent("Mircelal",
            //                            @"Data Source=MIRJALAL\SQLEXPRESS;Initial Catalog=payment;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False", true);

            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Start !");
            sw.Start();

            // NewImages(100,agent);

            //   NewImages(1000000 ,mrAgent);

            // ImagesGet(2, agent);

            // GetImgsync(agent);
            //Item Count : 158328  [Code : 1 - Execution Time :  356 ms  ] - Type : ExecByRef
            //Item Count : 158328  [Code : 1 - Execution Time :  977 ms  ] - Type : ExecByINheritance
            //  ReflectionGetAsync(agent).Wait();
            // InheritanceGEtAsync(agent).Wait();

            // ImagesGigImagesGET(2, agent).Wait();

            //            NewImagesViaAgent(agent); 
            NewImagesViaAgentSync(agent);
            sw.Stop();

            Console.WriteLine("Compleated : " + sw.ElapsedMilliseconds.ToString());
            Console.ReadKey();

        }
    }
}
