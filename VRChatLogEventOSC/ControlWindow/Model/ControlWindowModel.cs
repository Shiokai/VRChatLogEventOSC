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
using VRChatLogEventOSC.Core;

namespace VRChatLogEventOSC.Control
{
    internal class ControlWindowModel
    {
        private static ControlWindowModel? _instance;
        public static ControlWindowModel Instance => _instance ??= new ControlWindowModel();
        private LogEventCore _core;
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
            ConfigData? config;
            try
            {
                config = FileLoader.LoadConfig();
            }
            catch (IOException e)
            {
                MessageBox.Show($"Configファイルの読み込みに失敗しました\n{e.Message}", "IOException", MessageBoxButton.OK);
                config = null;
            }
            catch(UnauthorizedAccessException e)
            {
                MessageBox.Show($"Configファイルへのアクセスが拒否されました\n{e.Message}", "UnauthorizedAccessException", MessageBoxButton.OK);
                config = null;
            }

            if (config == null)
            {
                var result = MessageBox.Show("Failed to load config.\nCreate default config.", "Load config", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    config = new ConfigData();
                    
                    try
                    {
                        FileLoader.SaveConfig(config);
                    }
                    catch (IOException e)
                    {
                        MessageBox.Show($"Configファイルの書き込みに失敗しました\n{e.Message}", "IOException", MessageBoxButton.OK);
                    }
                    catch(UnauthorizedAccessException e)
                    {
                        MessageBox.Show($"Configファイルへのアクセスが拒否されました\n{e.Message}", "UnauthorizedAccessException", MessageBoxButton.OK);
                    }
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
            try
            {
                FileLoader.SaveConfig(config);
            }
            catch (IOException e)
            {
                MessageBox.Show($"Configファイルの書き込みに失敗しました\n{e.Message}", "IOException", MessageBoxButton.OK);
                return;
            }
            catch(UnauthorizedAccessException e)
            {
                MessageBox.Show($"Configファイルへのアクセスが拒否されました\n{e.Message}", "UnauthorizedAccessException", MessageBoxButton.OK);
                return;
            }
            _core.AttachConfig(config);
        }

        private ControlWindowModel()
        {
            _core = LogEventCore.Instance;
        }
    }
}
