using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherEvents
{
    public static class Calculator
    {
        public static int Task0(IEnumerable<WeatherEvent> weatherEvents)
        {
            return weatherEvents.AsParallel()
                .Count(e => e.StartTimeUtc.Year is 2018);
        }

        public static (int StateCount, int CityCount) Task1(IReadOnlyList<WeatherEvent> weatherEvents)
        {
            return (weatherEvents.AsParallel()
                    .Select(e => e.State)
                    .Distinct()
                    .Count(),
                weatherEvents.AsParallel()
                    .Select(e => e.City)
                    .Distinct()
                    .Count());
        }

        public static IReadOnlyList<(string City, int RainCount)> Task2(IEnumerable<WeatherEvent> weatherEvents)
        {
            return weatherEvents.AsParallel().AsOrdered()
                .Where(e => e.Type is WeatherEventType.Rain && e.StartTimeUtc.Year is 2019)
                .GroupBy(e => e.City)
                .Select(g => (g.Key, g.Count()))
                .OrderByDescending(tuple => tuple.Item2)
                .Take(3)
                .ToList();
        }

        public static IReadOnlyList<(int Year, DateTime StartTime, DateTime EndTime, string City)> Task3(IEnumerable<WeatherEvent> weatherEvents)
        {
            return weatherEvents.AsParallel()
                .Where(e => e.Type is WeatherEventType.Snow)
                .GroupBy(e => e.StartTimeUtc.Year)
                .Select(g =>
                {
                    WeatherEvent longestSnow = g.OrderByDescending(e => e.EndTimeUtc - e.StartTimeUtc).First();
                    return (g.Key, longestSnow.StartTimeUtc, longestSnow.EndTimeUtc, longestSnow.City);
                })
                .ToList();
        }
        
        public static IReadOnlyList<(string State, int EventCount)> Task4(IEnumerable<WeatherEvent> weatherEvents)
        {
            TimeSpan limit = TimeSpan.FromHours(2);
            return weatherEvents.AsParallel()
                .Where(e => e.StartTimeUtc.Year is 2019)
                .GroupBy(e => e.State)
                .Select(g =>
                    (g.Key, g.OrderBy(e =>
                        e.StartTimeUtc).TakeWhile(e => e.EndTimeUtc - e.StartTimeUtc <= limit).Count()))
                .ToList();
        }
        
        public static IReadOnlyList<(string State, string City, double TotalHours)> Task5(IEnumerable<WeatherEvent> weatherEvents)
        {
            return weatherEvents.AsParallel()
                .Where(e => e.Severity is WeatherEventSeverity.Severe && e.StartTimeUtc.Year is 2017)
                .GroupBy(e => e.State)
                .Select(g =>
                {
                    (string city, double totalHours) = g.GroupBy(e => e.City)
                        .Select(g2 =>
                            (g2.Key, g2.Sum(e => (e.EndTimeUtc - e.StartTimeUtc).TotalHours)))
                        .OrderByDescending(tuple => tuple.Item2)
                        .First();
                    
                    return (g.Key, city, totalHours);
                })
                .ToList();
        }
        
        public static
            IReadOnlyList<(int Year, WeatherEventType MostFrequentType, double MostFrequentAverage, WeatherEventType
                LeastFrequentType, double LeastFrequentAverage)> Task6(IEnumerable<WeatherEvent> weatherEvents)
        {
            return weatherEvents.AsParallel()
                .GroupBy(e => e.StartTimeUtc.Year)
                .Select(g =>
                {
                    List<IGrouping<WeatherEventType, WeatherEvent>> groups = g.GroupBy(e => e.Type).ToList();
                    IGrouping<WeatherEventType, WeatherEvent> mostFrequent = groups
                        .OrderByDescending(g2 => g2.Count()).First();
                    IGrouping<WeatherEventType, WeatherEvent> leastFrequent = groups
                        .OrderBy(g2 => g2.Count()).First();

                    return (g.Key,
                        mostFrequent.Key, mostFrequent.Average(e => (e.EndTimeUtc - e.StartTimeUtc).TotalHours),
                        leastFrequent.Key, leastFrequent.Average(e => (e.EndTimeUtc - e.StartTimeUtc).TotalHours));
                })
                .ToList();
        }
    }
}