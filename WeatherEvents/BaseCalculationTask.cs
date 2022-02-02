using System;
using System.Diagnostics;

namespace WeatherEvents
{
     public abstract class BaseCalculationTask<TRaw> : ICalculationTask
    {
        public Response Calculate(Request request)
        {
            var stopwatch = Stopwatch.StartNew();
            TRaw result = MakeCalculations(request);
            stopwatch.Stop();
            return FormatResponse(result, stopwatch.Elapsed);
        }

        protected abstract TRaw MakeCalculations(Request request);

        protected abstract Response FormatResponse(TRaw result, TimeSpan elapsed);
    }
}