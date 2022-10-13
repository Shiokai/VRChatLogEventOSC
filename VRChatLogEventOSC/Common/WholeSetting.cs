using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static VRChatLogEventOSC.Common.RegexPattern;

namespace VRChatLogEventOSC.Common
{
    public sealed record class WholeSetting
    {
        public int JsonVersion { get; init; } = 0;

        private readonly Dictionary<EventTypeEnum, List<SingleSetting>> _settings = new(){
            // {EventTypeEnum.ReceivedInvite, new(0)},
            // {EventTypeEnum.ReceivedRequestInvite, new(0)},
            // {EventTypeEnum.SendInvite, new(0)},
            // {EventTypeEnum.SendRequestInvite, new(0)},
            {EventTypeEnum.JoinedRoomURL, new(0)},
            {EventTypeEnum.JoinedRoomName, new(0)},
            // {EventTypeEnum.SendFriendRequest, new(0)},
            // {EventTypeEnum.ReceivedFriendRequest, new(0)},
            {EventTypeEnum.AcceptFriendRequest, new(0)},
            // {EventTypeEnum.ReceivedInviteResponse, new(0)},
            // {EventTypeEnum.ReceivedRequestInviteResponse, new(0)},
            {EventTypeEnum.PlayedVideo1, new(0)},
            {EventTypeEnum.PlayedVideo2, new(0)},
            {EventTypeEnum.AcceptInvite, new(0)},
            {EventTypeEnum.AcceptRequestInvite, new(0)},
            {EventTypeEnum.OnPlayerJoined, new(0)},
            {EventTypeEnum.OnPlayerLeft, new(0)},
            {EventTypeEnum.TookScreenshot, new(0)},
        };

        public IReadOnlyDictionary<EventTypeEnum, IReadOnlyList<SingleSetting>> Settings { get; }

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

        public static Dictionary<EventTypeEnum, List<SingleSetting>> CreateEmptyWholeSettingDict()
        {
            return new()
            {
                // {EventTypeEnum.ReceivedInvite, new(0)},
                // {EventTypeEnum.ReceivedRequestInvite, new(0)},
                // {EventTypeEnum.SendInvite, new(0)},
                // {EventTypeEnum.SendRequestInvite, new(0)},
                {EventTypeEnum.JoinedRoomURL, new(0)},
                {EventTypeEnum.JoinedRoomName, new(0)},
                // {EventTypeEnum.SendFriendRequest, new(0)},
                // {EventTypeEnum.ReceivedFriendRequest, new(0)},
                {EventTypeEnum.AcceptFriendRequest, new(0)},
                // {EventTypeEnum.ReceivedInviteResponse, new(0)},
                // {EventTypeEnum.ReceivedRequestInviteResponse, new(0)},
                {EventTypeEnum.PlayedVideo1, new(0)},
                {EventTypeEnum.PlayedVideo2, new(0)},
                {EventTypeEnum.AcceptInvite, new(0)},
                {EventTypeEnum.AcceptRequestInvite, new(0)},
                {EventTypeEnum.OnPlayerJoined, new(0)},
                {EventTypeEnum.OnPlayerLeft, new(0)},
                {EventTypeEnum.TookScreenshot, new(0)},
            };
        }

        public static Dictionary<EventTypeEnum, List<SingleSetting>> CreateDefaultWholeSettingDict()
        {

            return new()
            {
                // {EventTypeEnum.ReceivedInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedInvite))}},
                // {EventTypeEnum.ReceivedRequestInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedRequestInvite))}},
                // {EventTypeEnum.SendInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.SendInvite))}},
                // {EventTypeEnum.SendRequestInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.SendRequestInvite))}},
                {EventTypeEnum.JoinedRoomURL, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.JoinedRoomURL))}},
                {EventTypeEnum.JoinedRoomName, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.JoinedRoomName))}},
                // {EventTypeEnum.SendFriendRequest, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.SendFriendRequest))}},
                // {EventTypeEnum.ReceivedFriendRequest, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedFriendRequest))}},
                {EventTypeEnum.AcceptFriendRequest, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.AcceptFriendRequest))}},
                // {EventTypeEnum.ReceivedInviteResponse, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedInviteResponse))}},
                // {EventTypeEnum.ReceivedRequestInviteResponse, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedRequestInviteResponse))}},
                {EventTypeEnum.PlayedVideo1, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.PlayedVideo1))}},
                {EventTypeEnum.PlayedVideo2, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.PlayedVideo2))}},
                {EventTypeEnum.AcceptInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.AcceptInvite))}},
                {EventTypeEnum.AcceptRequestInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.AcceptRequestInvite))}},
                {EventTypeEnum.OnPlayerJoined, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.OnPlayerJoined))}},
                {EventTypeEnum.OnPlayerLeft, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.OnPlayerLeft))}},
                {EventTypeEnum.TookScreenshot, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.TookScreenshot))}},
            };
        }

        public WholeSetting()
        {
            _settings = CreateEmptyWholeSettingDict();
            Settings = new Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>>()
            {
                // {EventTypeEnum.ReceivedInvite, _settings[EventTypeEnum.ReceivedInvite].AsReadOnly()},
                // {EventTypeEnum.ReceivedRequestInvite, _settings[EventTypeEnum.ReceivedRequestInvite].AsReadOnly()},
                // {EventTypeEnum.SendInvite, _settings[EventTypeEnum.SendInvite].AsReadOnly()},
                // {EventTypeEnum.SendRequestInvite, _settings[EventTypeEnum.SendRequestInvite].AsReadOnly()},
                {EventTypeEnum.JoinedRoomURL, _settings[EventTypeEnum.JoinedRoomURL].AsReadOnly()},
                {EventTypeEnum.JoinedRoomName, _settings[EventTypeEnum.JoinedRoomName].AsReadOnly()},
                // {EventTypeEnum.SendFriendRequest, _settings[EventTypeEnum.SendFriendRequest].AsReadOnly()},
                // {EventTypeEnum.ReceivedFriendRequest, _settings[EventTypeEnum.ReceivedFriendRequest].AsReadOnly()},
                {EventTypeEnum.AcceptFriendRequest, _settings[EventTypeEnum.AcceptFriendRequest].AsReadOnly()},
                // {EventTypeEnum.ReceivedInviteResponse, _settings[EventTypeEnum.ReceivedInviteResponse].AsReadOnly()},
                // {EventTypeEnum.ReceivedRequestInviteResponse, _settings[EventTypeEnum.ReceivedRequestInviteResponse].AsReadOnly()},
                {EventTypeEnum.PlayedVideo1, _settings[EventTypeEnum.PlayedVideo1].AsReadOnly()},
                {EventTypeEnum.PlayedVideo2, _settings[EventTypeEnum.PlayedVideo2].AsReadOnly()},
                {EventTypeEnum.AcceptInvite, _settings[EventTypeEnum.AcceptInvite].AsReadOnly()},
                {EventTypeEnum.AcceptRequestInvite, _settings[EventTypeEnum.AcceptRequestInvite].AsReadOnly()},
                {EventTypeEnum.OnPlayerJoined, _settings[EventTypeEnum.OnPlayerJoined].AsReadOnly()},
                {EventTypeEnum.OnPlayerLeft, _settings[EventTypeEnum.OnPlayerLeft].AsReadOnly()},
                {EventTypeEnum.TookScreenshot, _settings[EventTypeEnum.TookScreenshot].AsReadOnly()},
            };
        }

        public WholeSetting(Dictionary<EventTypeEnum, List<SingleSetting>> settings)
        {
            _settings = settings;
            Settings = new Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>>()
            {
                // {EventTypeEnum.ReceivedInvite, _settings[EventTypeEnum.ReceivedInvite].AsReadOnly()},
                // {EventTypeEnum.ReceivedRequestInvite, _settings[EventTypeEnum.ReceivedRequestInvite].AsReadOnly()},
                // {EventTypeEnum.SendInvite, _settings[EventTypeEnum.SendInvite].AsReadOnly()},
                // {EventTypeEnum.SendRequestInvite, _settings[EventTypeEnum.SendRequestInvite].AsReadOnly()},
                {EventTypeEnum.JoinedRoomURL, _settings[EventTypeEnum.JoinedRoomURL].AsReadOnly()},
                {EventTypeEnum.JoinedRoomName, _settings[EventTypeEnum.JoinedRoomName].AsReadOnly()},
                // {EventTypeEnum.SendFriendRequest, _settings[EventTypeEnum.SendFriendRequest].AsReadOnly()},
                // {EventTypeEnum.ReceivedFriendRequest, _settings[EventTypeEnum.ReceivedFriendRequest].AsReadOnly()},
                {EventTypeEnum.AcceptFriendRequest, _settings[EventTypeEnum.AcceptFriendRequest].AsReadOnly()},
                // {EventTypeEnum.ReceivedInviteResponse, _settings[EventTypeEnum.ReceivedInviteResponse].AsReadOnly()},
                // {EventTypeEnum.ReceivedRequestInviteResponse, _settings[EventTypeEnum.ReceivedRequestInviteResponse].AsReadOnly()},
                {EventTypeEnum.PlayedVideo1, _settings[EventTypeEnum.PlayedVideo1].AsReadOnly()},
                {EventTypeEnum.PlayedVideo2, _settings[EventTypeEnum.PlayedVideo2].AsReadOnly()},
                {EventTypeEnum.AcceptInvite, _settings[EventTypeEnum.AcceptInvite].AsReadOnly()},
                {EventTypeEnum.AcceptRequestInvite, _settings[EventTypeEnum.AcceptRequestInvite].AsReadOnly()},
                {EventTypeEnum.OnPlayerJoined, _settings[EventTypeEnum.OnPlayerJoined].AsReadOnly()},
                {EventTypeEnum.OnPlayerLeft, _settings[EventTypeEnum.OnPlayerLeft].AsReadOnly()},
                {EventTypeEnum.TookScreenshot, _settings[EventTypeEnum.TookScreenshot].AsReadOnly()},
            };
        }

        [JsonConstructor]
        public WholeSetting(int jsonVersion, IReadOnlyDictionary<EventTypeEnum, IReadOnlyList<SingleSetting>> settings)
        {
            // if (jsonVersion < JsonVersion)
            // {
            //     MigrateSetting();
            // }
            // else
            // {
            //     JsonVersion = jsonVersion;
            // }

            _settings = new Dictionary<EventTypeEnum, List<SingleSetting>>()
            {
                // {EventTypeEnum.ReceivedInvite, settings.TryGetValue(EventTypeEnum.ReceivedInvite, out var receivedInvite) ? receivedInvite.ToList() : new List<SingleSetting>(0)},
                // {EventTypeEnum.ReceivedRequestInvite, settings.TryGetValue(EventTypeEnum.ReceivedRequestInvite, out var receivedRequestInvite) ? receivedRequestInvite.ToList() : new List<SingleSetting>(0)},
                // {EventTypeEnum.SendInvite, settings.TryGetValue(EventTypeEnum.SendInvite, out var sendInvite) ? sendInvite.ToList() : new List<SingleSetting>(0)},
                // {EventTypeEnum.SendRequestInvite, settings.TryGetValue(EventTypeEnum.SendRequestInvite, out var sendRequestInvite) ? sendRequestInvite.ToList() : new List<SingleSetting>(0)},
                {EventTypeEnum.JoinedRoomURL, settings.TryGetValue(EventTypeEnum.JoinedRoomURL, out var joinedRoomURL) ? joinedRoomURL.ToList() : new List<SingleSetting>(0)},
                {EventTypeEnum.JoinedRoomName, settings.TryGetValue(EventTypeEnum.JoinedRoomName, out var joinedRoomName) ? joinedRoomName.ToList() : new List<SingleSetting>(0)},
                // {EventTypeEnum.SendFriendRequest, settings.TryGetValue(EventTypeEnum.SendFriendRequest, out var sendFriendRequest) ? sendFriendRequest.ToList() : new List<SingleSetting>(0)},
                // {EventTypeEnum.ReceivedFriendRequest, settings.TryGetValue(EventTypeEnum.ReceivedFriendRequest, out var receivedFriendRequest) ? receivedFriendRequest.ToList() : new List<SingleSetting>(0)},
                {EventTypeEnum.AcceptFriendRequest, settings.TryGetValue(EventTypeEnum.AcceptFriendRequest, out var acceptFriendRequest) ? acceptFriendRequest.ToList() : new List<SingleSetting>(0)},
                // {EventTypeEnum.ReceivedInviteResponse, settings.TryGetValue(EventTypeEnum.ReceivedInviteResponse, out var receivedInviteResponse) ? receivedInviteResponse.ToList() : new List<SingleSetting>(0)},
                // {EventTypeEnum.ReceivedRequestInviteResponse, settings.TryGetValue(EventTypeEnum.ReceivedRequestInviteResponse, out var receivedRequestInviteResponse) ? receivedRequestInviteResponse.ToList() : new List<SingleSetting>(0)},
                {EventTypeEnum.PlayedVideo1, settings.TryGetValue(EventTypeEnum.PlayedVideo1, out var playedVideo1) ? playedVideo1.ToList() : new List<SingleSetting>(0)},
                {EventTypeEnum.PlayedVideo2, settings.TryGetValue(EventTypeEnum.PlayedVideo2, out var playedVideo2) ? playedVideo2.ToList() : new List<SingleSetting>(0)},
                {EventTypeEnum.AcceptInvite, settings.TryGetValue(EventTypeEnum.AcceptInvite, out var acceptInvite) ? acceptInvite.ToList() : new List<SingleSetting>(0)},
                {EventTypeEnum.AcceptRequestInvite, settings.TryGetValue(EventTypeEnum.AcceptRequestInvite, out var acceptRequestInvite) ? acceptRequestInvite.ToList() : new List<SingleSetting>(0)},
                {EventTypeEnum.OnPlayerJoined, settings.TryGetValue(EventTypeEnum.OnPlayerJoined, out var onPlayerJoined) ? onPlayerJoined.ToList() : new List<SingleSetting>(0)},
                {EventTypeEnum.OnPlayerLeft, settings.TryGetValue(EventTypeEnum.OnPlayerLeft, out var onPlayerLeft) ? onPlayerLeft.ToList() : new List<SingleSetting>(0)},
                {EventTypeEnum.TookScreenshot, settings.TryGetValue(EventTypeEnum.TookScreenshot, out var tookScreenshot) ? tookScreenshot.ToList() : new List<SingleSetting>(0)},
            };
            Settings = new Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>>()
            {
                // {EventTypeEnum.ReceivedInvite, _settings[EventTypeEnum.ReceivedInvite].AsReadOnly()},
                // {EventTypeEnum.ReceivedRequestInvite, _settings[EventTypeEnum.ReceivedRequestInvite].AsReadOnly()},
                // {EventTypeEnum.SendInvite, _settings[EventTypeEnum.SendInvite].AsReadOnly()},
                // {EventTypeEnum.SendRequestInvite, _settings[EventTypeEnum.SendRequestInvite].AsReadOnly()},
                {EventTypeEnum.JoinedRoomURL, _settings[EventTypeEnum.JoinedRoomURL].AsReadOnly()},
                {EventTypeEnum.JoinedRoomName, _settings[EventTypeEnum.JoinedRoomName].AsReadOnly()},
                // {EventTypeEnum.SendFriendRequest, _settings[EventTypeEnum.SendFriendRequest].AsReadOnly()},
                // {EventTypeEnum.ReceivedFriendRequest, _settings[EventTypeEnum.ReceivedFriendRequest].AsReadOnly()},
                {EventTypeEnum.AcceptFriendRequest, _settings[EventTypeEnum.AcceptFriendRequest].AsReadOnly()},
                // {EventTypeEnum.ReceivedInviteResponse, _settings[EventTypeEnum.ReceivedInviteResponse].AsReadOnly()},
                // {EventTypeEnum.ReceivedRequestInviteResponse, _settings[EventTypeEnum.ReceivedRequestInviteResponse].AsReadOnly()},
                {EventTypeEnum.PlayedVideo1, _settings[EventTypeEnum.PlayedVideo1].AsReadOnly()},
                {EventTypeEnum.PlayedVideo2, _settings[EventTypeEnum.PlayedVideo2].AsReadOnly()},
                {EventTypeEnum.AcceptInvite, _settings[EventTypeEnum.AcceptInvite].AsReadOnly()},
                {EventTypeEnum.AcceptRequestInvite, _settings[EventTypeEnum.AcceptRequestInvite].AsReadOnly()},
                {EventTypeEnum.OnPlayerJoined, _settings[EventTypeEnum.OnPlayerJoined].AsReadOnly()},
                {EventTypeEnum.OnPlayerLeft, _settings[EventTypeEnum.OnPlayerLeft].AsReadOnly()},
                {EventTypeEnum.TookScreenshot, _settings[EventTypeEnum.TookScreenshot].AsReadOnly()},
            };
        }

    }
}
