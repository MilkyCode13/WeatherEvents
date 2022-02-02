using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherEvents
{
    public class RunWrapper
    {
        public static void RunSequential(Request request, List<Func<Request, Response>> calculationTasks)
        {
            foreach (Func<Request,Response> task in calculationTasks)
            {
                Response response = task(request);
                Console.WriteLine(response.Payload);
            }
        }
        
        public static void RunParallel(Request request, List<Func<Request, Response>> calculationTasks)
        {
            Parallel.ForEach(calculationTasks, task =>
            {
                Response response = task(request);
                Console.WriteLine(response.Payload);
            });
        }
        
        public static void RunTasks(Request request, List<Func<Request, Response>> calculationTasks)
        {
            Task[] tasks = calculationTasks.Select(func => Task.Factory.StartNew(() =>
            {
                Response response = func(request);
                Console.WriteLine(response.Payload);
            })).ToArray();
            Task.WaitAll(tasks);
        }
    }
}