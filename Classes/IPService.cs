using System;
using System.Collections.Generic;
using System.Text;

namespace AlertChangeIP.Classes
{
    public static class IPService
    {
        public static string GetPublicIP()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    return client.GetStringAsync("https://api.ipify.org").Result;
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
