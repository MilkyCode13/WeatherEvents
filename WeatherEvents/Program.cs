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
            Console.WriteLine("Importing from file...");
            var stopwatch = Stopwatch.StartNew();
            IReadOnlyList<WeatherEvent> weatherEvents = ReadWeatherEvents(CsvFilePath);
            var request = new Request(weatherEvents);
            stopwatch.Stop();
            TimeSpan importElapsed = stopwatch.Elapsed;
            Console.WriteLine($"Import finished, elapsed {importElapsed}\n");
            
            // return;

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

            Console.WriteLine("Sequential execution (foreach loop):\n" +
                              "====================================\n");
            stopwatch = Stopwatch.StartNew();
            RunWrapper.RunSequential(request, calculationTasks);
            stopwatch.Stop();
            TimeSpan sequentialElapsed = stopwatch.Elapsed;
            Console.WriteLine($"Sequential finished, elapsed {sequentialElapsed}\n");
            
            Console.WriteLine("Parallel execution (Parallel.ForEach):\n" +
                              "======================================\n");
            stopwatch = Stopwatch.StartNew();
            RunWrapper.RunParallel(request, calculationTasks);
            TimeSpan parallelElapsed = stopwatch.Elapsed;
            Console.WriteLine($"Parallel finished, elapsed {parallelElapsed}\n");
            
            Console.WriteLine("Tasks execution (Parallel.ForEach):\n" +
                              "======================================\n");
            stopwatch = Stopwatch.StartNew();
            RunWrapper.RunTasks(request, calculationTasks);
            TimeSpan tasksElapsed = stopwatch.Elapsed;
            Console.WriteLine($"Tasks finished, elapsed {tasksElapsed}\n");

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

        private static List<WeatherEvent> ReadWeatherEvents(string filename)
        {
            var weatherEvents = new List<WeatherEvent>();

            using var reader = new StreamReader(filename);
            
            // reader.ReadLine();
            // string? record;
            // while ((record = reader.ReadLine()) != null)
            // {
            //     weatherEvents.Add(WeatherEvent.Parse(record));
            // }
            //
            // return weatherEvents;

            Stopwatch stopwatch = Stopwatch.StartNew();
            List<string> sList = reader.ReadToEnd()
                .Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Skip(1).ToList();
            stopwatch.Stop();
            Console.WriteLine($"Reading: {stopwatch.Elapsed:g}");
            
            stopwatch.Reset();
            stopwatch.Start();
            weatherEvents = sList.Select(WeatherEvent.Parse).ToList();
            stopwatch.Stop();
            Console.WriteLine($"Parsing: {stopwatch.Elapsed:g}");
            
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