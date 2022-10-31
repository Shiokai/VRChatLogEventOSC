using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Reactive.Bindings;

using VRChatLogEventOSC.Common;
using VRChatLogEventOSC.Core;

namespace VRChatLogEventOSC.Control
{
    internal class ControlWindowModel
    {
        private static ControlWindowModel? _instance;
        public static ControlWindowModel Instance => _instance ??= new ControlWindowModel();
        private readonly LogEventCore _core;
        private static readonly string _defaultLogDirectoryPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow", "VRChat", "VRChat"));
        public static string DefaultLogDirectoryPath => _defaultLogDirectoryPath;

        public ReadOnlyReactivePropertySlim<bool> IsRunning => _core.IsRunnging;

        /// <summary>
        /// ログの読み取りを一時停止します
        /// </summary>
        public void PuaseLogWEvent()
        {
            _core.Pause();
        }

        /// <summary>
        /// 最新の位置からログの読み取りを再開します
        /// </summary>
        public void RestartLogEvent()
        {
            _core.Restart();
        }

        /// <summary>
        /// 停止していた間のログを読み取りつつログの読み取りを再開します
        /// </summary>
        public void RestartLogEventWithScan()
        {
            _core.RestartWithScan();
        }

        /// <summary>
        /// 現在のログファイル全体を再読み込みします
        /// </summary>
        public void Rescan()
        {
            _core.Rescan();
        }

        /// <summary>
        /// アプリケーションを終了します
        /// </summary>
        public static void QuitApplication()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// コンフィグファイルを読み込み、読み込んだコンフィグを返します
        /// </summary>
        /// <returns>読み込まれたコンフィグ</returns>
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

        /// <summary>
        /// コンフィグを保存します
        /// </summary>
        /// <param name="ipAddress">保存するコンフィグのIP Adress</param>
        /// <param name="port">保存するコンフィグのPort番号</param>
        /// <param name="logFileDirectory">保存するコンフィグのログファイルのディレクトリパス</param>
        public void SaveConfig(string ipAddress, int port, string logFileDirectory, bool isTuned)
        {
            var config = new ConfigData(ipAddress, port, logFileDirectory, isTuned);
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
