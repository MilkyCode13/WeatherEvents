namespace WeatherEvents
{
    public interface ICalculationTask
    {
        public Response Calculate(Request request);
    }
}