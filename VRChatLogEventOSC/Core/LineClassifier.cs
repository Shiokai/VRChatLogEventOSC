using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using static VRChatLogEventOSC.Common.RegexPattern;


namespace VRChatLogEventOSC.Core
{
    internal sealed class LineClassifier : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly LogFileWatcher _logFileWatcher;
        private readonly IDisposable _classifyDisposable;
        private readonly CompositeDisposable _eventsDisposable;
        private readonly Dictionary<EventTypeEnum, ReactivePropertySlim<string>> _eventReactiveProperties;

        public IReadOnlyDictionary<EventTypeEnum, ReadOnlyReactivePropertySlim<string>> EventReactiveProperties { get; }

        private bool _disposed = false;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _logFileWatcher.Dispose();
            _classifyDisposable.Dispose();
            _eventsDisposable.Dispose();
            _disposed = true;
        }

        public LineClassifier(LogFileWatcher logFileWatcher)
        {
            _eventReactiveProperties = new Dictionary<EventTypeEnum, ReactivePropertySlim<string>>();
            // 全イベントのKeyを追加
            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }
                _eventReactiveProperties.Add(type, new(string.Empty, ReactivePropertyMode.None));
            }

            _eventsDisposable = new CompositeDisposable(_eventReactiveProperties.Values);
            var eventReactiveProperties = new Dictionary<EventTypeEnum, ReadOnlyReactivePropertySlim<string>>();
            // 全イベントのKeyを追加
            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }
                // ReactivePropertyをReadOnlyに
                eventReactiveProperties.Add(type, _eventReactiveProperties[type].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None).AddTo(_eventsDisposable));
            }
            // ReadonlyDictionaryに
            EventReactiveProperties = eventReactiveProperties;

            _logFileWatcher = logFileWatcher;

            _classifyDisposable = _logFileWatcher.LogLineObservable
            .Where(l => DatetimeRegex.IsMatch(l))
            .Select(l => DatetimeRegex.Replace(l, string.Empty))
            .Select(l => AnyEventRegex.Match(l))
            .Where(m => m.Success)
            .Subscribe(m =>
            {
                // この行がどのイベントに対応するか振り分け
                EventTypeEnum eventType = GetMatchGropeType(m);
                _eventReactiveProperties[eventType].Value = m.Value;
            });
        }
    }
}
