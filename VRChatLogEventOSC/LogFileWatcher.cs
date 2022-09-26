using System;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Reactive.Bindings;

namespace VRChatLogEventOSC
{
    public sealed class LogFileWatcher : IDisposable
    {
        private static readonly string _logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow", "VRChat", "VRChat");
        private string _latestLogPath = Directory.GetFiles(_logPath, "output_log_*.txt", SearchOption.TopDirectoryOnly).OrderByDescending(file => Directory.GetCreationTime(file)).First();
        private readonly FileSystemWatcher _watcher = new()
        {
            Path = _logPath,
            Filter = "*.txt",
            NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName,
            IncludeSubdirectories = false
        };

        private readonly IDisposable _watchDisposable;
        private readonly IDisposable _creatDisposable;

        // private int _currentLine = 0;
        private long _lastLength = 0;

        public float Interval { get; set; } = 0.1f;

        private ReactiveProperty<string> _logLine = new(string.Empty);
        public IObservable<string> LogLineObservable => _logLine.Skip(1);

        public void Dispose()
        {
            _creatDisposable.Dispose();
            _watchDisposable.Dispose();
        }

        private void LoadLatestLogFile()
        {
            _latestLogPath = Directory.GetFiles(_logPath, "output_log_*.txt", SearchOption.TopDirectoryOnly).OrderByDescending(file => Directory.GetCreationTime(file)).First();
            _lastLength = 0;
        }

        public LogFileWatcher()
        {
            // VRChat終了時にしか発行されない。
            // _disposable = Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
            //     h => (s, e) => h(e),
            //     h => _watcher.Changed += h,
            //     h => _watcher.Changed -= h
            // )
            // .Throttle(TimeSpan.FromSeconds(Interval))
            // .Subscribe(
            //     e =>
            //     {
            //         int lineCount = 0;
            //         foreach (string line in File.ReadLines(e.FullPath).Skip(_currentLine).Where(line => !string.IsNullOrWhiteSpace(line)))
            //         {
            //             lineCount++;
            //             Debug.Print(line);
            //         }
            //         _currentLine = lineCount;
            //     }
            // );
            _creatDisposable = Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                h => (s, e) => h(e),
                h => _watcher.Created += h,
                h => _watcher.Created -= h
            ).Select(e => e.FullPath).Where(path => path.Contains("output_log_")).Subscribe(path =>
            {
                _latestLogPath = path;
                // _currentLine = 0;
                _lastLength = 0;
                Debug.Print("OnCreat is called.");
            });

            
            _watchDisposable = Observable.Interval(TimeSpan.FromSeconds(Interval)).Subscribe(_ =>
            {
                // ファイルがロックされていてIOErrorになる
                // int lineCount = 0;
                // foreach (string line in File.ReadLines(_latestLogPath).Skip(_currentLine).Where(line => !string.IsNullOrEmpty(line)))
                // {
                //     lineCount++;
                //     Debug.Print(line);
                // }
                // _currentLine += lineCount;
                if (!File.Exists(_latestLogPath))
                {
                    return;
                }
                try
                {
                    using (var fileStream = new FileStream(_latestLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
                                // Debug.Print(line);
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
            _watcher.EnableRaisingEvents = true;
        }
    }


}
