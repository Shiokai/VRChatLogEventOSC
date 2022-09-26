using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using Reactive.Bindings;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;


namespace VRChatLogEventOSC
{
    public sealed class LineClassifier : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly LogFileWatcher _logFileWatcher;
        private readonly IDisposable _classifyDisposable;
        
        // private ReactivePropertySlim<string> _receivedInvite = new(string.Empty);
        // private ReactivePropertySlim<string> _receivedRequestInvite = new(string.Empty);
        // private ReactivePropertySlim<string> _sendInvite = new(string.Empty);
        // private ReactivePropertySlim<string> _sendRequestInvite = new(string.Empty);
        // private ReactivePropertySlim<string> _metPlayer = new(string.Empty);
        private ReactivePropertySlim<string> _joinedRoom1 = new(string.Empty);
        private ReactivePropertySlim<string> _joinedRoom2 = new(string.Empty);
        // private ReactivePropertySlim<string> _sendFriendRequest = new(string.Empty);
        // private ReactivePropertySlim<string> _receivedFriendRequest = new(string.Empty);
        private ReactivePropertySlim<string> _acceptFriendRequest = new(string.Empty);
        // private ReactivePropertySlim<string> _receivedInviteResponse = new(string.Empty);
        // private ReactivePropertySlim<string> _receivedRequestInviteResponse = new(string.Empty);
        private ReactivePropertySlim<string> _playedVideo1 = new(string.Empty);
        private ReactivePropertySlim<string> _playedVideo2 = new(string.Empty);
        private ReactivePropertySlim<string> _acceptInvite = new(string.Empty);
        private ReactivePropertySlim<string> _acceptRequestInvite = new(string.Empty);
        
        private ReactivePropertySlim<string> _onPlayerJoined = new(string.Empty);
        private ReactivePropertySlim<string> _onPlayerLeft = new(string.Empty);
        private ReactivePropertySlim<string> _joinedRoom1Detail = new(string.Empty);
        private ReactivePropertySlim<string> _acceptInviteDetail = new(string.Empty);
        // private ReactivePropertySlim<string> _notificationEvent = new(string.Empty);
        private ReactivePropertySlim<string> _tookScreenshot = new(string.Empty);

        

        // public ReadOnlyReactivePropertySlim<string> ReceivedInvite { get; }
        // public ReadOnlyReactivePropertySlim<string> ReceivedRequestInvite { get; }
        // public ReadOnlyReactivePropertySlim<string> SendInvite { get; }
        // public ReadOnlyReactivePropertySlim<string> SendRequestInvite { get; }
        // public ReadOnlyReactivePropertySlim<string> MetPlayer { get; }
        public ReadOnlyReactivePropertySlim<string> JoinedRoom1 { get; }
        public ReadOnlyReactivePropertySlim<string> JoinedRoom2 { get; }
        // public ReadOnlyReactivePropertySlim<string> SendFriendRequest { get; }
        // public ReadOnlyReactivePropertySlim<string> _receivedFriendRequest { get; }
        public ReadOnlyReactivePropertySlim<string> AcceptFriendRequest { get; }
        // public ReadOnlyReactivePropertySlim<string> ReceivedInviteResponse { get; }
        // public ReadOnlyReactivePropertySlim<string> ReceivedRequestInviteResponse { get; }
        public ReadOnlyReactivePropertySlim<string> PlayedVideo1 { get; }
        public ReadOnlyReactivePropertySlim<string> PlayedVideo2 { get; }
        public ReadOnlyReactivePropertySlim<string> AcceptInvite { get; }
        public ReadOnlyReactivePropertySlim<string> AcceptRequestInvite { get; }
        public ReadOnlyReactivePropertySlim<string> OnPlayerJoined { get; }
        public ReadOnlyReactivePropertySlim<string> OnPlayerLeft { get; }
        // public ReadOnlyReactivePropertySlim<string> JoinedRoom1Detail { get; }
        public ReadOnlyReactivePropertySlim<string> AcceptInviteDetail { get; }
        // public ReadOnlyReactivePropertySlim<string> NotificationEvent { get; }
        public ReadOnlyReactivePropertySlim<string> TookScreenshot { get; }

        public void Dispose()
        {
            _classifyDisposable.Dispose();
        }

        public LineClassifier(LogFileWatcher logFileWatcher)
        {

            // ReceivedInvite = _receivedInvite.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            // ReceivedRequestInvite = _receivedRequestInvite.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            // SendInvite = _sendInvite.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            // SendRequestInvite = _sendRequestInvite.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            // MetPlayer = _metPlayer.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            JoinedRoom1 = _joinedRoom1.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            JoinedRoom2 = _joinedRoom2.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            // SendFriendRequest = _sendFriendRequest.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            // _receivedFriendRequest = _receivedFriendRequest.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            AcceptFriendRequest = _acceptFriendRequest.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            // ReceivedInviteResponse = _receivedInviteResponse.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            // ReceivedRequestInviteResponse = _receivedRequestInviteResponse.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            PlayedVideo1 = _playedVideo1.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            PlayedVideo2 = _playedVideo2.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            AcceptInvite = _acceptInvite.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            AcceptRequestInvite = _acceptRequestInvite.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            OnPlayerJoined = _onPlayerJoined.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            OnPlayerLeft = _onPlayerLeft.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            // JoinedRoom1Detail = _joinedRoom1Detail.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            AcceptInviteDetail = _acceptInviteDetail.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            // NotificationEvent = _notificationEvent.Skip(1).ToReadOnlyReactivePropertySlim<string>();
            TookScreenshot = _tookScreenshot.Skip(1).ToReadOnlyReactivePropertySlim<string>();




            
            _logFileWatcher = logFileWatcher;
            _classifyDisposable = _logFileWatcher.LogLineObservable
            .Where(l => RegexPattern.DatetimeRegex.IsMatch(l))
            .Select(l => RegexPattern.DatetimeRegex.Replace(l, string.Empty))
            // .Where(l => RegexPattern.AnyEventRegex.IsMatch(l))
            .Select(l => RegexPattern.AnyEventRegex.Match(l))
            .Where(m => m.Success)
            .Subscribe(m =>
            {
                Debug.Print(m.Value);
                string name = RegexPattern.GetMatchGropeType(m).ToString();
                Debug.Print(name);

                switch (RegexPattern.GetMatchGropeType(m))
                {
                    // case RegexPattern.EventTypeEnum.ReceivedInvite:
                        // _receivedInvite.Value = m.Value;
                        // break;
                    // case RegexPattern.EventTypeEnum.ReceivedRequestInvite:
                        // _receivedRequestInvite.Value = m.Value;
                        // break;
                    // case RegexPattern.EventTypeEnum.SendInvite:
                        // _sendInvite.Value = m.Value;
                        // break;
                    // case RegexPattern.EventTypeEnum.SendRequestInvite:
                        // _sendRequestInvite.Value = m.Value;
                        // break;
                    // case RegexPattern.EventTypeEnum.MetPlayer:
                        // _metPlayer.Value = m.Value;
                        // break;
                    case RegexPattern.EventTypeEnum.JoinedRoom1:
                        _joinedRoom1.Value = m.Value;
                        break;
                    case RegexPattern.EventTypeEnum.JoinedRoom2:
                        _joinedRoom2.Value = m.Value;
                        break;
                    // case RegexPattern.EventTypeEnum.SendFriendRequest:
                        // _sendFriendRequest.Value = m.Value;
                        // break;
                    // case RegexPattern.EventTypeEnum.ReceivedFriendRequest:
                        // _receivedFriendRequest.Value = m.Value;
                        // break;
                    case RegexPattern.EventTypeEnum.AcceptFriendRequest:
                        _acceptFriendRequest.Value = m.Value;
                        break;
                    // case RegexPattern.EventTypeEnum.ReceivedInviteResponse:
                        // _receivedInviteResponse.Value = m.Value;
                        // break;
                    // case RegexPattern.EventTypeEnum.ReceivedRequestInviteResponse:
                        // _receivedRequestInviteResponse.Value = m.Value;
                        // break;
                    case RegexPattern.EventTypeEnum.PlayedVideo1:
                        _playedVideo1.Value = m.Value;
                        break;
                    case RegexPattern.EventTypeEnum.PlayedVideo2:
                        _playedVideo2.Value = m.Value;
                        break;
                    case RegexPattern.EventTypeEnum.AcceptInvite:
                        _acceptInvite.Value = m.Value;
                        break;
                    case RegexPattern.EventTypeEnum.AcceptRequestInvite:
                        _acceptRequestInvite.Value = m.Value;
                        break;
                    case RegexPattern.EventTypeEnum.OnPlayerJoined:
                        _onPlayerJoined.Value = m.Value;
                        break;
                    case RegexPattern.EventTypeEnum.OnPlayerLeft:
                        _onPlayerLeft.Value = m.Value;
                        break;
                    // case RegexPattern.EventTypeEnum.JoinedRoom1Detail:
                    //     _joinedRoom1Detail.Value = m.Value;
                    //     break;
                    // case RegexPattern.EventTypeEnum.AcceptInviteDetail:
                    //     _acceptInviteDetail.Value = m.Value;
                    //     break;
                    // case RegexPattern.EventTypeEnum.NotificationEvent:
                        // _notificationEvent.Value = m.Value;
                        // break;
                    case RegexPattern.EventTypeEnum.TookScreenshot:
                        _tookScreenshot.Value = m.Value;
                        break;
                    case RegexPattern.EventTypeEnum.None:
                    default:
                        break;
                }


            });
        }
    }
}
