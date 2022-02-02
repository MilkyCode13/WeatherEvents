using System;

namespace WeatherEvents
{
    public record Response(string Payload, TimeSpan Elapsed);
}