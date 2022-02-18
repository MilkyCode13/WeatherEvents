using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherEvents
{
    public class CalculationTask6 : BaseCalculationTask<IReadOnlyList<(int Year, WeatherEventType MostFrequentType, double MostFrequentAverage, WeatherEventType
        LeastFrequentType, double LeastFrequentAverage)>>
    {
        protected override IReadOnlyList<(int Year, WeatherEventType MostFrequentType, double MostFrequentAverage, WeatherEventType
            LeastFrequentType, double LeastFrequentAverage)> MakeCalculations(Request request)
        {
            IReadOnlyList<WeatherEvent> weatherEvents = request.Payload;
            
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

        protected override Response FormatResponse(IReadOnlyList<(int Year, WeatherEventType MostFrequentType, double MostFrequentAverage, WeatherEventType
            LeastFrequentType, double LeastFrequentAverage)> result, TimeSpan elapsed)
        {
            return new Response(Formatter.Task6(result, elapsed), elapsed);
        }
    }
}