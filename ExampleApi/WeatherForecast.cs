using SecurityBlanket.Interfaces;

namespace WebApiExperiment
{
    public class WeatherForecast : ICustomSecurity
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }

        public int AccountId = 0;

        public bool IsVisible(HttpContext context)
        {
            return this.AccountId == context.Session.GetInt32("accountId");
        }
    }
}