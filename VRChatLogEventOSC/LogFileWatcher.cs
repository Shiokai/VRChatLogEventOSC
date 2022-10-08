using System;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.IO;
using Reactive.Bindings;

namespace VRChatLogEventOSC
{
    public sealed class LogFileWatcher : IDisposable
    {
        private static readonly string _logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow", "VRChat", "VRChat");
        private string _logFilePath = "";
        private readonly FileSystemWatcher _watcher = new()
        {
            Path = _logDirectoryPath,
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
        private bool _isWatching = false;
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

        private void LoadLatestLogFile()
        {
            _logFilePath = Directory.GetFiles(_logDirectoryPath, "output_log_*.txt", SearchOption.TopDirectoryOnly).OrderByDescending(file => Directory.GetCreationTime(file)).First();
            _lastLength = 0;
        }

        public void StartWatchingFromCurrent()
        {
            using (var fileStream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                    _lastLength = fileStream.Length;
            }

            _isWatching = true;
        }

        public void StartWatchingFromTop()
        {
            _lastLength = 0;
            _isWatching = true;
        }

        public void StartWatchingFromLatest()
        {
            _isWatching = true;
        }

        public void PauseWatching()
        {
            _isWatching = false;
        }

        public LogFileWatcher()
        {
            LoadLatestLogFile();
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

            
            _watchDisposable = Observable.Interval(TimeSpan.FromSeconds(Interval)).Where(_ => _isWatching).Subscribe(_ =>
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
