using System;
using System.ComponentModel;
using Reactive.Bindings;
using System.Reactive.Linq;

namespace VRChatLogEventOSC
{
    public sealed class OSCSender : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // public ReactiveProperty<string> DatetimeInfo {get;}
    
        // public ReactiveProperty<string> ReceivedInviteInfo {get;}
        // public ReactiveProperty<string> ReceivedRequestInviteInfo {get;}
        // public ReactiveProperty<string> SendInviteInfo {get;}
        // public ReactiveProperty<string> SendRequestInviteInfo {get;}
        // public ReactiveProperty<string> MetPlayerInfo {get;}
        // public ReactiveProperty<string> JoinedRoom1Info {get;}
        public ReactiveProperty<string> JoinedRoom2Info {get;}
        // public ReactiveProperty<string> SendFriendRequestInfo {get;}
        // public ReactiveProperty<string> ReceivedFriendRequestInfo {get;}
        public ReactiveProperty<string> AcceptFriendRequestInfo {get;}
        // public ReactiveProperty<string> ReceivedInviteResponseInfo {get;}
        // public ReactiveProperty<string> ReceivedRequestInviteResponseInfo {get;}
        public ReactiveProperty<string> PlayedVideo1Info {get;}
        public ReactiveProperty<string> PlayedVideo2Info {get;}
        public ReactiveProperty<string> AcceptInviteInfo {get;}
        public ReactiveProperty<string> AcceptRequestInviteInfo {get;}
    
        public ReactiveProperty<string> JoinLeftInfo {get;}
        public ReactiveProperty<string> JoinedRoom1DetailInfo {get;}
        public ReactiveProperty<string> AcceptInviteDetailInfo {get;}
        // public ReactiveProperty<string> NotificationEventInfo {get;}
        public ReactiveProperty<string> TookScreenshotInfo {get;}
        public OSCSender()
        {
            // DatetimeInfo = new(string.Empty);
        
            // ReceivedInviteInfo = new(string.Empty);
            // ReceivedRequestInviteInfo = new(string.Empty);
            // SendInviteInfo = new(string.Empty);
            // SendRequestInviteInfo = new(string.Empty);
            // MetPlayerInfo = new(string.Empty);
            // JoinedRoom1Info = new(string.Empty);
            JoinedRoom2Info = new(string.Empty);
            // SendFriendRequestInfo = new(string.Empty);
            // ReceivedFriendRequestInfo = new(string.Empty);
            AcceptFriendRequestInfo = new(string.Empty);
            // ReceivedInviteResponseInfo = new(string.Empty);
            // ReceivedRequestInviteResponseInfo = new(string.Empty);
            PlayedVideo1Info = new(string.Empty);
            PlayedVideo2Info = new(string.Empty);
            AcceptInviteInfo = new(string.Empty);
            AcceptRequestInviteInfo = new(string.Empty);
        
            JoinLeftInfo = new(string.Empty);
            JoinedRoom1DetailInfo = new(string.Empty);
            AcceptInviteDetailInfo = new(string.Empty);
            // NotificationEventInfo = new(string.Empty);
            TookScreenshotInfo = new(string.Empty);
        }
    }
}
