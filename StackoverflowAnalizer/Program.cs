using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackoverflowAnalizer
{
    class Program
    {
        static void Main(string[] args)
        {
            var downloader = new Downloader();
            var tasks = new []
            {
                downloader.AddToQueue("http://stackoverflow.com/questions/tagged/c%23"),
                downloader.AddToQueue("http://stackoverflow.com/questions/23437760/accessviolationexception-was-unhandled"),    
                downloader.AddToQueue("http://stackoverflow.com/questions/23205731/entity-framework-6-1-code-first-cascading-delete-with-tph-for-one-to-one-relatio"),
                downloader.AddToQueue("http://weblogs.asp.net/manavi/inheritance-mapping-strategies-with-entity-framework-code-first-ctp5-part-3-table-per-concrete-type-tpc-and-choosing-strategy-guidelines")
            };
            Task.WaitAll(tasks);
            foreach (var result in downloader.GetResult())
            {
                Console.WriteLine(result.Key);
            }
        }
    }
}
