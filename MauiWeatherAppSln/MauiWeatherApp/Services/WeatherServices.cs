using MauiWeatherApp.Models;
using MauiWeatherApp.Configuration;
using System.Text.Json;

namespace MauiWeatherApp.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(AppConfig.DefaultTimeout);
        }

        public async Task<WeatherResponse?> GetCurrentWeatherAsync(double latitude, double longitude)
        {
            try
            {
                string url = $"{AppConfig.OpenWeatherBaseUrl}?lat={latitude}&lon={longitude}&appid={AppConfig.OpenWeatherApiKey}&units={AppConfig.DefaultUnits}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<WeatherResponse>(jsonContent);
                }
                else
                {
                    // Handle error response
                    string errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle error
                System.Diagnostics.Debug.WriteLine($"Error fetching weather data: {ex.Message}");
                return null;
            }
        }

        public async Task<WeatherResponse?> GetCurrentWeatherByCityAsync(string cityName)
        {
            try
            {
                string url = $"{AppConfig.OpenWeatherBaseUrl}?q={cityName}&appid={AppConfig.OpenWeatherApiKey}&units={AppConfig.DefaultUnits}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<WeatherResponse>(jsonContent);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching weather data: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}