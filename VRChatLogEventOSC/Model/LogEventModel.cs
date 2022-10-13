using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using System.ComponentModel;

using VRChatLogEventOSC.Common;

namespace VRChatLogEventOSC
{
    internal class LogEventModel : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private static LogEventModel? _instance;
        public static LogEventModel Instance
        { 
            get => _instance ??= new LogEventModel();
        }

        private readonly LogFileWatcher _logFileWatcher = new();
        private readonly LineClassifier _lineClassifier;
        private readonly EventToOSCConverter _converter;
        private readonly OSCSender _sender = new();
        
        public ReadOnlyReactivePropertySlim<bool> IsRunnging;

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

        public void Pause()
        {
            _logFileWatcher.PauseWatching();
        }

        public void Restart()
        {
            _logFileWatcher.StartWatchingFromCurrent();
        }

        public void RestartWithScan()
        {
            _logFileWatcher.StartWatchingFromLatest();
        }

        public void Rescan()
        {
            _logFileWatcher.StartWatchingFromTop();
        }

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

        private LogEventModel()
        {
            _lineClassifier = new LineClassifier(_logFileWatcher);
            _converter = new EventToOSCConverter(_lineClassifier, _sender);
            _converter.CurrentSetting = FileLoader.LoadSetting() ?? new WholeSetting();
            _logFileWatcher.LoadLatestLogFile();
            _logFileWatcher.IsDetectFileCreation = true;
            _logFileWatcher.StartWatchingFromCurrent();
            IsRunnging = _logFileWatcher.IsWatching;
        }
    }
}
