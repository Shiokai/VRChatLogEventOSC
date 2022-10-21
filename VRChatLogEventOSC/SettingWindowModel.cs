using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using System.Reactive.Disposables;
using Reactive.Bindings.Extensions;
using System.ComponentModel;

using System.Diagnostics;

using VRChatLogEventOSC.Common;

namespace VRChatLogEventOSC
{
    internal class SettingWindowModel : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
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
        private RegexPattern.EventTypeEnum _shownEventType;

        private void UpdateSetting()
        {
            foreach (var type in Enum.GetValues<RegexPattern.EventTypeEnum>())
            {
                if (!_settingsCache.ContainsKey(type))
                {
                    continue;
                }
                _settingsCache[type].Clear();
                var settings = _logEventModel.GetCurrentSettingsOfType(type);

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
                _shownEventType = RegexPattern.EventTypeEnum.None;
                return;
            }

            _shownEventType = type;
            
            foreach (var setting in _settingsCache[type])
            {
                _shownSetting.Add(setting);
            }
        }

        public void SwapItem(int selected, int target)
        {
            if (selected < 0 || target < 0 || target > _shownSetting.Count - 1)
            {
                return;
            }

            (_shownSetting[selected], _shownSetting[target]) = (_shownSetting[target], _shownSetting[selected]);
        }

        public void ApplySetting()
        {
            Dictionary<RegexPattern.EventTypeEnum, List<SingleSetting>> settings = new()
            {
                // {RegexPattern.EventTypeEnum.ReceivedInvite, _settingsCache[RegexPattern.EventTypeEnum.ReceivedInvite].ToList()},
                // {RegexPattern.EventTypeEnum.ReceivedRequestInvite, _settingsCache[RegexPattern.EventTypeEnum.ReceivedRequestInvite].ToList()},
                // {RegexPattern.EventTypeEnum.SendInvite, _settingsCache[RegexPattern.EventTypeEnum.SendInvite].ToList()},
                // {RegexPattern.EventTypeEnum.SendRequestInvite, _settingsCache[RegexPattern.EventTypeEnum.SendRequestInvite].ToList()},
                {RegexPattern.EventTypeEnum.JoinedRoomURL, _settingsCache[RegexPattern.EventTypeEnum.JoinedRoomURL].ToList()},
                {RegexPattern.EventTypeEnum.JoinedRoomName, _settingsCache[RegexPattern.EventTypeEnum.JoinedRoomName].ToList()},
                // {RegexPattern.EventTypeEnum.SendFriendRequest, _settingsCache[RegexPattern.EventTypeEnum.SendFriendRequest].ToList()},
                // {RegexPattern.EventTypeEnum.ReceivedFriendRequest, _settingsCache[RegexPattern.EventTypeEnum.ReceivedFriendRequest].ToList()},
                {RegexPattern.EventTypeEnum.AcceptFriendRequest, _settingsCache[RegexPattern.EventTypeEnum.AcceptFriendRequest].ToList()},
                // {RegexPattern.EventTypeEnum.ReceivedInviteResponse, _settingsCache[RegexPattern.EventTypeEnum.ReceivedInviteResponse].ToList()},
                // {RegexPattern.EventTypeEnum.ReceivedRequestInviteResponse, _settingsCache[RegexPattern.EventTypeEnum.ReceivedRequestInviteResponse].ToList()},
                {RegexPattern.EventTypeEnum.PlayedVideo1, _settingsCache[RegexPattern.EventTypeEnum.PlayedVideo1].ToList()},
                {RegexPattern.EventTypeEnum.PlayedVideo2, _settingsCache[RegexPattern.EventTypeEnum.PlayedVideo2].ToList()},
                {RegexPattern.EventTypeEnum.AcceptInvite, _settingsCache[RegexPattern.EventTypeEnum.AcceptInvite].ToList()},
                {RegexPattern.EventTypeEnum.AcceptRequestInvite, _settingsCache[RegexPattern.EventTypeEnum.AcceptRequestInvite].ToList()},
                {RegexPattern.EventTypeEnum.OnPlayerJoined, _settingsCache[RegexPattern.EventTypeEnum.OnPlayerJoined].ToList()},
                {RegexPattern.EventTypeEnum.OnPlayerLeft, _settingsCache[RegexPattern.EventTypeEnum.OnPlayerLeft].ToList()},
                {RegexPattern.EventTypeEnum.TookScreenshot, _settingsCache[RegexPattern.EventTypeEnum.TookScreenshot].ToList()},
            };
            settings[_shownEventType] = _shownSetting.ToList();
            FileLoader.SaveSetting(new WholeSetting(settings));
            FileLoader.LoadSetting();
            UpdateSetting();
        }
        private CompositeDisposable _compositeDisposables = new();

        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _compositeDisposables.Dispose();

        }
        private SettingWindowModel()
        {
            _logEventModel = LogEventModel.Instance;
            UpdateSetting();
            ShownSetting = _shownSetting.ToReadOnlyReactiveCollection();
            _shownSetting.AddTo(_compositeDisposables);
            ShownSetting.AddTo(_compositeDisposables);

            foreach (var cache in _settingsCache.Values)
            {
                cache.AddTo(_compositeDisposables);
            }
        }
    }
}
