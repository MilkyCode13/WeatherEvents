using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherEvents
{
    public class CalculationTask4 : BaseCalculationTask<IReadOnlyList<(string State, int EventCount)>>
    {
        protected override IReadOnlyList<(string State, int EventCount)> MakeCalculations(Request request)
        {
            IReadOnlyList<WeatherEvent> weatherEvents = request.Payload;
            
            TimeSpan limit = TimeSpan.FromHours(2);
            return weatherEvents.AsParallel()
                .Where(e => e.StartTimeUtc.Year is 2019)
                .GroupBy(e => e.State)
                .Select(g =>
                    (g.Key, g.OrderBy(e =>
                        e.StartTimeUtc).TakeWhile(e => e.EndTimeUtc - e.StartTimeUtc <= limit).Count()))
                .ToList();
        }

        protected override Response FormatResponse(IReadOnlyList<(string State, int EventCount)> result, TimeSpan elapsed)
        {
            return new Response(Formatter.Task4(result, elapsed), elapsed);
        }
    }
}