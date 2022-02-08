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

            TimeSpan sequentialElapsed = RunSequentialVerbose(request, calculationTasks);

            TimeSpan parallelElapsed = RunParallelVerbose(request, calculationTasks);

            TimeSpan tasksElapsed = RunTasksVerbose(request, calculationTasks);

            ShowRecap(sequentialElapsed, parallelElapsed, tasksElapsed, importElapsed);
        }

        private static void ShowRecap(TimeSpan sequentialElapsed, TimeSpan parallelElapsed, TimeSpan tasksElapsed,
            TimeSpan importElapsed)
        {
            TimeSpan worst = new List<TimeSpan> {sequentialElapsed, parallelElapsed, tasksElapsed}.Max();
            Console.WriteLine("Total Recap\n" +
                              "=========================================\n" +
                              $"Reading file: {importElapsed:g}\n" +
                              $"Sequential:   {sequentialElapsed:g}\n" +
                              $"Parallel:     {parallelElapsed:g}\n" +
                              $"Tasks:        {tasksElapsed:g}\n" +
                              "================================\n" +
                              $"Worst:        {worst:g}\n" +
                              $"Sequential:   -{(worst - sequentialElapsed) / worst * 100:F2}%\n" +
                              $"Parallel:     -{(worst - parallelElapsed) / worst * 100:F2}%\n" +
                              $"Tasks:        -{(worst - tasksElapsed) / worst * 100:F2}%\n");
        }

        private static TimeSpan RunTasksVerbose(Request request, List<Func<Request, Response>> calculationTasks)
        {
            Console.WriteLine("Tasks execution (Parallel.ForEach):\n" +
                              "======================================\n");
            TimeSpan tasksElapsed = RunStopwatch(RunWrapper.RunTasks, request, calculationTasks);
            Console.WriteLine($"Tasks finished, elapsed {tasksElapsed}\n");
            return tasksElapsed;
        }

        private static TimeSpan RunParallelVerbose(Request request, List<Func<Request, Response>> calculationTasks)
        {
            Console.WriteLine("Parallel execution (Parallel.ForEach):\n" +
                              "======================================\n");
            TimeSpan parallelElapsed = RunStopwatch(RunWrapper.RunParallel, request, calculationTasks);
            Console.WriteLine($"Parallel finished, elapsed {parallelElapsed}\n");
            return parallelElapsed;
        }

        private static TimeSpan RunSequentialVerbose(Request request, List<Func<Request, Response>> calculationTasks)
        {
            Console.WriteLine("Sequential execution (foreach loop):\n" +
                              "====================================\n");
            TimeSpan sequentialElapsed = RunStopwatch(RunWrapper.RunSequential, request, calculationTasks);
            Console.WriteLine($"Sequential finished, elapsed {sequentialElapsed}\n");
            return sequentialElapsed;
        }

        private static TimeSpan RunStopwatch(Action<Request, List<Func<Request, Response>>> runner, Request request,
            List<Func<Request, Response>> calculationTasks)
        {
            var stopwatch = Stopwatch.StartNew();
            runner(request, calculationTasks);
            stopwatch.Stop();
            return stopwatch.Elapsed;
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
    }
}