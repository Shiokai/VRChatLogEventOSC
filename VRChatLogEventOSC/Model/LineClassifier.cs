using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using Reactive.Bindings;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using static VRChatLogEventOSC.RegexPattern;


namespace VRChatLogEventOSC
{
    public sealed class LineClassifier : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly LogFileWatcher _logFileWatcher;
        private readonly IDisposable _classifyDisposable;
        private readonly CompositeDisposable _eventsDisposable;
        private readonly Dictionary<EventTypeEnum, ReactivePropertySlim<string>> _eventReactiveProperties = new(){
            {EventTypeEnum.ReceivedInvite, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.ReceivedRequestInvite, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.SendInvite, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.SendRequestInvite, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.JoinedRoomURL, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.JoinedRoomName, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.SendFriendRequest, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.ReceivedFriendRequest, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.AcceptFriendRequest, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.ReceivedInviteResponse, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.ReceivedRequestInviteResponse, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.PlayedVideo1, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.PlayedVideo2, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.AcceptInvite, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.AcceptRequestInvite, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.OnPlayerJoined, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.OnPlayerLeft, new(string.Empty, ReactivePropertyMode.None)},
            {EventTypeEnum.TookScreenshot, new(string.Empty, ReactivePropertyMode.None)},
        };

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
            EventReactiveProperties = new Dictionary<EventTypeEnum, ReadOnlyReactivePropertySlim<string>>()
            {
                {EventTypeEnum.ReceivedInvite, _eventReactiveProperties[EventTypeEnum.ReceivedInvite].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.ReceivedRequestInvite, _eventReactiveProperties[EventTypeEnum.ReceivedRequestInvite].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.SendInvite, _eventReactiveProperties[EventTypeEnum.SendInvite].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.SendRequestInvite, _eventReactiveProperties[EventTypeEnum.SendRequestInvite].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.JoinedRoomURL, _eventReactiveProperties[EventTypeEnum.JoinedRoomURL].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.JoinedRoomName, _eventReactiveProperties[EventTypeEnum.JoinedRoomName].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.SendFriendRequest, _eventReactiveProperties[EventTypeEnum.SendFriendRequest].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.ReceivedFriendRequest, _eventReactiveProperties[EventTypeEnum.ReceivedFriendRequest].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.AcceptFriendRequest, _eventReactiveProperties[EventTypeEnum.AcceptFriendRequest].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.ReceivedInviteResponse, _eventReactiveProperties[EventTypeEnum.ReceivedInviteResponse].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.ReceivedRequestInviteResponse, _eventReactiveProperties[EventTypeEnum.ReceivedRequestInviteResponse].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.PlayedVideo1, _eventReactiveProperties[EventTypeEnum.PlayedVideo1].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.PlayedVideo2, _eventReactiveProperties[EventTypeEnum.PlayedVideo2].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.AcceptInvite, _eventReactiveProperties[EventTypeEnum.AcceptInvite].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.AcceptRequestInvite, _eventReactiveProperties[EventTypeEnum.AcceptRequestInvite].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.OnPlayerJoined, _eventReactiveProperties[EventTypeEnum.OnPlayerJoined].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.OnPlayerLeft, _eventReactiveProperties[EventTypeEnum.OnPlayerLeft].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
                {EventTypeEnum.TookScreenshot, _eventReactiveProperties[EventTypeEnum.TookScreenshot].ToReadOnlyReactivePropertySlim<string>(mode: ReactivePropertyMode.None)},
            };



            _eventsDisposable = new CompositeDisposable(_eventReactiveProperties.Values);

            _logFileWatcher = logFileWatcher;

            _classifyDisposable = _logFileWatcher.LogLineObservable
            .Where(l => DatetimeRegex.IsMatch(l))
            .Select(l => DatetimeRegex.Replace(l, string.Empty))
            .Select(l => AnyEventRegex.Match(l))
            .Where(m => m.Success)
            .Subscribe(m =>
            {
                EventTypeEnum eventType = GetMatchGropeType(m);
                _eventReactiveProperties[eventType].Value = m.Value;
                // Debug.Print($"{eventType}: {m.Value}");
            });
        }
    }
}
