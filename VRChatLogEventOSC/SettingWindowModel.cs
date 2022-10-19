using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;

using System.Diagnostics;

using VRChatLogEventOSC.Common;

namespace VRChatLogEventOSC
{
    internal class SettingWindowModel
    {
        private static SettingWindowModel? _instance;
        public static SettingWindowModel Instance => _instance ??= new SettingWindowModel();
        private LogEventModel _logEventModel;

        private Dictionary<RegexPattern.EventTypeEnum, ReactiveCollection<SingleSetting>> _settingsCache = new()
        {
            // {RegexPattern.EventTypeEnum.ReceivedInvite, new()},
            // {RegexPattern.EventTypeEnum.ReceivedRequestInvite, new()},
            // {RegexPattern.EventTypeEnum.SendInvite, new()},
            // {RegexPattern.EventTypeEnum.SendRequestInvite, new()},
            {RegexPattern.EventTypeEnum.JoinedRoomURL, new()},
            {RegexPattern.EventTypeEnum.JoinedRoomName, new()},
            // {RegexPattern.EventTypeEnum.SendFriendRequest, new()},
            // {RegexPattern.EventTypeEnum.ReceivedFriendRequest, new()},
            {RegexPattern.EventTypeEnum.AcceptFriendRequest, new()},
            // {RegexPattern.EventTypeEnum.ReceivedInviteResponse, new()},
            // {RegexPattern.EventTypeEnum.ReceivedRequestInviteResponse, new()},
            {RegexPattern.EventTypeEnum.PlayedVideo1, new()},
            {RegexPattern.EventTypeEnum.PlayedVideo2, new()},
            {RegexPattern.EventTypeEnum.AcceptInvite, new()},
            {RegexPattern.EventTypeEnum.AcceptRequestInvite, new()},
            {RegexPattern.EventTypeEnum.OnPlayerJoined, new()},
            {RegexPattern.EventTypeEnum.OnPlayerLeft, new()},
            {RegexPattern.EventTypeEnum.TookScreenshot, new()},
        };

        private ReactiveCollection<SingleSetting> _shownSetting = new();

        public ReadOnlyReactiveCollection<SingleSetting> ShownSetting { get; set; }

        private void UpdateSetting()
        {
            foreach (var type in Enum.GetValues<RegexPattern.EventTypeEnum>())
            {
                if (!_settingsCache.ContainsKey(type))
                {
                    continue;
                }
                
                var settings = _logEventModel.GetTypeSettings(type);

                foreach (var setting in settings ?? Enumerable.Empty<SingleSetting>())
                {
                    _settingsCache[type].Add(setting);
                }
            }
        }

        public void ChangeShownSetting(RegexPattern.EventTypeEnum type)
        {
            _shownSetting.Clear();
            if (!_settingsCache.ContainsKey(type))
            {
                return;
            }
            foreach (var setting in _settingsCache[type])
            {
                _shownSetting.Add(setting);
            }
        }

        private SettingWindowModel()
        {
            _logEventModel = LogEventModel.Instance;
            UpdateSetting();
            ShownSetting = _shownSetting.ToReadOnlyReactiveCollection();
        }
    }
}
