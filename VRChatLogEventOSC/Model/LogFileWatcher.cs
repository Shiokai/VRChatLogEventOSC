using System;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.IO;
using Reactive.Bindings;
using System.ComponentModel;

namespace VRChatLogEventOSC
{
    public sealed class LogFileWatcher : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private static readonly string _defaultLogDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow", "VRChat", "VRChat");
        private string _logDirectoryPath { get; set; } = _defaultLogDirectoryPath;
        private string _logFilePath = "";
        private readonly FileSystemWatcher _watcher = new()
        {
            Path = _defaultLogDirectoryPath,
            Filter = "*.txt",
            NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName,
            IncludeSubdirectories = false
        };

        private readonly IDisposable _watchDisposable;
        private readonly IDisposable _fileCreationDisposable;

        private long _lastLength = 0;

        public float Interval { get; set; } = 0.1f;

        private readonly ReactivePropertySlim<string> _logLine = new(string.Empty);
        public IObservable<string> LogLineObservable => _logLine.Skip(1);
        // private bool _isWatching = false;
        private readonly ReactivePropertySlim<bool> _isWatching = new(false);
        public ReadOnlyReactivePropertySlim<bool> IsWatching;
        public bool IsDetectFileCreation {get; set;} = false;

        private bool _disposed = false;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _fileCreationDisposable.Dispose();
            _watchDisposable.Dispose();
            _disposed = true;
        }

        public void LoadLatestLogFile()
        {
            _logFilePath = Directory.GetFiles(_logDirectoryPath, "output_log_*.txt", SearchOption.TopDirectoryOnly).OrderByDescending(file => Directory.GetCreationTime(file)).First();
            _lastLength = 0;
        }

        public void StartWatchingFromCurrent()
        {
            if (!File.Exists(_logFilePath))
            {
                return;
            }
            
            using (var fileStream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                    _lastLength = fileStream.Length;
            }

            _isWatching.Value = true;
        }

        public void StartWatchingFromTop()
        {
            _lastLength = 0;
            _isWatching.Value = true;
        }

        public void StartWatchingFromLatest()
        {
            _isWatching.Value = true;
        }

        public void PauseWatching()
        {
            _isWatching.Value = false;
        }

        public void ChangeLogDerectory(string dirPath)
        {
            _logDirectoryPath = dirPath;
            _watcher.Path = dirPath;
        }

        public LogFileWatcher()
        {
            IsWatching = _isWatching.ToReadOnlyReactivePropertySlim(false, ReactivePropertyMode.DistinctUntilChanged);
            // LoadLatestLogFile();
            _fileCreationDisposable = Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                h => (s, e) => h(e),
                h => _watcher.Created += h,
                h => _watcher.Created -= h
            ).Where(_ => IsDetectFileCreation)
            .Select(e => e.FullPath)
            .Where(path => path.Contains("output_log_"))
            .Subscribe(path =>
            {
                _logFilePath = path;
                _lastLength = 0;
            });

            _watcher.EnableRaisingEvents = true;

            
            _watchDisposable = Observable.Interval(TimeSpan.FromSeconds(Interval)).Where(_ => _isWatching.Value).Subscribe(_ =>
            {
                if (!File.Exists(_logFilePath))
                {
                    return;
                }
                try
                {
                    using (var fileStream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        fileStream.Seek(_lastLength, SeekOrigin.Begin);
                        using (var reader = new StreamReader(fileStream))
                        {
                            string? line = string.Empty;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (string.IsNullOrWhiteSpace(line))
                                {
                                    continue;
                                }
                                _logLine.Value = line;
                            }
                            _lastLength = fileStream.Length;
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    LoadLatestLogFile();
                }
            });
        }
    }


}
