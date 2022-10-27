using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.IO;
using Reactive.Bindings;
using VRChatLogEventOSC.Common;

namespace VRChatLogEventOSC.Control
{
    internal class ControlWindowModel
    {
        private static ControlWindowModel? _instance;
        public static ControlWindowModel Instance => _instance ??= new ControlWindowModel();
        private Core _core;
        private static readonly string _defaultLogDirectoryPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow", "VRChat", "VRChat"));
        public string DefaultLogDirectoryPath => _defaultLogDirectoryPath;

        public ReadOnlyReactivePropertySlim<bool> IsRunning => _core.IsRunnging;

        public void PuaseLogWEvent()
        {
            _core.Pause();
        }

        public void RestartLogEvent()
        {
            _core.Restart();
        }

        public void RestartLogEventWithScan()
        {
            _core.RestartWithScan();
        }

        public void Rescan()
        {
            _core.Rescan();
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

            _core.AttachConfig(config);
            return config;
        }

        public void SaveConfig(string ipAddress, int port, string logFileDirectory)
        {
            var config = new ConfigData(ipAddress, port, logFileDirectory);
            FileLoader.SaveConfig(config);
            _core.AttachConfig(config);
        }

        private ControlWindowModel()
        {
            _core = Core.Instance;
        }
    }
}
