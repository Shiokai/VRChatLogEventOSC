using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace VRChatLogEventOSC
{
    public static class RegexPattern
    {
        public enum EventTypeEnum
        {
            None = -1,
            ReceivedInvite,
            ReceivedRequestInvite,
            SendInvite,
            SendRequestInvite,
            MetPlayer,
            JoinedRoom1,
            JoinedRoom2,
            SendFriendRequest,
            ReceivedFriendRequest,
            AcceptFriendRequest,
            ReceivedInviteResponse,
            ReceivedRequestInviteResponse,
            PlayedVideo1,
            PlayedVideo2,
            AcceptInvite,
            AcceptRequestInvite,
            OnPlayerJoined,
            OnPlayerLeft,
            JoinedRoom1Detail,
            AcceptInviteDetail,
            NotificationEvent,
            TookScreenshot,
        }
        private static Dictionary<string, EventTypeEnum> EventType => new(){
            {"ReceivedInvite", EventTypeEnum.ReceivedInvite},
            {"ReceivedRequestInvite", EventTypeEnum.ReceivedRequestInvite},
            {"SendInvite", EventTypeEnum.SendInvite},
            {"SendRequestInvite", EventTypeEnum.SendRequestInvite},
            {"MetPlayer", EventTypeEnum.MetPlayer},
            {"JoinedRoom1", EventTypeEnum.JoinedRoom1},
            {"JoinedRoom2", EventTypeEnum.JoinedRoom2},
            {"SendFriendRequest", EventTypeEnum.SendFriendRequest},
            {"ReceivedFriendRequest", EventTypeEnum.ReceivedFriendRequest},
            {"AcceptFriendRequest", EventTypeEnum.AcceptFriendRequest},
            {"ReceivedInviteResponse", EventTypeEnum.ReceivedInviteResponse},
            {"ReceivedRequestInviteResponse", EventTypeEnum.ReceivedRequestInviteResponse},
            {"PlayedVideo1", EventTypeEnum.PlayedVideo1},
            {"PlayedVideo2", EventTypeEnum.PlayedVideo2},
            {"AcceptInvite", EventTypeEnum.AcceptInvite},
            {"AcceptRequestInvite", EventTypeEnum.AcceptRequestInvite},
            {"OnPlayerJoined", EventTypeEnum.OnPlayerJoined},
            {"OnPlayerLeft", EventTypeEnum.OnPlayerLeft},
            {"JoinedRoom1Detail", EventTypeEnum.JoinedRoom1Detail},
            {"AcceptInviteDetail", EventTypeEnum.AcceptInviteDetail},
            {"NotificationEvent", EventTypeEnum.NotificationEvent},
            {"TookScreenshot", EventTypeEnum.TookScreenshot},
        };

        public static EventTypeEnum GetMatchGropeType(Match match)
        {
            return EventType.Keys.Where(k => match.Groups[k].Value.Length != 0).Select(k => EventType[k]).FirstOrDefault(EventTypeEnum.None);
        }
        public static Regex AnyEventRegex { get; }
        public static Regex DatetimeRegex { get; }

        // public static Regex ReceivedInviteRegex {get;}
        // public static Regex ReceivedRequestInviteRegex {get;}
        // public static Regex SendInviteRegex {get;}
        // public static Regex SendRequestInviteRegex {get;}
        // public static Regex MetPlayerRegex {get;}
        // public static Regex JoinedRoom1Regex {get;}
        public static Regex JoinedRoom2Regex { get; }
        // public static Regex SendFriendRequestRegex {get;}
        // public static Regex ReceivedFriendRequestRegex {get;}
        public static Regex AcceptFriendRequestRegex { get; }
        // public static Regex ReceivedInviteResponseRegex {get;}
        // public static Regex ReceivedRequestInviteResponseRegex {get;}
        public static Regex PlayedVideo1Regex { get; }
        public static Regex PlayedVideo2Regex { get; }
        public static Regex AcceptInviteRegex { get; }
        public static Regex AcceptRequestInviteRegex { get; }

        public static Regex OnPlayerJoinedRegex { get; }
        public static Regex OnPlayerLeftRegex { get; }
        public static Regex JoinedRoom1DetailRegex { get; }
        public static Regex AcceptInviteDetailRegex { get; }
        // public static Regex NotificationEventRegex {get;}
        public static Regex TookScreenshotRegex { get; }

        static RegexPattern()
        {
            // Regex is refered to VRChatActivityTools
            // Component Name: VRChatActivityTools
            // URL: https://github.com/nukora/VRChatActivityTools
            // License Type: "MIT"
            // License URL: https://github.com/nukora/VRChatActivityTools/blob/master/LICENSE

            // Regex URL: https://github.com/nukora/VRChatActivityTools/blob/master/VRChatActivityLogger/VRChatActivityLogger/RegexPatterns.cs
            // About log type currently not output URL: https://github.com/nukora/VRChatActivityTools/issues/13

            string datetimePattern = @"^[0-9]{4}\.[0-9]{2}\.[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2} Log {8}- {2}";

            // string receivedInvitePattern = @"Received Notification: <Notification from username:(.+), sender user id:(.{40}).+ of type: invite, id: (.{40}).+worldId=(.+), worldName=(.+?)(, inviteMessage=(.+?))?(, imageUrl=(.+?))?}}, type:invite,.+$";
            // string receivedRequestInvitePattern = @"Received Notification: <Notification from username:(.+), sender user id:(.{40}).+ of type: requestInvite, id: (.{40}),.+{{(requestMessage=(.+?))?,? ?(imageUrl=(.+?))??}}, type:requestInvite,.+$";
            // string sendInvitePattern = @"Send notification:.+sender user.+ to (.{40}).+worldId=([^,]+),.+worldName=(.+?)(, messageSlot=.+)?}}, type:invite,.+message: ""(.+)?"".+$";
            // string sendRequestInvitePattern = @"Send notification:.+sender user.+ to (.{40}).+type:requestInvite,.+message: ""(.+)?"".+$";
            // string metPlayerPattern = @"\[(Player|[Ǆǅ]*|Behaviour)\] Initialized PlayerAPI ""(.*)"" is (remote|local)$";
            // string joinedRoom1Pattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining (?<WorldURL>(.+))$";
            string joinedRoom2Pattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining or Creating Room: (?<WorldName>(.+))$";
            // string sendFriendRequestPattern = @"Send notification:.+sender user.+ to (.{40}).+type:friendRequest,.+$";
            // string receivedFriendRequestPattern = @"Received Notification: <Notification from username:(.+), sender user id:(.{40}).+ of type: friendRequest, id: (.{40}),.+type:friendRequest,.+$";
            string acceptFriendRequestPattern = @"AcceptNotification for notification:<Notification from username:(?<UserName>(.+)), sender user id:(?<UserID>(.{40})).+ of type: friendRequest, id: (.{40}),.+type:friendRequest,.+$";
            // string receivedInviteResponsePattern = @"Received Notification: <Notification from username:(.+), sender user id:(.{40}).+ of type: inviteResponse, id: (.{40}).+{{.+?(, responseMessage=(.+?))?(, imageUrl=(.+?))?}}, type:inviteResponse,.+$";
            // string receivedRequestInviteResponsePattern = @"Received Notification: <Notification from username:(.+), sender user id:(.{40}).+ of type: requestInviteResponse, id: (.{40}).+{{.+?(responseMessage=(.+?))?(, imageUrl=(.+?))?}}, type:requestInviteResponse,.+$";
            string playedVideo1Pattern = @"User (.+) added URL (?<URL>(.+))$";
            string playedVideo2Pattern = @"\[Video Playback\] Attempting to resolve URL '(?<URL>(.+))'$";
            string acceptInvitePattern = @"AcceptNotification for notification:<Notification from username:(?<UserName>(.+)), sender user id:(?<UserID>(.{40})).+ of type: invite, id: (.{40}).+worldId=(?<WorldURL>(.+)), worldName=(?<WorldName>(.+?))(, inviteMessage=(?<Message>(.+?)))?(, imageUrl=(.+?))?}}, type:invite,.+$";
            string acceptRequestInvitePattern = @"AcceptNotification for notification:<Notification from username:(?<UserName>(.+)), sender user id:(?<UserID>(.{40})).+ of type: requestInvite, id: (.{40}),.+{{(requestMessage=(?<Message>(.+?)))?,? ?(imageUrl=(.+?))??}}, type:requestInvite,.+$";

            string onPlayerJoinedPattern = @"\[(?:Player|[Ǆǅ]*|Behaviour)\] OnPlayerJoined\s(?<DisplayName>.+)$";
            string onPlayerLeftPattern = @"\[(?:Player|[Ǆǅ]*|Behaviour)\] OnPlayerLeft\s(?<DisplayName>.+)$";
            string joinedRoom1DetailPattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining (?<WorldURL>(?<WorldID>wrld_[0-9a-zA-Z-]+):(?<InstaiceID>[0-9]+)?~?(?<Type>((private)|(friends)|hidden))?(\((?<UserID>(.{40}))\))?(?<ReqInv>~canRequestInvite)?~region\((?<Region>.+)\).+)$";
            string acceptInviteDetailPattern = @"AcceptNotification for notification:<Notification from username:(?<UserName>(.+)), sender user id:(?<UserID>(.{40})).+ of type: invite, id: (.{40}).+worldId=(?<WorldURL>(?<WorldID>wrld_[0-9a-zA-Z-]+):(?<InstaiceID>[0-9]+)?~?(?<Type>((private)|(friends)|hidden))?(\((?<WorldUserID>(.{40}))\))?(?<ReqInv>~canRequestInvite)?~region\((?<Region>.+)\).+), worldName=(?<WorldName>(.+?))(, inviteMessage=(?<Message>(.+?)))?(, imageUrl=(.+?))?}}, type:invite,.+$";
            // string notificationEventPattern = @"(?<Transceive>Received|Send|AcceptNotification for) [Nn]otification:.+type:(?<Type>(request)?[Ii]nvite(Response)?|friendRequest),.+$";
            string tookScreenshotPattern = @"\[VRC Camera\] Took screenshot to: (?<Path>(.*))$";
            

            // string receivedInviteSimple = @"Received Notification:.+type:invite,.+$";
            // string receivedRequestInviteSimple = @"Received Notification:.+type:requestInvite,.+$";
            // string sendInviteSimple = @"Send notification:.+type:invite,.+$";
            // string sendRequestInviteSimple = @"Send notification:.+type:requestInvite,.+$";
            string joinedRoom1Simple = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining w.+$";
            string joinedRoom2Simple = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining or Creating Room:.+$";
            // string metPlayerSimple = @"\[(Player|[Ǆǅ]*|Behaviour)\] Initialized PlayerAPI.+$";
            // string sendFriendRequestSimple = @"Send notification:.+type:friendRequest,.+$";
            // string receivedFriendRequestSimple = @"Received Notification:.+type:friendRequest,.+$";
            string acceptFriendRequestSimple = @"AcceptNotification for notification:.+type:friendRequest,.+$";
            // string receivedInviteResponseSimple = @"Received Notification:.+type:inviteResponse,.+$";
            // string receivedRequestInviteResponseSimple = @"Received Notification:.+type:requestInviteResponse,.+$";
            string playedVideo1Simple = @"User .+ added URL .+$";
            string playedVideo2Simple = @"\[Video Playback\] Attempting to resolve URL '.+'$";
            string acceptInviteSimple = @"AcceptNotification for notification:.+type:invite,.+$";
            string acceptRequestInviteSimple = @"AcceptNotification for notification:.+type:requestInvite,.+$";
            
            string onPlayerJoinedSimple = @"\[(?:Player|[Ǆǅ]*|Behaviour)\] OnPlayerJoined\s(.+)$";
            string onPlayerLeftSimple = @"\[(?:Player|[Ǆǅ]*|Behaviour)\] OnPlayerLeft\s(.+)$";
            // string joinedRoom1DetailSimple = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining (?<WorldURL>(?<WorldID>wrld_[0-9a-zA-Z-]+):(?<InstaiceID>[0-9]+)?~?(?<Type>((private)|(friends)|hidden))?(\((?<UserID>(.{40}))\))?(?<ReqInv>~canRequestInvite)?~region\((?<Region>.+)\).+)$";
            // string acceptInviteDetailSimple = @"AcceptNotification for notification:<Notification from username:(?<UserName>(.+)), sender user id:(?<UserID>(.{40})).+ of type: invite, id: (.{40}).+worldId=(?<WorldURL>(?<WorldID>wrld_[0-9a-zA-Z-]+):(?<InstaiceID>[0-9]+)?~?(?<Type>((private)|(friends)|hidden))?(\((?<WorldUserID>(.{40}))\))?(?<ReqInv>~canRequestInvite)?~region\((?<Region>.+)\).+), worldName=(?<WorldName>(.+?))(, inviteMessage=(?<Message>(.+?)))?(, imageUrl=(.+?))?}}, type:invite,.+$";
            // string notificationEventSimple = @"(?<Transceive>Received|Send|AcceptNotification for) [Nn]otification:.+type:(?<Type>(request)?[Ii]nvite(Response)?|friendRequest),.+$";
            string tookScreenshotSimple = @"\[VRC Camera\] Took screenshot to: ((.*))$";

            string anyEventPattern = "("
            // + "?<ReceivedInvite>" +  receivedInviteSimple + ")" + "|(" 
            // + "?<ReceivedRequestInvite>" + receivedRequestInviteSimple + ")" + "|(" 
            // + "?<SendInvite>" + sendInviteSimple + ")" + "|(" 
            // + "?<SendRequestInvite>" + sendRequestInviteSimple + ")" + "|(" 
            // + "?<MetPlayer>" + metPlayerSimple + ")" + "|(" 
            + "?<JoinedRoom1>" + joinedRoom1Simple + ")" + "|(" 
            + "?<JoinedRoom2>" + joinedRoom2Simple + ")" + "|(" 
            // + "?<SendFriendRequest>" + sendFriendRequestSimple + ")" + "|(" 
            // + "?<ReceivedFriendRequest>" + receivedFriendRequestSimple + ")" + "|(" 
            + "?<AcceptFriendRequest>" + acceptFriendRequestSimple + ")" + "|(" 
            // + "?<ReceivedInviteResponse>" + receivedInviteResponseSimple + ")" + "|(" 
            // + "?<ReceivedRequestInviteResponse>" + receivedRequestInviteResponseSimple + ")" + "|(" 
            + "?<PlayedVideo1>" + playedVideo1Simple + ")" + "|(" 
            + "?<PlayedVideo2>" + playedVideo2Simple + ")" + "|(" 
            + "?<AcceptInvite>" + acceptInviteSimple + ")" + "|(" 
            + "?<AcceptRequestInvite>" + acceptRequestInviteSimple + ")" + "|(" 

            + "?<OnPlayerJoined>" + onPlayerJoinedSimple + ")" + "|(" 
            + "?<OnPlayerLeft>" + onPlayerLeftSimple + ")" + "|(" 
            // + "?<JoinedRoom1Detail>" + joinedRoom1DetailSimple + ")" + "|(" 
            // + "?<AcceptInviteDetail>" + acceptInviteDetailSimple + ")" + "|(" 
            // + "?<NotificationEvent>" + notificationEventSimple + ")" + "|(" 
            + "?<TookScreenshot>" + tookScreenshotSimple + ")";
            


            AnyEventRegex = new Regex(anyEventPattern, RegexOptions.Compiled);

            DatetimeRegex = new(datetimePattern, RegexOptions.Compiled);

            // ReceivedInviteRegex = new(receivedInvitePattern, RegexOptions.Compiled);
            // ReceivedRequestInviteRegex = new(receivedRequestInvitePattern, RegexOptions.Compiled);
            // SendInviteRegex = new(sendInvitepattern, RegexOptions.Compiled);
            // SendRequestInviteRegex = new(sendRequestInvitePattern, RegexOptions.Compiled);
            // MetPlayerRegex = new(metPlayerPattern, RegexOptions.Compiled);
            // JoinedRoom1Regex = new(joinedRoom1DetailPattern, RegexOptions.Compiled);
            JoinedRoom2Regex = new(joinedRoom2Pattern, RegexOptions.Compiled);
            // SendFriendRequestRegex = new(sendFriendRequestPattern, RegexOptions.Compiled);
            // ReceivedFriendRequestRegex = new(receivedFriendRequestPattern, RegexOptions.Compiled);
            AcceptFriendRequestRegex = new(acceptFriendRequestPattern, RegexOptions.Compiled);
            // ReceivedInviteResponseRegex = new(receivedInviteResponsePattern, RegexOptions.Compiled);
            // ReceivedRequestInviteResponseRegex = new(receivedRequestInviteResponsePattern, RegexOptions.Compiled);
            PlayedVideo1Regex = new(playedVideo1Pattern, RegexOptions.Compiled);
            PlayedVideo2Regex = new(playedVideo2Pattern, RegexOptions.Compiled);
            AcceptInviteRegex = new(acceptInvitePattern, RegexOptions.Compiled);
            AcceptRequestInviteRegex = new(acceptRequestInvitePattern, RegexOptions.Compiled);

            OnPlayerJoinedRegex = new(onPlayerJoinedPattern, RegexOptions.Compiled);
            OnPlayerLeftRegex = new(onPlayerLeftPattern, RegexOptions.Compiled);
            JoinedRoom1DetailRegex = new(joinedRoom1DetailPattern, RegexOptions.Compiled);
            AcceptInviteDetailRegex = new(acceptInviteDetailPattern, RegexOptions.Compiled);
            // NotificationEventRegex = new(notificationEventPattern, RegexOptions.Compiled);
            TookScreenshotRegex = new(tookScreenshotPattern, RegexOptions.Compiled);

        }

    }


}

