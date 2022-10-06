using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static VRChatLogEventOSC.RegexPattern;

namespace VRChatLogEventOSC
{
    public class WholeSetting
    {
        public string Version { get; } = "0.0.0";

        private readonly Dictionary<EventTypeEnum, List<SingleSetting>> _settings = new(){
            {EventTypeEnum.ReceivedInvite, new(0)},
            {EventTypeEnum.ReceivedRequestInvite, new(0)},
            {EventTypeEnum.SendInvite, new(0)},
            {EventTypeEnum.SendRequestInvite, new(0)},
            {EventTypeEnum.JoinedRoomURL, new(0)},
            {EventTypeEnum.JoinedRoomName, new(0)},
            {EventTypeEnum.SendFriendRequest, new(0)},
            {EventTypeEnum.ReceivedFriendRequest, new(0)},
            {EventTypeEnum.AcceptFriendRequest, new(0)},
            {EventTypeEnum.ReceivedInviteResponse, new(0)},
            {EventTypeEnum.ReceivedRequestInviteResponse, new(0)},
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
                {EventTypeEnum.ReceivedInvite, new(0)},
                {EventTypeEnum.ReceivedRequestInvite, new(0)},
                {EventTypeEnum.SendInvite, new(0)},
                {EventTypeEnum.SendRequestInvite, new(0)},
                {EventTypeEnum.JoinedRoomURL, new(0)},
                {EventTypeEnum.JoinedRoomName, new(0)},
                {EventTypeEnum.SendFriendRequest, new(0)},
                {EventTypeEnum.ReceivedFriendRequest, new(0)},
                {EventTypeEnum.AcceptFriendRequest, new(0)},
                {EventTypeEnum.ReceivedInviteResponse, new(0)},
                {EventTypeEnum.ReceivedRequestInviteResponse, new(0)},
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
                {EventTypeEnum.ReceivedInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedInvite))}},
                {EventTypeEnum.ReceivedRequestInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedRequestInvite))}},
                {EventTypeEnum.SendInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.SendInvite))}},
                {EventTypeEnum.SendRequestInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.SendRequestInvite))}},
                {EventTypeEnum.JoinedRoomURL, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.JoinedRoomURL))}},
                {EventTypeEnum.JoinedRoomName, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.JoinedRoomName))}},
                {EventTypeEnum.SendFriendRequest, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.SendFriendRequest))}},
                {EventTypeEnum.ReceivedFriendRequest, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedFriendRequest))}},
                {EventTypeEnum.AcceptFriendRequest, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.AcceptFriendRequest))}},
                {EventTypeEnum.ReceivedInviteResponse, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedInviteResponse))}},
                {EventTypeEnum.ReceivedRequestInviteResponse, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedRequestInviteResponse))}},
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
            _settings = CreateDefaultWholeSettingDict();
            Settings = new Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>>()
            {
                {EventTypeEnum.ReceivedInvite, _settings[EventTypeEnum.ReceivedInvite].AsReadOnly()},
                {EventTypeEnum.ReceivedRequestInvite, _settings[EventTypeEnum.ReceivedRequestInvite].AsReadOnly()},
                {EventTypeEnum.SendInvite, _settings[EventTypeEnum.SendInvite].AsReadOnly()},
                {EventTypeEnum.SendRequestInvite, _settings[EventTypeEnum.SendRequestInvite].AsReadOnly()},
                {EventTypeEnum.JoinedRoomURL, _settings[EventTypeEnum.JoinedRoomURL].AsReadOnly()},
                {EventTypeEnum.JoinedRoomName, _settings[EventTypeEnum.JoinedRoomName].AsReadOnly()},
                {EventTypeEnum.SendFriendRequest, _settings[EventTypeEnum.SendFriendRequest].AsReadOnly()},
                {EventTypeEnum.ReceivedFriendRequest, _settings[EventTypeEnum.ReceivedFriendRequest].AsReadOnly()},
                {EventTypeEnum.AcceptFriendRequest, _settings[EventTypeEnum.AcceptFriendRequest].AsReadOnly()},
                {EventTypeEnum.ReceivedInviteResponse, _settings[EventTypeEnum.ReceivedInviteResponse].AsReadOnly()},
                {EventTypeEnum.ReceivedRequestInviteResponse, _settings[EventTypeEnum.ReceivedRequestInviteResponse].AsReadOnly()},
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
                {EventTypeEnum.ReceivedInvite, _settings[EventTypeEnum.ReceivedInvite].AsReadOnly()},
                {EventTypeEnum.ReceivedRequestInvite, _settings[EventTypeEnum.ReceivedRequestInvite].AsReadOnly()},
                {EventTypeEnum.SendInvite, _settings[EventTypeEnum.SendInvite].AsReadOnly()},
                {EventTypeEnum.SendRequestInvite, _settings[EventTypeEnum.SendRequestInvite].AsReadOnly()},
                {EventTypeEnum.JoinedRoomURL, _settings[EventTypeEnum.JoinedRoomURL].AsReadOnly()},
                {EventTypeEnum.JoinedRoomName, _settings[EventTypeEnum.JoinedRoomName].AsReadOnly()},
                {EventTypeEnum.SendFriendRequest, _settings[EventTypeEnum.SendFriendRequest].AsReadOnly()},
                {EventTypeEnum.ReceivedFriendRequest, _settings[EventTypeEnum.ReceivedFriendRequest].AsReadOnly()},
                {EventTypeEnum.AcceptFriendRequest, _settings[EventTypeEnum.AcceptFriendRequest].AsReadOnly()},
                {EventTypeEnum.ReceivedInviteResponse, _settings[EventTypeEnum.ReceivedInviteResponse].AsReadOnly()},
                {EventTypeEnum.ReceivedRequestInviteResponse, _settings[EventTypeEnum.ReceivedRequestInviteResponse].AsReadOnly()},
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
        public WholeSetting(string version, IReadOnlyDictionary<EventTypeEnum, IReadOnlyList<SingleSetting>> settings)
        {
            Version = version;
            _settings = new Dictionary<EventTypeEnum, List<SingleSetting>>()
            {
                {EventTypeEnum.ReceivedInvite, settings[EventTypeEnum.ReceivedInvite].ToList()},
                {EventTypeEnum.ReceivedRequestInvite, settings[EventTypeEnum.ReceivedRequestInvite].ToList()},
                {EventTypeEnum.SendInvite, settings[EventTypeEnum.SendInvite].ToList()},
                {EventTypeEnum.SendRequestInvite, settings[EventTypeEnum.SendRequestInvite].ToList()},
                {EventTypeEnum.JoinedRoomURL, settings[EventTypeEnum.JoinedRoomURL].ToList()},
                {EventTypeEnum.JoinedRoomName, settings[EventTypeEnum.JoinedRoomName].ToList()},
                {EventTypeEnum.SendFriendRequest, settings[EventTypeEnum.SendFriendRequest].ToList()},
                {EventTypeEnum.ReceivedFriendRequest, settings[EventTypeEnum.ReceivedFriendRequest].ToList()},
                {EventTypeEnum.AcceptFriendRequest, settings[EventTypeEnum.AcceptFriendRequest].ToList()},
                {EventTypeEnum.ReceivedInviteResponse, settings[EventTypeEnum.ReceivedInviteResponse].ToList()},
                {EventTypeEnum.ReceivedRequestInviteResponse, settings[EventTypeEnum.ReceivedRequestInviteResponse].ToList()},
                {EventTypeEnum.PlayedVideo1, settings[EventTypeEnum.PlayedVideo1].ToList()},
                {EventTypeEnum.PlayedVideo2, settings[EventTypeEnum.PlayedVideo2].ToList()},
                {EventTypeEnum.AcceptInvite, settings[EventTypeEnum.AcceptInvite].ToList()},
                {EventTypeEnum.AcceptRequestInvite, settings[EventTypeEnum.AcceptRequestInvite].ToList()},
                {EventTypeEnum.OnPlayerJoined, settings[EventTypeEnum.OnPlayerJoined].ToList()},
                {EventTypeEnum.OnPlayerLeft, settings[EventTypeEnum.OnPlayerLeft].ToList()},
                {EventTypeEnum.TookScreenshot, settings[EventTypeEnum.TookScreenshot].ToList()},
            };
            Settings = new Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>>()
            {
                {EventTypeEnum.ReceivedInvite, _settings[EventTypeEnum.ReceivedInvite].AsReadOnly()},
                {EventTypeEnum.ReceivedRequestInvite, _settings[EventTypeEnum.ReceivedRequestInvite].AsReadOnly()},
                {EventTypeEnum.SendInvite, _settings[EventTypeEnum.SendInvite].AsReadOnly()},
                {EventTypeEnum.SendRequestInvite, _settings[EventTypeEnum.SendRequestInvite].AsReadOnly()},
                {EventTypeEnum.SendFriendRequest, _settings[EventTypeEnum.SendFriendRequest].AsReadOnly()},
                {EventTypeEnum.ReceivedFriendRequest, _settings[EventTypeEnum.ReceivedFriendRequest].AsReadOnly()},
                {EventTypeEnum.AcceptFriendRequest, _settings[EventTypeEnum.AcceptFriendRequest].AsReadOnly()},
                {EventTypeEnum.ReceivedInviteResponse, _settings[EventTypeEnum.ReceivedInviteResponse].AsReadOnly()},
                {EventTypeEnum.ReceivedRequestInviteResponse, _settings[EventTypeEnum.ReceivedRequestInviteResponse].AsReadOnly()},
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
