using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.IO;
using VRChatLogEventOSC.Common;

namespace VRChatLogEventOSC.Control
{
    internal class ControlWindowModel
    {
        private static ControlWindowModel? _instance;
        public static ControlWindowModel Instance => _instance ??= new ControlWindowModel();
        private LogEventModel _logEventModel;
        private static readonly string _defaultLogDirectoryPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow", "VRChat", "VRChat"));
        public string DefaultLogDirectoryPath => _defaultLogDirectoryPath;

        public void PuaseLogWEvent()
        {
            _logEventModel.Pause();
        }

        public void RestartLogEvent()
        {
            _logEventModel.Restart();
        }

        public void RestartLogEventWithScan()
        {
            _logEventModel.RestartWithScan();
        }

        public void Rescan()
        {
            _logEventModel.Rescan();
        }

        public static void QuitApplication()
        {
            Application.Current.Shutdown();
        }

        public ConfigData LoadConfig()
        {
            var config = FileLoader.LoadConfig();
            if (config == null)
            {
                var result = MessageBox.Show("Failed to load config.\nCreate default config.", "Load config", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    config = new ConfigData();
                    FileLoader.SaveConfig(config);
                }
                else
                {
                    return new ConfigData();
                }
            }

            _logEventModel.AttachConfig(config);
            return config;
        }

        public void SaveConfig(string ipAddress, int port, string logFileDirectory)
        {
            var config = new ConfigData(ipAddress, port, logFileDirectory);
            FileLoader.SaveConfig(config);
            _logEventModel.AttachConfig(config);
        }

        private ControlWindowModel()
        {
            _logEventModel = LogEventModel.Instance;
        }
    }
}
