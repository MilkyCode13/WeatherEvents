using System;
using System.Globalization;

namespace WeatherEvents
{
    public record WeatherEvent(string EventId, WeatherEventType Type, WeatherEventSeverity Severity,
        DateTime StartTimeUtc, DateTime EndTimeUtc, string TimeZone, string AirportCode, double LocationLat,
        double LocationLng, string City, string County, string State, string ZipCode)

    {
        public static WeatherEvent Parse(string record)
        {
            string[] fields = record.Split(',');
            if (fields.Length != 13)
            {
                throw new ArgumentException("Invalid record string format.");
            }

            return new WeatherEvent(
                fields[0],
                Enum.Parse<WeatherEventType>(fields[1], true),
                Enum.Parse<WeatherEventSeverity>(fields[2], true),
                DateTime.Parse(fields[3], CultureInfo.InvariantCulture),
                DateTime.Parse(fields[4], CultureInfo.InvariantCulture),
                fields[5],
                fields[6],
                double.Parse(fields[7]),
                double.Parse(fields[8]),
                fields[9],
                fields[10],
                fields[11],
                fields[12]);
        }
    }
}