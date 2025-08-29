using MauiWeatherApp.Models;
using MauiWeatherApp.Services;
using MauiWeatherApp.Configuration;
using System.Globalization;

namespace MauiWeatherApp
{
    public partial class MainPage : ContentPage
    {
        private readonly WeatherService _weatherService;
        private readonly GeolocationService _geolocationService;

        public MainPage()
        {
            InitializeComponent();
            _weatherService = new WeatherService();
            _geolocationService = new GeolocationService();
        }

        private async void OnGetLocationClicked(object? sender, EventArgs e)
        {
            await GetWeatherByLocationAsync();
        }

        private async void OnSearchCityClicked(object? sender, EventArgs e)
        {
            string cityName = CityEntry.Text?.Trim();
            if (string.IsNullOrEmpty(cityName))
            {
                await DisplayAlert("Error", "Please enter a city name", "OK");
                return;
            }

            await GetWeatherByCityAsync(cityName);
        }

        private async Task GetWeatherByLocationAsync()
        {
            ShowLoading(true);
            HideStatusMessage();

            try
            {
                // Check and request location permission
                bool hasPermission = await _geolocationService.CheckAndRequestLocationPermission();

                if (!hasPermission)
                {
                    ShowStatusMessage("Location permission is required to get current weather.");
                    return;
                }

                // Get current location
                var location = await _geolocationService.GetCurrentLocationAsync();

                if (location == null)
                {
                    ShowStatusMessage("Unable to get current location. Please try again or enter a city name.");
                    return;
                }

                // Get weather data
                var weatherData = await _weatherService.GetCurrentWeatherAsync(location.Latitude, location.Longitude);

                if (weatherData != null)
                {
                    DisplayWeatherData(weatherData);
                }
                else
                {
                    ShowStatusMessage("Unable to fetch weather data. Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Error: {ex.Message}");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private async Task GetWeatherByCityAsync(string cityName)
        {
            ShowLoading(true);
            HideStatusMessage();

            try
            {
                var weatherData = await _weatherService.GetCurrentWeatherByCityAsync(cityName);

                if (weatherData != null)
                {
                    DisplayWeatherData(weatherData);
                }
                else
                {
                    ShowStatusMessage("City not found or unable to fetch weather data. Please check the city name and try again.");
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Error: {ex.Message}");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private void DisplayWeatherData(WeatherResponse weather)
        {
            // City and country
            CityLabel.Text = $"{weather.Name}, {weather.Sys?.Country}";

            // Weather description
            if (weather.Weather != null && weather.Weather.Length > 0)
            {
                var weatherInfo = weather.Weather[0];
                WeatherDescription.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(weatherInfo.Description ?? "");
                WeatherMain.Text = weatherInfo.Main;

                // Weather icon
                if (!string.IsNullOrEmpty(weatherInfo.Icon))
                {
                    WeatherIcon.Source = $"{AppConfig.WeatherIconBaseUrl}{weatherInfo.Icon}@2x.png";
                }
            }

            // Temperature
            if (weather.Main != null)
            {
                TemperatureLabel.Text = $"{Math.Round(weather.Main.Temp)}°C";
                FeelsLikeLabel.Text = $"{Math.Round(weather.Main.FeelsLike)}°C";
                HumidityLabel.Text = $"{weather.Main.Humidity}%";
                PressureLabel.Text = $"{weather.Main.Pressure} hPa";
            }

            // Wind
            if (weather.Wind != null)
            {
                WindSpeedLabel.Text = $"{weather.Wind.Speed} m/s";
            }

            // Visibility
            VisibilityLabel.Text = $"{weather.Visibility / 1000.0:F1} km";

            // Show weather frame
            WeatherFrame.IsVisible = true;
        }

        private void ShowLoading(bool isLoading)
        {
            LoadingIndicator.IsVisible = isLoading;
            LoadingIndicator.IsRunning = isLoading;

            // Disable buttons while loading
            GetLocationBtn.IsEnabled = !isLoading;
            SearchCityBtn.IsEnabled = !isLoading;
        }

        private void ShowStatusMessage(string message)
        {
            StatusLabel.Text = message;
            StatusLabel.IsVisible = true;
        }

        private void HideStatusMessage()
        {
            StatusLabel.IsVisible = false;
        }
    }
}
