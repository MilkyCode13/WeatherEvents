using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherEvents
{
    public class CalculationTask5 : BaseCalculationTask<IReadOnlyList<(string State, string City, double TotalHours)>>
    {
        protected override IReadOnlyList<(string State, string City, double TotalHours)> MakeCalculations(Request request)
        {
            IReadOnlyList<WeatherEvent> weatherEvents = request.Payload;
            
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

        protected override Response FormatResponse(IReadOnlyList<(string State, string City, double TotalHours)> result, TimeSpan elapsed)
        {
            return new Response(Formatter.Task5(result), elapsed);
        }
    }
}