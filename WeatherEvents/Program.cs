using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WeatherEvents
{
    internal static class Program
    {
        private const string CsvFilePath = "WeatherEvents_Jan2016-Dec2020.csv";

        private static void Main()
        {
            Request request = ImportVerbose(out TimeSpan importElapsed);

            var calculationTasks = new List<Func<Request, Response>>
            {
                new CalculationTask0().Calculate,
                new CalculationTask1().Calculate,
                new CalculationTask2().Calculate,
                new CalculationTask3().Calculate,
                new CalculationTask4().Calculate,
                new CalculationTask5().Calculate,
                new CalculationTask6().Calculate,
            };

            TaskTimers sequentialElapsed = RunSequentialVerbose(request, calculationTasks);

            TaskTimers parallelElapsed = RunParallelVerbose(request, calculationTasks);

            TaskTimers tasksElapsed = RunTasksVerbose(request, calculationTasks);

            ShowRecap(sequentialElapsed, parallelElapsed, tasksElapsed, importElapsed);
        }

        private static void ShowRecap(TaskTimers sequentialElapsed, TaskTimers parallelElapsed, TaskTimers tasksElapsed,
            TimeSpan importElapsed)
        {
            ShowBasicRecap(sequentialElapsed, parallelElapsed, tasksElapsed, importElapsed);

            int taskCount = sequentialElapsed.ElapsedTasks.Length;
            
            DisplayHeader(taskCount);

            TaskTimers worst = GetWorst(sequentialElapsed, parallelElapsed, tasksElapsed, taskCount);

            DisplayRow("Sequential", sequentialElapsed, worst);
            DisplayRow("Parallel", parallelElapsed, worst);
            DisplayRow("Tasks", tasksElapsed, worst);
        }

        private static TaskTimers GetWorst(TaskTimers sequentialElapsed, TaskTimers parallelElapsed, TaskTimers tasksElapsed,
            int taskCount)
        {
            TimeSpan worstTotal = new[]
                {sequentialElapsed.ElapsedTotal, parallelElapsed.ElapsedTotal, tasksElapsed.ElapsedTotal}.Max();
            TimeSpan[] worstByTask = Enumerable.Range(0, taskCount)
                .Select(i => new[]
                    {
                        sequentialElapsed.ElapsedTasks[i],
                        parallelElapsed.ElapsedTasks[i],
                        tasksElapsed.ElapsedTasks[i]
                    }
                    .Max())
                .ToArray();
            var worst = new TaskTimers(worstTotal, worstByTask);
            return worst;
        }

        private static void DisplayHeader(int taskCount)
        {
            Console.Write("Execution Type    ");
            for (var i = 0; i < taskCount; i++)
            {
                Console.Write($"Task {i}".PadRight(20));
            }

            Console.WriteLine("Total");
            Console.WriteLine(new string('=', 28 + 20 * taskCount));
        }

        private static void ShowBasicRecap(TaskTimers sequentialElapsed, TaskTimers parallelElapsed, TaskTimers tasksElapsed,
            TimeSpan importElapsed)
        {
            Console.WriteLine("Total Recap\n" +
                              "=========================================\n" +
                              $"Reading file: {importElapsed:g}\n" +
                              $"Sequential:   {sequentialElapsed.ElapsedTotal:g}\n" +
                              $"Parallel:     {parallelElapsed.ElapsedTotal:g}\n" +
                              $"Tasks:        {tasksElapsed.ElapsedTotal:g}\n");
        }

        private static void DisplayRow(string name, TaskTimers elapsed, TaskTimers worst)
        {
            Console.Write(name.PadRight(18));
            for (var i = 0; i < elapsed.ElapsedTasks.Length; i++)
            {
                double percent = (worst.ElapsedTasks[i] - elapsed.ElapsedTasks[i]) / worst.ElapsedTasks[i] * 100;
                Console.Write($"{elapsed.ElapsedTasks[i]:ss\\.fff} (-{percent:F2}%)".PadRight(20));
            }
            double totalPercent = (worst.ElapsedTotal - elapsed.ElapsedTotal) / worst.ElapsedTotal * 100;
            Console.WriteLine($"{elapsed.ElapsedTotal:ss\\.fff} (-{totalPercent:F2}%)".PadRight(20));
        }

        private static TaskTimers RunTasksVerbose(Request request, List<Func<Request, Response>> calculationTasks)
        {
            Console.WriteLine("Tasks execution (Tasks.Run):\n" +
                              "======================================\n");
            TaskTimers result = RunStopwatch(RunWrapper.RunTasks, request, calculationTasks);
            Console.WriteLine($"Tasks finished, elapsed {result.ElapsedTotal}\n");
            return result;
        }

        private static TaskTimers RunParallelVerbose(Request request, List<Func<Request, Response>> calculationTasks)
        {
            Console.WriteLine("Parallel execution (Parallel.ForEach):\n" +
                              "======================================\n");
            TaskTimers result = RunStopwatch(RunWrapper.RunParallel, request, calculationTasks);
            Console.WriteLine($"Parallel finished, elapsed {result.ElapsedTotal}\n");
            return result;
        }

        private static TaskTimers RunSequentialVerbose(Request request, List<Func<Request, Response>> calculationTasks)
        {
            Console.WriteLine("Sequential execution (foreach loop):\n" +
                              "====================================\n");
            TaskTimers result = RunStopwatch(RunWrapper.RunSequential, request, calculationTasks);
            Console.WriteLine($"Sequential finished, elapsed {result.ElapsedTotal}\n");
            return result;
        }

        private static TaskTimers RunStopwatch(Func<Request, List<Func<Request, Response>>, TimeSpan[]> runner, Request request,
            List<Func<Request, Response>> calculationTasks)
        {
            var stopwatch = Stopwatch.StartNew();
            TimeSpan[] elapsedTasks = runner(request, calculationTasks);
            stopwatch.Stop();
            return new TaskTimers(stopwatch.Elapsed, elapsedTasks);
        }

        private static Request ImportVerbose(out TimeSpan importElapsed)
        {
            Console.WriteLine("Importing from file...");
            var stopwatch = Stopwatch.StartNew();
            IReadOnlyList<WeatherEvent> weatherEvents = ReadWeatherEvents(CsvFilePath);
            var request = new Request(weatherEvents);
            stopwatch.Stop();
            importElapsed = stopwatch.Elapsed;
            Console.WriteLine($"Import finished, elapsed {importElapsed}\n");
            return request;
        }

        private static List<WeatherEvent> ReadWeatherEvents(string filename)
        {
            var weatherEvents = new List<WeatherEvent>();

            using var reader = new StreamReader(filename);
            
            reader.ReadLine();
            string? record;
            while ((record = reader.ReadLine()) != null)
            {
                weatherEvents.Add(WeatherEvent.Parse(record));
            }
            
            return weatherEvents;
        }

        private static void DisplayWeatherEvents(IEnumerable<WeatherEvent> weatherEvents)
        {
            foreach (WeatherEvent weatherEvent in weatherEvents)
            {
                Console.WriteLine(weatherEvent);
            }
        }

        private record TaskTimers(TimeSpan ElapsedTotal, TimeSpan[] ElapsedTasks);
    }
}