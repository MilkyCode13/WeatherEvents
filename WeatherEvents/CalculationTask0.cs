using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherEvents
{
    public class CalculationTask0 : BaseCalculationTask<int>
    {
        protected override int MakeCalculations(Request request)
        {
            IReadOnlyList<WeatherEvent> weatherEvents = request.Payload;
            
            return weatherEvents.AsParallel()
                .Count(e => e.StartTimeUtc.Year is 2018);
        }

        protected override Response FormatResponse(int result, TimeSpan elapsed)
        {
            return new Response(Formatter.Task0(result), elapsed);
        }
    }
}