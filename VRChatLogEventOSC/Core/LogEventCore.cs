using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Reactive.Bindings;

using VRChatLogEventOSC.Common;

namespace VRChatLogEventOSC.Core
{
    internal class LogEventCore : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private static LogEventCore? _instance;
        public static LogEventCore Instance
        { 
            get => _instance ??= new LogEventCore();
        }

        private readonly LogFileWatcher _logFileWatcher = new();
        private readonly LineClassifier _lineClassifier;
        private readonly EventToOSCConverter _converter;
        private readonly OSCSender _sender = new();
        
        public ReadOnlyReactivePropertySlim<bool> IsRunnging;

        public IReadOnlyList<SingleSetting>? GetCurrentSettingsOfType(RegexPattern.EventTypeEnum type)
        {
            _converter.CurrentSetting.Settings.TryGetValue(type, out var settings);
            return settings;
        }

        private bool _disposed = false;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _logFileWatcher.Dispose();
            _lineClassifier.Dispose();
            _converter.Dispose();
            _sender.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// ログの読み取りを一時停止します
        /// </summary>
        public void Pause()
        {
            _logFileWatcher.PauseWatching();
        }

        /// <summary>
        /// ログの最新部分からログの読み取りを再開します
        /// </summary>
        public void Restart()
        {
            _logFileWatcher.StartWatchingFromCurrent();
        }

        /// <summary>
        /// 最後に読み取った位置からログの読み取りを再開します
        /// </summary>
        public void RestartWithScan()
        {
            _logFileWatcher.StartWatchingFromLatest();
        }

        /// <summary>
        /// 現在のログファイル全体を再度読み取ります
        /// </summary>
        public void Rescan()
        {
            _logFileWatcher.StartWatchingFromTop();
        }

        /// <summary>
        /// コンフィグを元にOSC送信先のIP Adress、Portおよびログのディレクトリを変更します
        /// </summary>
        /// <param name="config">変更先の情報を持ったコンフィグ</param>
        public void AttachConfig(ConfigData config)
        {
            _sender.ChangeClient(config.IPAddress.ToString(), config.Port);
            
            if (config.LogFileDirectory == _logFileWatcher.LogDirectoryPath)
            {
                return;
            }

            _logFileWatcher.ChangeLogDerectory(config.LogFileDirectory);
            _logFileWatcher.LoadLatestLogFile();
            _logFileWatcher.SeekToCurrent();
        }

        /// <summary>
        /// 設定ファイルを読み取り、判定に用いる設定に適用します
        /// </summary>
        public void LoadCurrentSetting()
        {
            _converter.CurrentSetting = FileLoader.LoadSetting() ?? new WholeSetting();
        }

        private LogEventCore()
        {
            _lineClassifier = new LineClassifier(_logFileWatcher);
            _converter = new EventToOSCConverter(_lineClassifier, _sender);
            IsRunnging = _logFileWatcher.IsWatching;
            
            try
            {
                LoadCurrentSetting();
            }
            catch (System.IO.IOException e)
            {
                MessageBox.Show($"設定ファイルの読み込みに失敗しました\n{e.Message}", "IOException", MessageBoxButton.OK);
                Application.Current.Shutdown();
                return;
            }
            catch(UnauthorizedAccessException e)
            {
                MessageBox.Show($"設定ファイルへのアクセスが拒否されました\n{e.Message}", "UnauthorizedAccessException", MessageBoxButton.OK);
                Application.Current.Shutdown();
                return;
            }
            
            _logFileWatcher.LoadLatestLogFile();
            _logFileWatcher.IsDetectFileCreation = true;
            _logFileWatcher.StartWatchingFromCurrent();
        }
    }
}
