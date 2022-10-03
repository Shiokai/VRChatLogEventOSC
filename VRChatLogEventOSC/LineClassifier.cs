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
            {EventTypeEnum.ReceivedInvite, new(string.Empty)},
            {EventTypeEnum.ReceivedRequestInvite, new(string.Empty)},
            {EventTypeEnum.SendInvite, new(string.Empty)},
            {EventTypeEnum.SendRequestInvite, new(string.Empty)},
            {EventTypeEnum.MetPlayer, new(string.Empty)},
            {EventTypeEnum.JoinedRoom1, new(string.Empty)},
            {EventTypeEnum.JoinedRoom2, new(string.Empty)},
            {EventTypeEnum.SendFriendRequest, new(string.Empty)},
            {EventTypeEnum.ReceivedFriendRequest, new(string.Empty)},
            {EventTypeEnum.AcceptFriendRequest, new(string.Empty)},
            {EventTypeEnum.ReceivedInviteResponse, new(string.Empty)},
            {EventTypeEnum.ReceivedRequestInviteResponse, new(string.Empty)},
            {EventTypeEnum.PlayedVideo1, new(string.Empty)},
            {EventTypeEnum.PlayedVideo2, new(string.Empty)},
            {EventTypeEnum.AcceptInvite, new(string.Empty)},
            {EventTypeEnum.AcceptRequestInvite, new(string.Empty)},
            {EventTypeEnum.OnPlayerJoined, new(string.Empty)},
            {EventTypeEnum.OnPlayerLeft, new(string.Empty)},
            {EventTypeEnum.JoinedRoom1Detail, new(string.Empty)},
            {EventTypeEnum.AcceptInviteDetail, new(string.Empty)},
            {EventTypeEnum.NotificationEvent, new(string.Empty)},
            {EventTypeEnum.TookScreenshot, new(string.Empty)},
        };

        public IReadOnlyDictionary<EventTypeEnum, ReadOnlyReactivePropertySlim<string>> EventReactiveProperties;

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
                {EventTypeEnum.ReceivedInvite, _eventReactiveProperties[EventTypeEnum.ReceivedInvite].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.ReceivedRequestInvite, _eventReactiveProperties[EventTypeEnum.ReceivedRequestInvite].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.SendInvite, _eventReactiveProperties[EventTypeEnum.SendInvite].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.SendRequestInvite, _eventReactiveProperties[EventTypeEnum.SendRequestInvite].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.MetPlayer, _eventReactiveProperties[EventTypeEnum.MetPlayer].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.JoinedRoom1, _eventReactiveProperties[EventTypeEnum.JoinedRoom1].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.JoinedRoom2, _eventReactiveProperties[EventTypeEnum.JoinedRoom2].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.SendFriendRequest, _eventReactiveProperties[EventTypeEnum.SendFriendRequest].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.ReceivedFriendRequest, _eventReactiveProperties[EventTypeEnum.ReceivedFriendRequest].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.AcceptFriendRequest, _eventReactiveProperties[EventTypeEnum.AcceptFriendRequest].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.ReceivedInviteResponse, _eventReactiveProperties[EventTypeEnum.ReceivedInviteResponse].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.ReceivedRequestInviteResponse, _eventReactiveProperties[EventTypeEnum.ReceivedRequestInviteResponse].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.PlayedVideo1, _eventReactiveProperties[EventTypeEnum.PlayedVideo1].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.PlayedVideo2, _eventReactiveProperties[EventTypeEnum.PlayedVideo2].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.AcceptInvite, _eventReactiveProperties[EventTypeEnum.AcceptInvite].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.AcceptRequestInvite, _eventReactiveProperties[EventTypeEnum.AcceptRequestInvite].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.OnPlayerJoined, _eventReactiveProperties[EventTypeEnum.OnPlayerJoined].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.OnPlayerLeft, _eventReactiveProperties[EventTypeEnum.OnPlayerLeft].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.JoinedRoom1Detail, _eventReactiveProperties[EventTypeEnum.JoinedRoom1Detail].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.AcceptInviteDetail, _eventReactiveProperties[EventTypeEnum.AcceptInviteDetail].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.NotificationEvent, _eventReactiveProperties[EventTypeEnum.NotificationEvent].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
                {EventTypeEnum.TookScreenshot, _eventReactiveProperties[EventTypeEnum.TookScreenshot].Skip(1).ToReadOnlyReactivePropertySlim<string>()},
            };



            _eventsDisposable = new CompositeDisposable(_eventReactiveProperties.Values);





            _logFileWatcher = logFileWatcher;

            _classifyDisposable = _logFileWatcher.LogLineObservable
            .Where(l => DatetimeRegex.IsMatch(l))
            .Select(l => DatetimeRegex.Replace(l, string.Empty))
            // .Where(l => AnyEventRegex.IsMatch(l))
            .Select(l => AnyEventRegex.Match(l))
            .Where(m => m.Success)
            .Subscribe(m =>
            {
                // Debug.Print(m.Value);
                // string name = GetMatchGropeType(m).ToString();
                // Debug.Print(name);

                EventTypeEnum eventType = GetMatchGropeType(m);
                _eventReactiveProperties[eventType].Value = m.Value;
            });
        }
    }
}
