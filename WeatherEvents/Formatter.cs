using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherEvents
{
    public static class Formatter
    {
        public static string Task0(int count)
        {
            return $"Task 0:\nTotal in 2018: {count} events\n";
        }

        public static string Task1((int StateCount, int CityCount) tuple)
        {
            (int stateCount, int cityCount) = tuple;
            return $"Task 1:\nTotal {stateCount} states, {cityCount} cities\n";
        }

        public static string Task2(IEnumerable<(string City, int RainCount)> data)
        {
            return "Task 2:\nTop most rainy cities in 2019:\n" + string.Join("",
                data.Select((tuple, i) => $"{i + 1}. City {tuple.City}: total {tuple.RainCount} rainy events\n"));
        }

        public static string Task3(IEnumerable<(int Year, DateTime StartTime, DateTime EndTime, string City)> data)
        {
            return "Task 3:\nThe longest snows by year:\n" + string.Join("",
                data.Select(tuple => $"{tuple.Year}: From {tuple.StartTime} to {tuple.EndTime} in {tuple.City}\n"));
        }

        public static string Task4(IEnumerable<(string State, int EventCount)> data)
        {
            return "Task 4:\nCount of events before longer than 2 hours in 2019 by state:\n" + string.Join("",
                data.OrderBy(tuple => tuple.State)
                    .Select(tuple => $"{tuple.State}: {tuple.EventCount} events\n"));
        }
        
        public static string Task5(IEnumerable<(string State, string City, double TotalHours)> data)
        {
            return "Task 5:\nThe most severe (total length of severe events) city in 2017 by state:\n" + string.Join("",
                data.OrderBy(tuple => tuple.State)
                    .Select(tuple => $"{tuple.State}: City {tuple.City}: total {tuple.TotalHours} hours\n"));
        }
        
        public static string Task6(
            IEnumerable<(int Year, WeatherEventType MostFrequentType, double MostFrequentAverage, WeatherEventType
                LeastFrequentType, double LeastFrequentAverage)> data)
        {
            return "Task 6:\nThe most and least frequent event by year:\n" + string.Join("",
                data.Select(tuple =>
                    $"{tuple.Year}: Most frequent {tuple.MostFrequentType}, average duration: {tuple.MostFrequentAverage} hours\n" +
                    $"      Least frequent {tuple.LeastFrequentType}, average duration: {tuple.LeastFrequentAverage} hours\n"));
        }
    }
}