using SPBP;
using SPBP.Connector;
using SPBP.Connector.Abstract;
using SPBP.Connector.Attributes;

using SPBP.Handling;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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

        static async Task NewImagesViaAgent(DbAgent agent )
        {
            DataSItem itm = new DataSItem
            {
                //  itm.Schema = "general";
                Schema = "general",
                Name = "Images_NewINstance"
            };
            itm.AddParam(new DataParam("@path", CustomSqlTypes.String ,ParamDirection.Input, Guid.NewGuid().ToString()));
            itm.AddReturnParam(CustomSqlTypes.Int);

           // itm.Params["@path"].Value = Guid.NewGuid().ToString();

            await agent.OpenConnectionAsync();

            ExecAsyncResult res = await itm.ExecuteNonQueryAsync(agent);
            Console.WriteLine(res.ToString());

            itm.Params["@path"].Value = Guid.NewGuid().ToString();
             res = await itm.ExecuteNonQueryAsync(agent);
            Console.WriteLine(res.ToString());
            agent.Dispose(); 
            

        }
        /*
         Start !
First Path  : 558278df-28d0-4bb5-82a1-87d2397569fb
Code : 1 - Execution Time :  224.06 ms
Compleated : 1256
Second Path  : 67e1ed21-3570-4a64-8647-5b030ea03c40
Code : 1 - Execution Time :  8.617 ms
             */
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
            string first = Guid.NewGuid().ToString();
            itm.Params["@path"].Value =first ;
            Console.WriteLine("First Path  : " + first);
            //  agent.OpenConnection();
            //using (agent)
            //{
            // agent.OpenConnection();

          agent.OpenConnection();

            ExecResult res = itm.ExecuteNonQuery(agent);

                Console.WriteLine(res.ToString());
                first = Guid.NewGuid().ToString();
                
               
                    for (int i = 0; i < 25000; i++)
                    {
                        first = Guid.NewGuid().ToString();
                        itm.Params["@path"].Value = first;
                        Console.WriteLine($"ThreadId : {Thread.CurrentThread.ManagedThreadId.ToString()} Path i : {i.ToString()}  : " + first);
                        res = itm.ExecuteNonQuery(agent);
                        Console.WriteLine(res.ToString());
                    }

            bool firstfin = false;
            Thread t = new Thread(new ThreadStart(delegate
           {
               for (int i = 0; i < 25000; i++)
               {
                   first = Guid.NewGuid().ToString();
                   itm.Params["@path"].Value = first;
                   Console.WriteLine($"ThreadId : {Thread.CurrentThread.ManagedThreadId.ToString()} Path i : {i.ToString()}  : " + first);
                   res = itm.ExecuteNonQuery(agent);
                   Console.WriteLine(res.ToString());
               }

               firstfin = true;
           }));

            bool sec = false; 
            Thread t2 = new Thread(new ThreadStart(delegate
            {
                for (int i = 0; i < 25000; i++)
                {
                    first = Guid.NewGuid().ToString();
                    itm.Params["@path"].Value = first;
                    Console.WriteLine($"ThreadId : {Thread.CurrentThread.ManagedThreadId.ToString()} Path i : {i.ToString()}  : " + first);
                    res = itm.ExecuteNonQuery(agent);
                    Console.WriteLine(res.ToString());
                }
                sec = true; 
            }));

            t.Start();
            t2.Start();

            while(!(firstfin && sec))
            {

                if(firstfin && sec)
                {
                    agent.Dispose();
                    break;
                }

            }


            //  agent.CloseConnection();

            //itm.Params["@path"].Value = first;
            //Console.WriteLine("Second Path  : " + first);
            //res = itm.ExecuteNonQuery(agent);
            //Console.WriteLine(res.ToString());
            //}
            //  agent.Dispose();
        }

        static async void NewImages(int count, DbAgent agent)
        {
            //Images_NewINstance

            DataSItem itm = new DataSItem
            {
                //  itm.Schema = "general";
                Schema = "dbo",
                Name = "Images_NewINstance"
            };
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


        static async void ImagesGetAsyncNewTest(DbAgent agent )
        {
            if (agent.ConnectionLevel ==ConnectionLevel.AllInOne)
            {
                await agent.OpenConnectionAsync();
            }

            DataSItem itm = new DataSItem();
            itm.Schema = "general";

            itm.Name = "Images_Get";
            // itm.AddParam(new DataParam("@path", CustomSqlTypes.String));
            itm.AddReturnParam(CustomSqlTypes.Int);
            //  itm.FillPRocedureParamsFromSQLAgent(agent);
            // itm.Params.Add("@path",new DataParam());


            List<Task<ExecAsyncResult>> _alltaskst = new List<Task<ExecAsyncResult>>();

            for (int i = 0; i < 2; i++)
            {
                Task<ExecAsyncResult> t = itm.ExecuteDataReaderByRefAsync<Img>(agent);
                _alltaskst.Add(t); 
            }



            while(_alltaskst.Count>0)
            {
                Task<ExecAsyncResult> fintask = await Task.WhenAny(_alltaskst);
                _alltaskst.Remove(fintask);

                ExecAsyncResult result =  await fintask;
                IBag<Img> images = result.Object as IBag<Img>;
                Console.WriteLine("Item Count : " + images.Objects.Count.ToString() + "  " + result.ToString());

            }
           
                    // itm.Params["@path"].Value = Guid.NewGuid().ToString();
                   
                    //  Console.WriteLine(string.Format("Starting task {0}", x));
                    // await Task.Delay(1000);
                    //Thread.Sleep(methodWait);
                    //  Console.WriteLine(string.Format("Finishing task {0}", x));
              
            if (agent.ConnectionLevel==ConnectionLevel.AllInOne)
            {
                agent.Dispose();
            }
            
        }

        static async Task ImagesGetAsyncMUltiple(DbAgent agent )
        {
            if (agent.ConnectionLevel == ConnectionLevel.AllInOne)
            {
                await agent.OpenConnectionAsync();
            }

            DataSItem itm = new DataSItem();
            itm.Schema = "general";

            itm.Name = "Images_Get";
            // itm.AddParam(new DataParam("@path", CustomSqlTypes.String));
            itm.AddReturnParam(CustomSqlTypes.Int);
            //  itm.FillPRocedureParamsFromSQLAgent(agent);
            // itm.Params.Add("@path",new DataParam());


            

            for (int i = 0; i < 1000; i++)
            {
                ExecAsyncResult res1 = await itm.ExecuteDataReaderByRefAsync<Img>(agent);
                IBag<Img> images = res1.Object as IBag<Img>;
                Console.WriteLine("Item Count : " + images.Objects.Count.ToString() + "  " + res1.ToString());


            }




            if (agent.ConnectionLevel == ConnectionLevel.AllInOne)
            {
                agent.Dispose();
            }
        }

        static void ModelsGET(DbAgent agent)
        {
            DataSItem _selectedProcedure = new DataSItem();
            _selectedProcedure.Schema = "auto";
            _selectedProcedure.Name = "Models_Get";

            DataSet ds = new DataSet();

            //  _selectedProcedure.FillPRocedureParamsFromSQLAgent(agent); 
           ExecResult res =  _selectedProcedure.ExecDataSet(agent, out ds);

           
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



        static void TestAgentTisposing(DbAgent agent )
        {
            using (agent)
            {

            }
            GC.Collect();
            SqlConnection con = agent.CreateConnection(); 

        }



        static void Main(string[] args)
        {
         
            DbAgent agent = new DbAgent("marsDb", "Data Source=FREEDY;Initial Catalog=Mars_db;Integrated Security=True;MultipleActiveResultSets=True",
                                       true ,ConnectionLevel.AllInOne);


            DbAgent ag = new DbAgent();
            
           // TestAgentTisposing(agent);
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

            //ImagesGigImagesGET(2, agent).Wait();

            // NewImagesViaAgent(agent).Wait(); 

            // NewImagesViaAgentSync(agent);
           // ImagesGetAsyncNewTest(agent); 

            ImagesGetAsyncMUltiple(agent).Wait();

            sw.Stop();

            Console.WriteLine("Compleated : " + sw.Elapsed.TotalSeconds.ToString());
            Console.ReadKey();

        }
    }
}
