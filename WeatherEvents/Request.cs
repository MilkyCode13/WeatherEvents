using System.Collections.Generic;

namespace WeatherEvents
{
    public record Request(IReadOnlyList<WeatherEvent> Payload);
}