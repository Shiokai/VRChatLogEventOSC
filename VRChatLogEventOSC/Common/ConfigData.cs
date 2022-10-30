using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace VRChatLogEventOSC.Common
{
    internal sealed record class ConfigData
    {
        public int JsonVersion { get; init; } = 1;
        public string IPAddress { get; init; }
        public int Port { get; init; }
        public string LogFileDirectory { get; init; }
        public ConfigData()
        {
            IPAddress = System.Net.IPAddress.Loopback.ToString();
            Port = 9000;
            LogFileDirectory = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow", "VRChat", "VRChat"));
        }

        public ConfigData(string ipAddress, int port, string logFileDirectory)
        {
            IPAddress = ipAddress;
            Port = port;
            LogFileDirectory = logFileDirectory;
        }

        [JsonConstructor]
        public ConfigData(int jsonVersion, string ipAddress, int port, string logFileDirectory)
        {
            JsonVersion = jsonVersion;
            IPAddress = ipAddress;
            Port = port;
            LogFileDirectory = logFileDirectory;
        }
    }
}
