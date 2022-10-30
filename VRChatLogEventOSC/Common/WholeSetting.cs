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
        public IReadOnlyDictionary<EventTypeEnum, IReadOnlyList<SingleSetting>> Settings { get; }
        // Dictionaryで読み書きするとKeyに無いイベントが設定されていた時にエラーになるので、読み書き用に個別プロパティを用意
        // イベント追加時の追加忘れ注意
        [JsonInclude]
        public IReadOnlyList<SingleSetting> JoinedRoomURL { private get; init;}
        [JsonInclude]
        public IReadOnlyList<SingleSetting> JoinedRoomName { private get; init;}
        [JsonInclude]
        public IReadOnlyList<SingleSetting> AcceptFriendRequest { private get; init;}
        [JsonInclude]
        public IReadOnlyList<SingleSetting> PlayedVideo1 { private get; init;}
        [JsonInclude]
        public IReadOnlyList<SingleSetting> PlayedVideo2 { private get; init;}
        [JsonInclude]
        public IReadOnlyList<SingleSetting> AcceptInvite { private get; init;}
        [JsonInclude]
        public IReadOnlyList<SingleSetting> AcceptRequestInvite { private get; init;}
        [JsonInclude]
        public IReadOnlyList<SingleSetting> OnPlayerJoined { private get; init;}
        [JsonInclude]
        public IReadOnlyList<SingleSetting> OnPlayerLeft { private get; init;}
        [JsonInclude]
        public IReadOnlyList<SingleSetting> TookScreenshot { private get; init;}

        public override string ToString()
        {
            StringBuilder result = new();
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

        public WholeSetting()
        {
            var settings = CreateEmptyWholeSettingDict();
            var settingsBase = new Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>>();

            // イベント追加時の追加忘れ注意
            JoinedRoomURL = settings[EventTypeEnum.JoinedRoomURL];
            JoinedRoomName = settings[EventTypeEnum.JoinedRoomName];
            AcceptFriendRequest = settings[EventTypeEnum.AcceptFriendRequest];
            PlayedVideo1 = settings[EventTypeEnum.PlayedVideo1];
            PlayedVideo2 = settings[EventTypeEnum.PlayedVideo2];
            AcceptInvite = settings[EventTypeEnum.AcceptInvite];
            AcceptRequestInvite = settings[EventTypeEnum.AcceptRequestInvite];
            OnPlayerJoined = settings[EventTypeEnum.OnPlayerJoined];
            OnPlayerLeft = settings[EventTypeEnum.OnPlayerLeft];
            TookScreenshot = settings[EventTypeEnum.TookScreenshot];
            
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
            };

            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }
                settingsBase.Add(type, eachSettings[type]);
            }
            Settings = settingsBase;
        }

        public WholeSetting(Dictionary<EventTypeEnum, List<SingleSetting>> settings)
        {
            var settingsBase = new Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>>();

            // イベント追加時の追加忘れ注意
            JoinedRoomURL = settings[EventTypeEnum.JoinedRoomURL].AsReadOnly();
            JoinedRoomName = settings[EventTypeEnum.JoinedRoomName].AsReadOnly();
            AcceptFriendRequest = settings[EventTypeEnum.AcceptFriendRequest].AsReadOnly();
            PlayedVideo1 = settings[EventTypeEnum.PlayedVideo1].AsReadOnly();
            PlayedVideo2 = settings[EventTypeEnum.PlayedVideo2].AsReadOnly();
            AcceptInvite = settings[EventTypeEnum.AcceptInvite].AsReadOnly();
            AcceptRequestInvite = settings[EventTypeEnum.AcceptRequestInvite].AsReadOnly();
            OnPlayerJoined = settings[EventTypeEnum.OnPlayerJoined].AsReadOnly();
            OnPlayerLeft = settings[EventTypeEnum.OnPlayerLeft].AsReadOnly();
            TookScreenshot = settings[EventTypeEnum.TookScreenshot].AsReadOnly();

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
            };

            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }
                settingsBase.Add(type, eachSettings[type]);
            }
            Settings = settingsBase;
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
            IReadOnlyList<SingleSetting> tookScreenshot)
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
            };

            var settingsBase = new Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>>();
            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }
                settingsBase.Add(type, eachSettings[type]);
            }

            Settings = settingsBase;
        }

    }
}
