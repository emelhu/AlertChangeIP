using System;
using System.Collections.Generic;
using System.Text;

namespace AlertChangeIP.Classes
{
    public static class IPService
    {
        /// <summary>
        /// Gets the public IP address of the machine by making a request to an external service. Exception if not successful.
        /// </summary>
        /// <returns>The public IP address as a string.</returns>
        public static string GetPublicIP()
        {
            using (var client = new HttpClient())
            {
                AppParams appParams = new AppParams();

                try
                {
                    return client.GetStringAsync(appParams.webURL).Result;
                }
                catch (Exception ex)
                {
                    LoggingService.WriteLog($"Error fetching public IP: {ex.Message}", LoggingService.LogLevel.Error);

                    throw;
                }
            }
        }
    }
}
