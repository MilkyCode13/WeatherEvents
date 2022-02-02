using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherEvents
{
    public class CalculationTask2 : BaseCalculationTask<IReadOnlyList<(string City, int RainCount)>>
    {
        protected override IReadOnlyList<(string City, int RainCount)> MakeCalculations(Request request)
        {
            IReadOnlyList<WeatherEvent> weatherEvents = request.Payload;
            
            return weatherEvents.AsParallel().AsOrdered()
                .Where(e => e.Type is WeatherEventType.Rain && e.StartTimeUtc.Year is 2019)
                .GroupBy(e => e.City)
                .Select(g => (g.Key, g.Count()))
                .OrderByDescending(tuple => tuple.Item2)
                .Take(3)
                .ToList();
        }

        protected override Response FormatResponse(IReadOnlyList<(string City, int RainCount)> result, TimeSpan elapsed)
        {
            return new Response(Formatter.Task2(result), elapsed);
        }
    }
}