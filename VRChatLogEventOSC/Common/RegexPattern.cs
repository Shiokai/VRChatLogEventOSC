using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VRChatLogEventOSC.Common
{
    internal static class RegexPattern
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
            SuccessfullyLeftRoom,
            FinishedEnteringWorld,
            Rejoining,
            GoHome,
        }

        /// <summary>
        /// イベントから取得可能な名前付きグループ
        /// </summary>
        [Flags]
        public enum Captures
        {
            None = 0,
            UserName = 1 << 0,
            UseID = 1 << 1,
            WorldURL = 1 << 2,
            WorldName = 1 << 3,
            Message = 1 << 4,
            URL = 1 << 5,
        }

        /// <summary>
        /// 各イベントとそのイベントで使用されている名前付きグループの対応
        /// </summary>
        /// <returns></returns>
        private static readonly Dictionary<EventTypeEnum, Captures> EventCapture = new()
        {
            {EventTypeEnum.JoinedRoomURL, Captures.WorldURL},
            {EventTypeEnum.JoinedRoomName, Captures.WorldName},
            {EventTypeEnum.AcceptFriendRequest, Captures.UserName | Captures.UseID},
            {EventTypeEnum.PlayedVideo1, Captures.URL},
            {EventTypeEnum.PlayedVideo2, Captures.URL},
            {EventTypeEnum.AcceptInvite, Captures.UserName | Captures.UseID | Captures.WorldURL | Captures.WorldName | Captures.Message},
            {EventTypeEnum.AcceptRequestInvite, Captures.UserName | Captures.UseID | Captures.Message},
            {EventTypeEnum.OnPlayerJoined, Captures.UserName},
            {EventTypeEnum.OnPlayerLeft, Captures.UserName},
            {EventTypeEnum.TookScreenshot, Captures.None},
            {EventTypeEnum.SuccessfullyLeftRoom, Captures.None},
            {EventTypeEnum.FinishedEnteringWorld, Captures.None},
            {EventTypeEnum.Rejoining, Captures.WorldURL},
            {EventTypeEnum.GoHome, Captures.None},
        };

        private static readonly IReadOnlyDictionary<EventTypeEnum, Regex> _regexes;
        public static IReadOnlyDictionary<EventTypeEnum, Regex> Regexes => _regexes;

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
        public static Regex SuccessfullyLeftRoomRegex { get; }
        public static Regex FinishedEnteringWorldRegex { get; }
        public static Regex RejoiningRegex { get; }
        public static Regex GoHomeRegex { get; }

        /// <summary>
        /// 名前付きグループのフラグを名前付きグループの名前の一覧に変換します。
        /// </summary>
        /// <param name="captures">名前の一覧を取得する名前付きグループのフラグ</param>
        /// <returns>対応する名前付きグループの名前の一覧</returns>
        private static IEnumerable<string> CapturesToCaptureName(Captures captures)
        {
            var names = new List<string>();

            if (captures.HasFlag(Captures.UserName))
            {
                names.Add("UserName");
            }

            if (captures.HasFlag(Captures.UseID))
            {
                names.Add("UserID");
            }

            if (captures.HasFlag(Captures.WorldURL))
            {
                names.AddRange(new[]{"WorldURL", "WorldID", "InstanceID", "InstanceType", "WorldUserID", "ReqInv", "Region"});
            }

            if (captures.HasFlag(Captures.WorldName))
            {
                names.Add("WorldName");
            }

            if (captures.HasFlag(Captures.Message))
            {
                names.Add("Message");
            }

            if (captures.HasFlag(Captures.URL))
            {
                names.Add("URL");
            }

            return names;
        }

        /// <summary>
        /// イベントの正規表現の名前付きグループの名前の一覧を取得します
        /// </summary>
        /// <param name="eventType">名前付きグループの名前を取得するイベント</param>
        /// <returns>名前付きグループの名前の一覧</returns>
        public static IEnumerable<string> CaptureNames(EventTypeEnum eventType) => CapturesToCaptureName(EventCapture[eventType]);

        /// <summary>
        /// 正規表現へのマッチがどのイベントのものかを取得します
        /// </summary>
        /// <param name="match">イベントを調べる正規表現へのマッチ</param>
        /// <returns>対応するイベント</returns>
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

            string joinedRoomURLPattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining (?<WorldURL>(?<WorldID>wrld_[0-9a-zA-Z-]+):(?<InstanceID>[0-9]+)?~?(?<InstanceType>((private)|(friends)|hidden))?(\((?<WorldUserID>(.{40}))\))?(?<ReqInv>~canRequestInvite)?(~region\((?<Region>.+)\))?.+)$";
            string joinedRoomNamePattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining or Creating Room: (?<WorldName>(.+))$";
            string acceptFriendRequestPattern = @"AcceptNotification for notification:<Notification from username:(?<UserName>(.+)), sender user id:(?<UserID>(.{40})).+ of type: friendRequest, id: (.{40}),.+type:friendRequest,.+$";
            string playedVideo1Pattern = @"User (.+) added URL (?<URL>(.+))$";
            string playedVideo2Pattern = @"\[Video Playback\] Attempting to resolve URL '(?<URL>(.+))'$";
            string acceptInvitePattern = @"AcceptNotification for notification:<Notification from username:(?<UserName>(.+)), sender user id:(?<UserID>(.{40})).+ of type: invite, id: (.{40}).+worldId=(?<WorldURL>(?<WorldID>wrld_[0-9a-zA-Z-]+):(?<InstanceID>[0-9]+)?~?(?<InstanceType>((private)|(friends)|hidden))?(\((?<WorldUserID>(.{40}))\))?(?<ReqInv>~canRequestInvite)?(~region\((?<Region>.+)\))?.+), worldName=(?<WorldName>(.+?))(, inviteMessage=(?<Message>(.+?)))?(, imageUrl=(.+?))?\}\}, type:invite,.+$";
            string acceptRequestInvitePattern = @"AcceptNotification for notification:<Notification from username:(?<UserName>(.+)), sender user id:(?<UserID>(.{40})).+ of type: requestInvite, id: (.{40}),.+\{\{(requestMessage=(?<Message>(.+?)))?,? ?(imageUrl=(.+?))??\}\}, type:requestInvite,.+$";
            string onPlayerJoinedPattern = @"\[(?:Player|[Ǆǅ]*|Behaviour)\] OnPlayerJoined\s(?<UserName>.+)$";
            string onPlayerLeftPattern = @"\[(?:Player|[Ǆǅ]*|Behaviour)\] OnPlayerLeft\s(?<UserName>.+)$";
            string tookScreenshotPattern = @"\[VRC Camera\] Took screenshot to: (?<Path>(.*))$";
            // string testPattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Going to current users Home world$";
            string successfullyLeftRoomPattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Successfully left room$";
            string finishedEnteringWorldPattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Finished entering world.$";
            string rejoiningPattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Rejoining local world: (?<WorldURL>(?<WorldID>wrld_[0-9a-zA-Z-]+):(?<InstanceID>[0-9]+)?~?(?<InstanceType>((private)|(friends)|hidden))?(\((?<WorldUserID>(.{40}))\))?(?<ReqInv>~canRequestInvite)?(~region\((?<Region>.+)\))?.+)$";
            string goHomePattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Going to current users Home world$";

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
            // string testSimple = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Going to current users Home world$";
            string successfullyLeftRoomSimple = successfullyLeftRoomPattern;
            string finishedEnteringWorldSimple = finishedEnteringWorldPattern;
            string rejoiningSimple = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Rejoining local world: w.+$";
            string goHomeSimple = goHomePattern;


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
            + $"?<{nameof(EventTypeEnum.TookScreenshot)}>" + tookScreenshotSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.SuccessfullyLeftRoom)}>" + successfullyLeftRoomSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.FinishedEnteringWorld)}>" + finishedEnteringWorldSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.Rejoining)}>" + rejoiningSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.GoHome)}>" + goHomeSimple + ")";



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
            SuccessfullyLeftRoomRegex = new(successfullyLeftRoomPattern, RegexOptions.Compiled);
            FinishedEnteringWorldRegex = new(finishedEnteringWorldPattern, RegexOptions.Compiled);
            RejoiningRegex = new(rejoiningPattern, RegexOptions.Compiled);
            GoHomeRegex = new(goHomePattern, RegexOptions.Compiled);

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
                {EventTypeEnum.SuccessfullyLeftRoom, SuccessfullyLeftRoomRegex},
                {EventTypeEnum.FinishedEnteringWorld, FinishedEnteringWorldRegex},
                {EventTypeEnum.Rejoining, RejoiningRegex},
                {EventTypeEnum.GoHome, GoHomeRegex},
            };

        }

    }


}

