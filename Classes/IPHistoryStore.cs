using Microsoft.WindowsAppSDK.Runtime.Packages;

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

using Windows.Devices.Printers;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AlertChangeIP.Classes
{
    public static class IPHistoryStore
    {
        public static string logFileName => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"IPChangeHistory.data");

        public static void Write(string ipText)
        {
            if (IPAddress.TryParse(ipText, out IPAddress? ipAddress))
            {
                Int32 ipInt = Helper.IpToInt(ipText);
                Write(ipInt);
            }
            else
            {
                LoggingService.WriteLog($"Invalid IP address format: {ipText}", LoggingService.LogLevel.Error);
            }   
        }

        public static void Write(Int32 ip)
        {
            string? directoryPath = Path.GetDirectoryName(logFileName);

            if (directoryPath is not null && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }


            using (var fs = new FileStream(logFileName, FileMode.Append, FileAccess.Write))
            {
                byte[] ipBytes = BitConverter.GetBytes(ip);
                fs.Write(ipBytes, 0, ipBytes.Length);

                Int64 dtBinary = DateTime.Now.ToBinary();

                byte[] dtBytes = BitConverter.GetBytes(dtBinary);
                fs.Write(dtBytes, 0, dtBytes.Length);
            }
        }

        private const int   minimumReadedHistoryItemsCount = 1;
        private const int   maximumReadedHistoryItemsCount = 10000;
        private static int _defaultReadedHistoryItemsCount = 100;
        public  static int  defaultReadedHistoryItemsCount {get => _defaultReadedHistoryItemsCount; set => _defaultReadedHistoryItemsCount = Math.Min(Math.Max(value, minimumReadedHistoryItemsCount), maximumReadedHistoryItemsCount); }

        public static List<IPHistoryStruct>? Read(int itemsCount = 0)
        {
            if (itemsCount <= 0)
            {
                itemsCount = defaultReadedHistoryItemsCount;
            }

            var result = new List<IPHistoryStruct>();

            using (var fs = new FileStream(logFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                const int recordLength = sizeof(Int32) + sizeof(Int64);                                                     // 4 bytes for IP + 8 bytes for DateTime
                int fileSize = (int)fs.Length;
                if (fileSize % recordLength != 0)
                {
                    LoggingService.WriteLog("Invalid data in the 'IP history' file!", LoggingService.LogLevel.Error);
                    return null;                                                                                            // Invalid data in the log file, return null
                }

                int offset = 0;

                if (fileSize > itemsCount * recordLength)
                {
                    offset = fileSize - (itemsCount * recordLength);                                                        // If the file is larger than the requested number of items, set the offset to read only the last 'itemsCount' records
                }   

                fs.Seek(offset, SeekOrigin.Begin);

                do { 
                    byte[] ipBytes = new byte[sizeof(Int32)];
                    int readedIPBytes = fs.Read(ipBytes, 0, ipBytes.Length);

                    if (readedIPBytes == 0)
                    {
                        break;                                                                                              // End of file reached, exit the loop, return the result list
                    }
                    else if (readedIPBytes < sizeof(Int32))
                    {
                        LoggingService.WriteLog("Invalid data in the 'IP history' file!", LoggingService.LogLevel.Error);
                        return null;                                                                                        // Invalid data in the log file, return null
                    }

                    Int32 ip = BitConverter.ToInt32(ipBytes, 0);
                    

                    byte[] dtBytes = new byte[sizeof(Int64)];
                    int readedDTBytes = fs.Read(dtBytes, 0, dtBytes.Length);

                    if (readedDTBytes < sizeof(Int64))
                    {
                        LoggingService.WriteLog("Invalid data in the 'IP history' file!", LoggingService.LogLevel.Error);
                        return null;                                                                                              // Invalid data in the log file, return null
                    }

                    DateTime when = DateTime.FromBinary(BitConverter.ToInt64(dtBytes, 0));

                    result.Add(new IPHistoryStruct { ip = ip, when = when });

                    offset += sizeof(Int32) + sizeof(Int64);    

                } while (true);
            }

            return result;
        }
    }

    public struct IPHistoryStruct
    {
        public Int32 ip;
        public DateTime when;

        public string ipText   => Helper.IntToIp(ip);
        public string whenText => when.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
