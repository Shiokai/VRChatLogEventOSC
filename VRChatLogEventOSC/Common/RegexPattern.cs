using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace VRChatLogEventOSC.Common
{
    public static class RegexPattern
    {
        public enum EventTypeEnum
        {
            None = -1,
            JoinedRoomURL,
            JoinedRoomName,
            AcceptFriendRequest,
            PlayedVideo1,
            PlayedVideo2,
            AcceptInvite,
            AcceptRequestInvite,
            OnPlayerJoined,
            OnPlayerLeft,
            TookScreenshot,
        }
        private static readonly Dictionary<EventTypeEnum, IEnumerable<string>> CaptureName = new(){
            {EventTypeEnum.JoinedRoomURL, new[]{"WorldURL", "WorldID", "InstanceID", "InstanceType", "WorldUserID", "ReqInv", "Region"}},
            {EventTypeEnum.JoinedRoomName, new[]{"WorldName"}},
            {EventTypeEnum.AcceptFriendRequest, new[]{"UserName", "UserID"}},
            {EventTypeEnum.PlayedVideo1, new[]{"URL"}},
            {EventTypeEnum.PlayedVideo2, new[]{"URL"}},
            {EventTypeEnum.AcceptInvite, new[]{"UserName", "UserID","WorldURL","WorldID","InstanceID","InstanceType","WorldUserID","ReqInv","Region","WorldName","Message",}},
            {EventTypeEnum.AcceptRequestInvite, new[]{"UserName", "UserID", "Message"}},
            {EventTypeEnum.OnPlayerJoined, new[]{"DisplayName"}},
            {EventTypeEnum.OnPlayerLeft, new[]{"DisplayName"}},
            {EventTypeEnum.TookScreenshot, Enumerable.Empty<string>()},

        };

        public static IEnumerable<string> CaptureNames(EventTypeEnum eventType) => CaptureName[eventType];

        public static EventTypeEnum GetMatchGropeType(Match match)
        {
            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }

                if (match.Groups[type.ToString()].Value.Length == 0)
                {
                    continue;
                }

                return type;
            }

            return EventTypeEnum.None;
        }
        public static Regex AnyEventRegex { get; }
        public static Regex DatetimeRegex { get; }
        public static Regex JoinedRoomURLRegex { get; }
        public static Regex JoinedRoomNameRegex { get; }
        public static Regex AcceptFriendRequestRegex { get; }
        public static Regex PlayedVideo1Regex { get; }
        public static Regex PlayedVideo2Regex { get; }
        public static Regex AcceptInviteRegex { get; }
        public static Regex AcceptRequestInviteRegex { get; }
        public static Regex OnPlayerJoinedRegex { get; }
        public static Regex OnPlayerLeftRegex { get; }
        public static Regex TookScreenshotRegex { get; }

        private static readonly IReadOnlyDictionary<EventTypeEnum, Regex> _regexes;

        public static IReadOnlyDictionary<EventTypeEnum, Regex> Regexes => _regexes;

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

            string joinedRoomURLPattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining (?<WorldURL>(?<WorldID>wrld_[0-9a-zA-Z-]+):(?<InstanceID>[0-9]+)?~?(?<InstanceType>((private)|(friends)|hidden))?(\((?<WorldUserID>(.{40}))\))?(?<ReqInv>~canRequestInvite)?~region\((?<Region>.+)\).+)$";
            string joinedRoomNamePattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining or Creating Room: (?<WorldName>(.+))$";
            string acceptFriendRequestPattern = @"AcceptNotification for notification:<Notification from username:(?<UserName>(.+)), sender user id:(?<UserID>(.{40})).+ of type: friendRequest, id: (.{40}),.+type:friendRequest,.+$";
            string playedVideo1Pattern = @"User (.+) added URL (?<URL>(.+))$";
            string playedVideo2Pattern = @"\[Video Playback\] Attempting to resolve URL '(?<URL>(.+))'$";
            string acceptInvitePattern = @"AcceptNotification for notification:<Notification from username:(?<UserName>(.+)), sender user id:(?<UserID>(.{40})).+ of type: invite, id: (.{40}).+worldId=(?<WorldURL>(?<WorldID>wrld_[0-9a-zA-Z-]+):(?<InstanceID>[0-9]+)?~?(?<InstanceType>((private)|(friends)|hidden))?(\((?<WorldUserID>(.{40}))\))?(?<ReqInv>~canRequestInvite)?~region\((?<Region>.+)\).+), worldName=(?<WorldName>(.+?))(, inviteMessage=(?<Message>(.+?)))?(, imageUrl=(.+?))?\}\}, type:invite,.+$";
            string acceptRequestInvitePattern = @"AcceptNotification for notification:<Notification from username:(?<UserName>(.+)), sender user id:(?<UserID>(.{40})).+ of type: requestInvite, id: (.{40}),.+\{\{(requestMessage=(?<Message>(.+?)))?,? ?(imageUrl=(.+?))??\}\}, type:requestInvite,.+$";
            string onPlayerJoinedPattern = @"\[(?:Player|[Ǆǅ]*|Behaviour)\] OnPlayerJoined\s(?<DisplayName>.+)$";
            string onPlayerLeftPattern = @"\[(?:Player|[Ǆǅ]*|Behaviour)\] OnPlayerLeft\s(?<DisplayName>.+)$";
            string tookScreenshotPattern = @"\[VRC Camera\] Took screenshot to: (?<Path>(.*))$";


            string joinedRoomURLSimple = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining w.+$";
            string joinedRoomNameSimple = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining or Creating Room:.+$";
            string acceptFriendRequestSimple = @"AcceptNotification for notification:.+type:friendRequest,.+$";
            string playedVideo1Simple = @"User .+ added URL .+$";
            string playedVideo2Simple = @"\[Video Playback\] Attempting to resolve URL '.+'$";
            string acceptInviteSimple = @"AcceptNotification for notification:.+type:invite,.+$";
            string acceptRequestInviteSimple = @"AcceptNotification for notification:.+type:requestInvite,.+$";
            string onPlayerJoinedSimple = @"\[(?:Player|[Ǆǅ]*|Behaviour)\] OnPlayerJoined\s(.+)$";
            string onPlayerLeftSimple = @"\[(?:Player|[Ǆǅ]*|Behaviour)\] OnPlayerLeft\s(.+)$";
            string tookScreenshotSimple = @"\[VRC Camera\] Took screenshot to: ((.*))$";

            string anyEventPattern = "("
            + $"?<{nameof(EventTypeEnum.JoinedRoomURL)}>" + joinedRoomURLSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.JoinedRoomName)}>" + joinedRoomNameSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.AcceptFriendRequest)}>" + acceptFriendRequestSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.PlayedVideo1)}>" + playedVideo1Simple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.PlayedVideo2)}>" + playedVideo2Simple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.AcceptInvite)}>" + acceptInviteSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.AcceptRequestInvite)}>" + acceptRequestInviteSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.OnPlayerJoined)}>" + onPlayerJoinedSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.OnPlayerLeft)}>" + onPlayerLeftSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.TookScreenshot)}>" + tookScreenshotSimple + ")";



            AnyEventRegex = new Regex(anyEventPattern, RegexOptions.Compiled);

            DatetimeRegex = new(datetimePattern, RegexOptions.Compiled);

            JoinedRoomURLRegex = new(joinedRoomURLPattern, RegexOptions.Compiled);
            JoinedRoomNameRegex = new(joinedRoomNamePattern, RegexOptions.Compiled);
            AcceptFriendRequestRegex = new(acceptFriendRequestPattern, RegexOptions.Compiled);
            PlayedVideo1Regex = new(playedVideo1Pattern, RegexOptions.Compiled);
            PlayedVideo2Regex = new(playedVideo2Pattern, RegexOptions.Compiled);
            AcceptInviteRegex = new(acceptInvitePattern, RegexOptions.Compiled);
            AcceptRequestInviteRegex = new(acceptRequestInvitePattern, RegexOptions.Compiled);
            OnPlayerJoinedRegex = new(onPlayerJoinedPattern, RegexOptions.Compiled);
            OnPlayerLeftRegex = new(onPlayerLeftPattern, RegexOptions.Compiled);
            TookScreenshotRegex = new(tookScreenshotPattern, RegexOptions.Compiled);


            _regexes = new Dictionary<EventTypeEnum, Regex>()
            {
                {EventTypeEnum.JoinedRoomURL, JoinedRoomURLRegex},
                {EventTypeEnum.JoinedRoomName, JoinedRoomNameRegex},
                {EventTypeEnum.AcceptFriendRequest, AcceptFriendRequestRegex},
                {EventTypeEnum.PlayedVideo1, PlayedVideo1Regex},
                {EventTypeEnum.PlayedVideo2, PlayedVideo2Regex},
                {EventTypeEnum.AcceptInvite, AcceptInviteRegex},
                {EventTypeEnum.AcceptRequestInvite, AcceptRequestInviteRegex},
                {EventTypeEnum.OnPlayerJoined, OnPlayerJoinedRegex},
                {EventTypeEnum.OnPlayerLeft, OnPlayerLeftRegex},
                {EventTypeEnum.TookScreenshot, TookScreenshotRegex},
            };

        }

    }


}

