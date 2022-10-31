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
            JoiningRoomURL,
            JoiningRoomName,
            AcceptFriendRequest,
            PlayedVideo,
            AcceptInvite,
            AcceptRequestInvite,
            OnPlayerJoined,
            OnPlayerLeft,
            TookScreenshot,
            LeftRoom,
            EnteredWorld,
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
            {EventTypeEnum.JoiningRoomURL, Captures.WorldURL},
            {EventTypeEnum.JoiningRoomName, Captures.WorldName},
            {EventTypeEnum.AcceptFriendRequest, Captures.UserName | Captures.UseID},
            {EventTypeEnum.PlayedVideo, Captures.URL},
            {EventTypeEnum.AcceptInvite, Captures.UserName | Captures.UseID | Captures.WorldURL | Captures.WorldName | Captures.Message},
            {EventTypeEnum.AcceptRequestInvite, Captures.UserName | Captures.UseID | Captures.Message},
            {EventTypeEnum.OnPlayerJoined, Captures.UserName},
            {EventTypeEnum.OnPlayerLeft, Captures.UserName},
            {EventTypeEnum.TookScreenshot, Captures.None},
            {EventTypeEnum.LeftRoom, Captures.None},
            {EventTypeEnum.EnteredWorld, Captures.None},
            {EventTypeEnum.Rejoining, Captures.WorldURL},
            {EventTypeEnum.GoHome, Captures.None},
        };

        private static readonly IReadOnlyDictionary<EventTypeEnum, Regex> _regexes;
        public static IReadOnlyDictionary<EventTypeEnum, Regex> Regexes => _regexes;

        public static Regex AnyEventRegex { get; }
        public static Regex DatetimeRegex { get; }
        public static Regex JoiningRoomURLRegex { get; }
        public static Regex JoiningRoomNameRegex { get; }
        public static Regex AcceptFriendRequestRegex { get; }
        public static Regex PlayedVideoRegex { get; }
        public static Regex AcceptInviteRegex { get; }
        public static Regex AcceptRequestInviteRegex { get; }
        public static Regex OnPlayerJoinedRegex { get; }
        public static Regex OnPlayerLeftRegex { get; }
        public static Regex TookScreenshotRegex { get; }
        public static Regex LeftRoomRegex { get; }
        public static Regex EnteredWorldRegex { get; }
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

            string joiningRoomURLPattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining (?<WorldURL>(?<WorldID>wrld_[0-9a-zA-Z-]+):(?<InstanceID>[0-9]+)?~?(?<InstanceType>((private)|(friends)|hidden))?(\((?<WorldUserID>(.{40}))\))?(?<ReqInv>~canRequestInvite)?(~region\((?<Region>.+)\))?.+)$";
            string joiningRoomNamePattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining or Creating Room: (?<WorldName>(.+))$";
            string acceptFriendRequestPattern = @"AcceptNotification for notification:<Notification from username:(?<UserName>(.+)), sender user id:(?<UserID>(.{40})).+ of type: friendRequest, id: (.{40}),.+type:friendRequest,.+$";
            string playedVideoPattern = @"\[Video Playback\] Attempting to resolve URL '(?<URL>(.+))'$";
            string acceptInvitePattern = @"AcceptNotification for notification:<Notification from username:(?<UserName>(.+)), sender user id:(?<UserID>(.{40})).+ of type: invite, id: (.{40}).+worldId=(?<WorldURL>(?<WorldID>wrld_[0-9a-zA-Z-]+):(?<InstanceID>[0-9]+)?~?(?<InstanceType>((private)|(friends)|hidden))?(\((?<WorldUserID>(.{40}))\))?(?<ReqInv>~canRequestInvite)?(~region\((?<Region>.+)\))?.+), worldName=(?<WorldName>(.+?))(, inviteMessage=(?<Message>(.+?)))?(, imageUrl=(.+?))?\}\}, type:invite,.+$";
            string acceptRequestInvitePattern = @"AcceptNotification for notification:<Notification from username:(?<UserName>(.+)), sender user id:(?<UserID>(.{40})).+ of type: requestInvite, id: (.{40}),.+\{\{(requestMessage=(?<Message>(.+?)))?,? ?(imageUrl=(.+?))??\}\}, type:requestInvite,.+$";
            string onPlayerJoinedPattern = @"\[(?:Player|[Ǆǅ]*|Behaviour)\] OnPlayerJoined\s(?<UserName>.+)$";
            string onPlayerLeftPattern = @"\[(?:Player|[Ǆǅ]*|Behaviour)\] OnPlayerLeft\s(?<UserName>.+)$";
            string tookScreenshotPattern = @"\[VRC Camera\] Took screenshot to: (?<Path>(.*))$";
            string leftRoomPattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Successfully left room$";
            string enteredWorldPattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Finished entering world.$";
            string rejoiningPattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Rejoining local world: (?<WorldURL>(?<WorldID>wrld_[0-9a-zA-Z-]+):(?<InstanceID>[0-9]+)?~?(?<InstanceType>((private)|(friends)|hidden))?(\((?<WorldUserID>(.{40}))\))?(?<ReqInv>~canRequestInvite)?(~region\((?<Region>.+)\))?.+)$";
            string goHomePattern = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Going to current users Home world$";

            string joiningRoomURLSimple = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining w.+$";
            string joiningRoomNameSimple = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Joining or Creating Room:.+$";
            string acceptFriendRequestSimple = @"AcceptNotification for notification:.+type:friendRequest,.+$";
            string playedVideoSimple = @"\[Video Playback\] Attempting to resolve URL '.+'$";
            string acceptInviteSimple = @"AcceptNotification for notification:.+type:invite,.+$";
            string acceptRequestInviteSimple = @"AcceptNotification for notification:.+type:requestInvite,.+$";
            string onPlayerJoinedSimple = @"\[(?:Player|[Ǆǅ]*|Behaviour)\] OnPlayerJoined\s(.+)$";
            string onPlayerLeftSimple = @"\[(?:Player|[Ǆǅ]*|Behaviour)\] OnPlayerLeft\s(.+)$";
            string tookScreenshotSimple = @"\[VRC Camera\] Took screenshot to: ((.*))$";
            string leftRoomSimple = leftRoomPattern;
            string enteredWorldSimple = enteredWorldPattern;
            string rejoiningSimple = @"\[(RoomManager|[Ǆǅ]*|Behaviour)\] Rejoining local world: w.+$";
            string goHomeSimple = goHomePattern;


            string anyEventPattern = "("
            + $"?<{nameof(EventTypeEnum.JoiningRoomURL)}>" + joiningRoomURLSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.JoiningRoomName)}>" + joiningRoomNameSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.AcceptFriendRequest)}>" + acceptFriendRequestSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.PlayedVideo)}>" + playedVideoSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.AcceptInvite)}>" + acceptInviteSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.AcceptRequestInvite)}>" + acceptRequestInviteSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.OnPlayerJoined)}>" + onPlayerJoinedSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.OnPlayerLeft)}>" + onPlayerLeftSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.TookScreenshot)}>" + tookScreenshotSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.LeftRoom)}>" + leftRoomSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.EnteredWorld)}>" + enteredWorldSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.Rejoining)}>" + rejoiningSimple + ")" + "|("
            + $"?<{nameof(EventTypeEnum.GoHome)}>" + goHomeSimple + ")";



            AnyEventRegex = new Regex(anyEventPattern, RegexOptions.Compiled);

            DatetimeRegex = new(datetimePattern, RegexOptions.Compiled);

            JoiningRoomURLRegex = new(joiningRoomURLPattern, RegexOptions.Compiled);
            JoiningRoomNameRegex = new(joiningRoomNamePattern, RegexOptions.Compiled);
            AcceptFriendRequestRegex = new(acceptFriendRequestPattern, RegexOptions.Compiled);
            PlayedVideoRegex = new(playedVideoPattern, RegexOptions.Compiled);
            AcceptInviteRegex = new(acceptInvitePattern, RegexOptions.Compiled);
            AcceptRequestInviteRegex = new(acceptRequestInvitePattern, RegexOptions.Compiled);
            OnPlayerJoinedRegex = new(onPlayerJoinedPattern, RegexOptions.Compiled);
            OnPlayerLeftRegex = new(onPlayerLeftPattern, RegexOptions.Compiled);
            TookScreenshotRegex = new(tookScreenshotPattern, RegexOptions.Compiled);
            LeftRoomRegex = new(leftRoomPattern, RegexOptions.Compiled);
            EnteredWorldRegex = new(enteredWorldPattern, RegexOptions.Compiled);
            RejoiningRegex = new(rejoiningPattern, RegexOptions.Compiled);
            GoHomeRegex = new(goHomePattern, RegexOptions.Compiled);

            _regexes = new Dictionary<EventTypeEnum, Regex>()
            {
                {EventTypeEnum.JoiningRoomURL, JoiningRoomURLRegex},
                {EventTypeEnum.JoiningRoomName, JoiningRoomNameRegex},
                {EventTypeEnum.AcceptFriendRequest, AcceptFriendRequestRegex},
                {EventTypeEnum.PlayedVideo, PlayedVideoRegex},
                {EventTypeEnum.AcceptInvite, AcceptInviteRegex},
                {EventTypeEnum.AcceptRequestInvite, AcceptRequestInviteRegex},
                {EventTypeEnum.OnPlayerJoined, OnPlayerJoinedRegex},
                {EventTypeEnum.OnPlayerLeft, OnPlayerLeftRegex},
                {EventTypeEnum.TookScreenshot, TookScreenshotRegex},
                {EventTypeEnum.LeftRoom, LeftRoomRegex},
                {EventTypeEnum.EnteredWorld, EnteredWorldRegex},
                {EventTypeEnum.Rejoining, RejoiningRegex},
                {EventTypeEnum.GoHome, GoHomeRegex},
            };

        }

    }


}

