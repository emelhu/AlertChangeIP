using Microsoft.UI.Xaml;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AlertChangeIP.Classes
{
    public static class Helper
    {
        public static int IpToInt(string ip)
        {
            byte[] bytes = IPAddress.Parse(ip).GetAddressBytes();
            Array.Reverse(bytes); // hálózati -> host sorrend
            return BitConverter.ToInt32(bytes, 0);
        }

        public static string IntToIp(int ipInt)
        {
            byte[] bytes = BitConverter.GetBytes(ipInt);
            Array.Reverse(bytes); // host -> hálózati sorrend
            return new IPAddress(bytes).ToString();
        }

        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            if (value.CompareTo(max) > 0) return max;
            return value;
        }

        public static string GetPublicIP(string webAddress)
        {
            using var client = new HttpClient();
            string ip = client.GetStringAsync(webAddress).GetAwaiter().GetResult(); 
            return ip.Trim();
        }

        public static async Task<string> GetPublicIPAsync(string webAddress)
        {
            using var client = new HttpClient();
            string ip = await client.GetStringAsync(webAddress);
            return ip.Trim();
        }
    }
}
