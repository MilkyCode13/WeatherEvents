using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherEvents
{
    public class CalculationTask1 : BaseCalculationTask<(int StateCount, int CityCount)>
    {
        protected override (int StateCount, int CityCount) MakeCalculations(Request request)
        {
            IReadOnlyList<WeatherEvent> weatherEvents = request.Payload;
            
            return (weatherEvents.AsParallel()
                    .Select(e => e.State)
                    .Distinct()
                    .Count(),
                weatherEvents.AsParallel()
                    .Select(e => e.City)
                    .Distinct()
                    .Count());
        }

        protected override Response FormatResponse((int StateCount, int CityCount) result, TimeSpan elapsed)
        {
            return new Response(Formatter.Task1(result, elapsed), elapsed);
        }
    }
}