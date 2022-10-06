﻿using System;
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
        // public string Version { get; } = "0.0.0";

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
                {EventTypeEnum.ReceivedInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedInvite), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.ReceivedRequestInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedRequestInvite), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.SendInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.SendInvite), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.SendRequestInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.SendRequestInvite), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.JoinedRoomURL, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.JoinedRoomURL), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.JoinedRoomName, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.JoinedRoomName), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.SendFriendRequest, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.SendFriendRequest), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.ReceivedFriendRequest, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedFriendRequest), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.AcceptFriendRequest, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.AcceptFriendRequest), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.ReceivedInviteResponse, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedInviteResponse), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.ReceivedRequestInviteResponse, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.ReceivedRequestInviteResponse), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.PlayedVideo1, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.PlayedVideo1), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.PlayedVideo2, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.PlayedVideo2), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.AcceptInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.AcceptInvite), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.AcceptRequestInvite, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.AcceptRequestInvite), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.OnPlayerJoined, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.OnPlayerJoined), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.OnPlayerLeft, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.OnPlayerLeft), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
                {EventTypeEnum.TookScreenshot, new(){new(oscAddress: "/avatar/parameters/" + nameof(EventTypeEnum.TookScreenshot), reqInv:SingleSetting.ReqInvEnum.NotSpecified)}},
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
        // [JsonConstructor]

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

        // [JsonConstructor]
        // public WholeSetting(string version, Dictionary<EventTypeEnum, List<SingleSetting>> settings)
        // {
        //     Version = version;
        //     _settings = settings;
        //     Settings = new Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>>()
        //     {
        //         {EventTypeEnum.ReceivedInvite, _settings[EventTypeEnum.ReceivedInvite].AsReadOnly()},
        //         {EventTypeEnum.ReceivedRequestInvite, _settings[EventTypeEnum.ReceivedRequestInvite].AsReadOnly()},
        //         {EventTypeEnum.SendInvite, _settings[EventTypeEnum.SendInvite].AsReadOnly()},
        //         {EventTypeEnum.SendRequestInvite, _settings[EventTypeEnum.SendRequestInvite].AsReadOnly()},
        //         {EventTypeEnum.MetPlayer, _settings[EventTypeEnum.MetPlayer].AsReadOnly()},
        //         {EventTypeEnum.JoinedRoom1, _settings[EventTypeEnum.JoinedRoom1].AsReadOnly()},
        //         {EventTypeEnum.JoinedRoom2, _settings[EventTypeEnum.JoinedRoom2].AsReadOnly()},
        //         {EventTypeEnum.SendFriendRequest, _settings[EventTypeEnum.SendFriendRequest].AsReadOnly()},
        //         {EventTypeEnum.ReceivedFriendRequest, _settings[EventTypeEnum.ReceivedFriendRequest].AsReadOnly()},
        //         {EventTypeEnum.AcceptFriendRequest, _settings[EventTypeEnum.AcceptFriendRequest].AsReadOnly()},
        //         {EventTypeEnum.ReceivedInviteResponse, _settings[EventTypeEnum.ReceivedInviteResponse].AsReadOnly()},
        //         {EventTypeEnum.ReceivedRequestInviteResponse, _settings[EventTypeEnum.ReceivedRequestInviteResponse].AsReadOnly()},
        //         {EventTypeEnum.PlayedVideo1, _settings[EventTypeEnum.PlayedVideo1].AsReadOnly()},
        //         {EventTypeEnum.PlayedVideo2, _settings[EventTypeEnum.PlayedVideo2].AsReadOnly()},
        //         {EventTypeEnum.AcceptInvite, _settings[EventTypeEnum.AcceptInvite].AsReadOnly()},
        //         {EventTypeEnum.AcceptRequestInvite, _settings[EventTypeEnum.AcceptRequestInvite].AsReadOnly()},
        //         {EventTypeEnum.OnPlayerJoined, _settings[EventTypeEnum.OnPlayerJoined].AsReadOnly()},
        //         {EventTypeEnum.OnPlayerLeft, _settings[EventTypeEnum.OnPlayerLeft].AsReadOnly()},
        //         {EventTypeEnum.JoinedRoom1Detail, _settings[EventTypeEnum.JoinedRoom1Detail].AsReadOnly()},
        //         {EventTypeEnum.AcceptInviteDetail, _settings[EventTypeEnum.AcceptInviteDetail].AsReadOnly()},
        //         {EventTypeEnum.NotificationEvent, _settings[EventTypeEnum.NotificationEvent].AsReadOnly()},
        //         {EventTypeEnum.TookScreenshot, _settings[EventTypeEnum.TookScreenshot].AsReadOnly()},
        //     };
        // }

    }
}
