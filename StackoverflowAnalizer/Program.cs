using System;
using System.Threading;
using System.Threading.Tasks;

namespace StackoverflowAnalizer
{
    public class PageProcessor:IPageProcessor
    {
        public void ProcessPage(string url, string html)
        {
            
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            new StackoverflowWalker(new PageProcessor()).Walk().ContinueWith(n => Console.WriteLine("end")).Wait();
            //Task test = Test();
            //test.Wait();
            Console.ReadKey();
        }

        //public static async Task Test()
        //{
        //    for (int i = 0; i < 4; i++)
        //    {
        //        var task = new Task(() =>
        //        {
        //            Thread.Sleep(1000);
        //            Console.WriteLine("end"+i);
        //        });
        //        task.Start();
        //        await task;
        //    }
        //}
    }
}
