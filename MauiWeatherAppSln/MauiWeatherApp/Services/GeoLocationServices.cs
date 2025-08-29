namespace MauiWeatherApp.Services
{
    public class GeolocationService
    {
        public async Task<Location?> GetCurrentLocationAsync()
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(10)
                };

                var location = await Geolocation.Default.GetLocationAsync(request);
                return location;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unable to get location: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CheckAndRequestLocationPermission()
        {
            try
            {
                PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status == PermissionStatus.Granted)
                    return true;

                if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    // On iOS, once a user denies the permission, the app cannot request it again
                    return false;
                }

                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                return status == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking location permission: {ex.Message}");
                return false;
            }
        }
    }
}