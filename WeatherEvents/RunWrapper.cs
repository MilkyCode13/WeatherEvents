using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherEvents
{
    public static class RunWrapper
    {
        public static TimeSpan[] RunSequential(Request request, List<Func<Request, Response>> calculationTasks)
        {
            var list = new List<TimeSpan>(7);
            foreach (Func<Request,Response> task in calculationTasks)
            {
                (string? payload, TimeSpan elapsed) = task(request);
                Console.WriteLine(payload);
                list.Add(elapsed);
            }

            return list.ToArray();
        }
        
        public static TimeSpan[] RunParallel(Request request, List<Func<Request, Response>> calculationTasks)
        {
            var array = new TimeSpan[7];
            
            Parallel.ForEach(calculationTasks, (task, state, index) =>
            {
                (string? payload, TimeSpan elapsed) = task(request);
                Console.WriteLine(payload);
                array[index] = elapsed;
            });

            return array;
        }
        
        public static TimeSpan[] RunTasks(Request request, List<Func<Request, Response>> calculationTasks)
        {
            var array = new TimeSpan[7];
            
            Task[] tasks = calculationTasks.Select((func, i) => Task.Factory.StartNew(() =>
            {
                (string? payload, TimeSpan elapsed) = func(request);
                Console.WriteLine(payload);
                array[i] = elapsed;
            })).ToArray();
            Task.WaitAll(tasks);
            
            return array;
        }
    }
}