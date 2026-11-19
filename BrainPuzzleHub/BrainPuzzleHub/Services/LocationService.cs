using System;
using System.Collections.Generic;
using System.Text;
//A simple encapsulation of GPS location functionality:
//Requests permission and obtains location information once upon entering the homepage.

namespace BrainPuzzleHub.Services
{
    public class LocationService
    {
        public async Task RequestLocationOnceAsync()
        {
            try
            {
                //Request location permission
                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted) return;

                //Get the current location once (medium precision, saves power and waiting time).
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.Default.GetLocationAsync(request);

                if (location is not null)
                {
                    //Currently, only debug output is being recorded, which does not affect game functionality;
                    //it only proves that GPS has been successfully acquired.
                    System.Diagnostics.Debug.WriteLine(
                        $"[GPS] Latitude={location.Latitude}, Longitude={location.Longitude}");
                }
            }
            catch (Exception ex)
            {
                //Location failure /permission denied/GPS off/no signal on emulator, etc.
                //does not affect the normal use of the app; it only logs the failure.
                System.Diagnostics.Debug.WriteLine($"[GPS] Failed to get location: {ex.Message}");
            }
        }
    }

}
