namespace MauiWeatherApp.Configuration
{
    public static class AppConfig
    {
        public const string OpenWeatherApiKey = "62fcdce4827a56cf9ee998f0e1a6a3ba";

        // OpenWeather API endpoints
        public const string OpenWeatherBaseUrl = "https://api.openweathermap.org/data/2.5/weather";
        public const string WeatherIconBaseUrl = "https://openweathermap.org/img/wn/";

        // Default settings
        public const string DefaultUnits = "metric"; // metric, imperial, or kelvin
        public const int DefaultTimeout = 10; // seconds
    }
}