using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using static VRChatLogEventOSC.Common.RegexPattern;

namespace VRChatLogEventOSC.Common
{
    internal sealed record class WholeSetting
    {
        public int JsonVersion { get; init; } = 1;

        [JsonIgnore]
        public IReadOnlyDictionary<EventTypeEnum, IReadOnlyList<SingleSetting>> Settings { get; private set; }
        // Dictionaryで読み書きするとKeyに無いイベントが設定されていた時にエラーになるので、読み書き用に個別プロパティを用意
        // イベント追加時の追加忘れ注意
        [JsonInclude]
        public IReadOnlyList<SingleSetting> JoinedRoomURL { private get; init; }
        [JsonInclude]
        public IReadOnlyList<SingleSetting> JoinedRoomName { private get; init; }
        [JsonInclude]
        public IReadOnlyList<SingleSetting> AcceptFriendRequest { private get; init; }
        [JsonInclude]
        public IReadOnlyList<SingleSetting> PlayedVideo1 { private get; init; }
        [JsonInclude]
        public IReadOnlyList<SingleSetting> PlayedVideo2 { private get; init; }
        [JsonInclude]
        public IReadOnlyList<SingleSetting> AcceptInvite { private get; init; }
        [JsonInclude]
        public IReadOnlyList<SingleSetting> AcceptRequestInvite { private get; init; }
        [JsonInclude]
        public IReadOnlyList<SingleSetting> OnPlayerJoined { private get; init; }
        [JsonInclude]
        public IReadOnlyList<SingleSetting> OnPlayerLeft { private get; init; }
        [JsonInclude]
        public IReadOnlyList<SingleSetting> TookScreenshot { private get; init; }
        [JsonInclude]
        public IReadOnlyList<SingleSetting> SuccessfullyLeftRoom { private get; init; }
        [JsonInclude]
        public IReadOnlyList<SingleSetting> FinishedEnteringWorld { private get; init; }
        [JsonInclude]
        public IReadOnlyList<SingleSetting> Rejoining { private get; init; }
        [JsonInclude]
        public IReadOnlyList<SingleSetting> GoHome { private get; init; }

        public override string ToString()
        {
            StringBuilder result = new();

            if (Settings == null)
            {
                return "Null";
            }

            foreach (var kvp in Settings)
            {
                result.Append(kvp.Key.ToString());
                result.Append(": {");
                foreach (var val in kvp.Value)
                {
                    result.Append(val.ToString());
                }
                result.Append("}\n");
            }
            return result.ToString();
        }

        /// <summary>
        /// イベント毎の設定の一覧が空の設定
        /// </summary>
        /// <returns>空の設定</returns>
        public static Dictionary<EventTypeEnum, List<SingleSetting>> CreateEmptyWholeSettingDict()
        {
            var settings = new Dictionary<EventTypeEnum, List<SingleSetting>>();

            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }
                settings.Add(type, new(0));
            }
            return settings;
        }

        /// <summary>
        /// イベント毎の設定にデフォルト値で一つ追加された設定
        /// </summary>
        /// <returns>デフォルト値一つで構成された設定</returns>
        public static Dictionary<EventTypeEnum, List<SingleSetting>> CreateDefaultWholeSettingDict()
        {
            var settings = new Dictionary<EventTypeEnum, List<SingleSetting>>();

            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }
                settings.Add(type, new() { new(oscAddress: "/avatar/parameters/" + type.ToString()) });
            }
            return settings;
        }

        private (IReadOnlyList<SingleSetting> joinedRoomUrl,
            IReadOnlyList<SingleSetting> joinedRoomName,
            IReadOnlyList<SingleSetting> acceptFriendRequest,
            IReadOnlyList<SingleSetting> playedVideo1,
            IReadOnlyList<SingleSetting> playedVideo2,
            IReadOnlyList<SingleSetting> acceptInvite,
            IReadOnlyList<SingleSetting> acceptRequestInvite,
            IReadOnlyList<SingleSetting> onPlayerJoined,
            IReadOnlyList<SingleSetting> onPlayerLeft,
            IReadOnlyList<SingleSetting> tookScreenshot,
            IReadOnlyList<SingleSetting> SuccessfullyLeftRoom,
            IReadOnlyList<SingleSetting> FinishedEnteringWorld,
            IReadOnlyList<SingleSetting> Rejoining,
            IReadOnlyList<SingleSetting> GoHome) DistributeSettings(Dictionary<EventTypeEnum, List<SingleSetting>> settings)
        {
            // イベント追加時の追加忘れ注意
            return
            (
                settings[EventTypeEnum.JoinedRoomURL].AsReadOnly(),
                settings[EventTypeEnum.JoinedRoomName].AsReadOnly(),
                settings[EventTypeEnum.AcceptFriendRequest].AsReadOnly(),
                settings[EventTypeEnum.PlayedVideo1].AsReadOnly(),
                settings[EventTypeEnum.PlayedVideo2].AsReadOnly(),
                settings[EventTypeEnum.AcceptInvite].AsReadOnly(),
                settings[EventTypeEnum.AcceptRequestInvite].AsReadOnly(),
                settings[EventTypeEnum.OnPlayerJoined].AsReadOnly(),
                settings[EventTypeEnum.OnPlayerLeft].AsReadOnly(),
                settings[EventTypeEnum.TookScreenshot].AsReadOnly(),
                settings[EventTypeEnum.SuccessfullyLeftRoom].AsReadOnly(),
                settings[EventTypeEnum.FinishedEnteringWorld].AsReadOnly(),
                settings[EventTypeEnum.Rejoining].AsReadOnly(),
                settings[EventTypeEnum.GoHome].AsReadOnly()
            );
        }

        private Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>> CompoundSettings()
        {
            var settings = new Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>>();

            // イベント追加時の追加忘れ注意
            var eachSettings = new Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>>()
            {
                {EventTypeEnum.JoinedRoomURL, JoinedRoomURL},
                {EventTypeEnum.JoinedRoomName, JoinedRoomName},
                {EventTypeEnum.AcceptFriendRequest, AcceptFriendRequest},
                {EventTypeEnum.PlayedVideo1, PlayedVideo1},
                {EventTypeEnum.PlayedVideo2, PlayedVideo2},
                {EventTypeEnum.AcceptInvite, AcceptInvite},
                {EventTypeEnum.AcceptRequestInvite, AcceptRequestInvite},
                {EventTypeEnum.OnPlayerJoined, OnPlayerJoined},
                {EventTypeEnum.OnPlayerLeft, OnPlayerLeft},
                {EventTypeEnum.TookScreenshot, TookScreenshot},
                {EventTypeEnum.SuccessfullyLeftRoom, SuccessfullyLeftRoom},
                {EventTypeEnum.FinishedEnteringWorld, FinishedEnteringWorld},
                {EventTypeEnum.Rejoining, Rejoining},
                {EventTypeEnum.GoHome, GoHome},
            };

            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }
                settings.Add(type, eachSettings[type]);
            }
            return settings;
        }
        public WholeSetting()
        {
            var settings = CreateEmptyWholeSettingDict();

            // イベント追加時の追加忘れ注意
            (
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
                GoHome
            ) = DistributeSettings(settings);

            Settings = CompoundSettings();
        }

        public WholeSetting(Dictionary<EventTypeEnum, List<SingleSetting>> settings)
        {
            // イベント追加時の追加忘れ注意
            (
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
                GoHome
            ) = DistributeSettings(settings);

            Settings = CompoundSettings();
        }

        [JsonConstructor]
        public WholeSetting(int jsonVersion,
            IReadOnlyList<SingleSetting> joinedRoomUrl,
            IReadOnlyList<SingleSetting> joinedRoomName,
            IReadOnlyList<SingleSetting> acceptFriendRequest,
            IReadOnlyList<SingleSetting> playedVideo1,
            IReadOnlyList<SingleSetting> playedVideo2,
            IReadOnlyList<SingleSetting> acceptInvite,
            IReadOnlyList<SingleSetting> acceptRequestInvite,
            IReadOnlyList<SingleSetting> onPlayerJoined,
            IReadOnlyList<SingleSetting> onPlayerLeft,
            IReadOnlyList<SingleSetting> tookScreenshot,
            IReadOnlyList<SingleSetting> successfullyLeftRoom,
            IReadOnlyList<SingleSetting> finishedEnteringWorld,
            IReadOnlyList<SingleSetting> rejoining,
            IReadOnlyList<SingleSetting> goHome)
        {
            // 設定ファイルの書式が変わった場合バージョンを見てマイグレート
            // if (jsonVersion < JsonVersion)
            // {
            //     MigrateSetting();
            // }
            // else
            // {
            //     JsonVersion = jsonVersion;
            // }

            // イベント追加時の追加忘れ注意
            JoinedRoomURL = joinedRoomUrl;
            JoinedRoomName = joinedRoomName;
            AcceptFriendRequest = acceptFriendRequest;
            PlayedVideo1 = playedVideo1;
            PlayedVideo2 = playedVideo2;
            AcceptInvite = acceptInvite;
            AcceptRequestInvite = acceptRequestInvite;
            OnPlayerJoined = onPlayerJoined;
            OnPlayerLeft = onPlayerLeft;
            TookScreenshot = tookScreenshot;
            SuccessfullyLeftRoom = successfullyLeftRoom;
            FinishedEnteringWorld = finishedEnteringWorld;
            Rejoining = rejoining;
            GoHome = goHome;

            Settings = CompoundSettings();
        }

    }
}
