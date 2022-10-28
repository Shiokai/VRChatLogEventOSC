using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;

namespace VRChatLogEventOSC.Core
{
    internal sealed class LogFileWatcher : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private static readonly string _defaultLogDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow", "VRChat", "VRChat");
        public string LogDirectoryPath { get; private set; } = _defaultLogDirectoryPath;
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

        /// <summary>
        /// ログの読み取り間隔
        /// インスタンス初期化後の変更は無効
        /// </summary>
        public float Interval { get; set; } = 0.1f;

        private readonly ReactivePropertySlim<string> _logLine = new(string.Empty);
        public IObservable<string> LogLineObservable => _logLine.Skip(1);
        private readonly ReactivePropertySlim<bool> _isWatching = new(false);
        /// <summary>
        /// 今現在ログの読み取りが行われているか
        /// </summary>
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

        /// <summary>
        /// 指定されたフォルダ内にある、作成日が最新のログファイルを読み取り対象のログファイルに設定します
        /// </summary>
        public void LoadLatestLogFile()
        {
            _logFilePath = Directory.GetFiles(LogDirectoryPath, "output_log_*.txt", SearchOption.TopDirectoryOnly).OrderByDescending(file => Directory.GetCreationTime(file)).FirstOrDefault() ?? "";
            _lastLength = 0;
        }

        /// <summary>
        /// 最終読み取り位置をログファイルの最後尾に移動します
        /// </summary>
        public void SeekToCurrent()
        {
            if (!File.Exists(_logFilePath))
            {
                return;
            }
            
            using var fileStream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _lastLength = fileStream.Length;
        }

        /// <summary>
        /// 読み取り開始位置をログファイルの末尾に移動し、ログの読み取りが行われるように設定します
        /// </summary>
        public void StartWatchingFromCurrent()
        {
            SeekToCurrent();
            _isWatching.Value = true;
        }

        /// <summary>
        /// 読み取り開始位置をログファイルの先頭に移動し、ログの読み取りが行われるように設定します
        /// </summary>
        public void StartWatchingFromTop()
        {
            _lastLength = 0;
            _isWatching.Value = true;
        }

        /// <summary>
        /// 読み取り開始位置は変更せず、ログの読み取りが行われるように設定します
        /// </summary>
        public void StartWatchingFromLatest()
        {
            _isWatching.Value = true;
        }

        /// <summary>
        /// ログの読み取りが行われないように設定します
        /// </summary>
        public void PauseWatching()
        {
            _isWatching.Value = false;
        }

        /// <summary>
        /// ログファイルの作製を検知するフォルダを変更します
        /// </summary>
        /// <param name="dirPath">ログファイルの作製を検知するフォルダへのパス</param>
        public void ChangeLogDerectory(string dirPath)
        {
            LogDirectoryPath = dirPath;
            _watcher.Path = dirPath;
        }

        public LogFileWatcher()
        {
            IsWatching = _isWatching.ToReadOnlyReactivePropertySlim(false, ReactivePropertyMode.DistinctUntilChanged);
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
