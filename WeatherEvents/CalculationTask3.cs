using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherEvents
{
    public class CalculationTask3 : BaseCalculationTask<IReadOnlyList<(int Year, DateTime StartTime, DateTime EndTime, string City)>>
    {
        protected override IReadOnlyList<(int Year, DateTime StartTime, DateTime EndTime, string City)> MakeCalculations(Request request)
        {
            IReadOnlyList<WeatherEvent> weatherEvents = request.Payload;
            
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

        protected override Response FormatResponse(IReadOnlyList<(int Year, DateTime StartTime, DateTime EndTime, string City)> result, TimeSpan elapsed)
        {
            return new Response(Formatter.Task3(result), elapsed);
        }
    }
}