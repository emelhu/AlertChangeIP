using System;
using System.Collections.Generic;
using System.Text;

namespace AlertChangeIP.Classes
{
    public static class LoggingService
    {
        public static string logFileName => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IPChangeLog.txt");

        public static void WriteLog(string text, LogLevel level = LogLevel.Info)
        {
            string logEntry = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | {level.ToString()} | {text}";
            File.AppendAllText(logFileName, logEntry + Environment.NewLine);
        }

        public static void LogIPChange(string oldIP, string newIP)
        {
            WriteLog($"IP changed from {oldIP} to {newIP}");
        }

        public enum LogLevel
        {
            Info,
            Warning,
            Error
        }
    }
}
