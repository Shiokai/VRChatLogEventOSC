using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.Json.Serialization;


namespace VRChatLogEventOSC.Model
{
    internal sealed record class ConfigData
    {
        public string IPAddress { get; init; }
        public int Port { get; init; }
        public string LogFileDirectory { get; init; }
        public bool DetectLatestLogFile { get; init; }
        public bool FullScanWithDetect { get; init; }

        public ConfigData()
        {
            IPAddress = System.Net.IPAddress.Loopback.ToString();
            Port = 9000;
            LogFileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow", "VRChat", "VRChat");
            DetectLatestLogFile = true;
            FullScanWithDetect = false;
        }

        [JsonConstructor]
        public ConfigData(string ipAddress, int port, string logFileDirectory, bool detectLatestLogFile, bool fullScanWithDetect)
        {
            IPAddress = ipAddress;
            Port = port;
            LogFileDirectory = logFileDirectory;
            DetectLatestLogFile = detectLatestLogFile;
            FullScanWithDetect = fullScanWithDetect;
        }
    }
}
