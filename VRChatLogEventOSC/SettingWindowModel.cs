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

        private Dictionary<RegexPattern.EventTypeEnum, ReactiveCollection<SingleSetting>> _settingsCache;

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
            Dictionary<RegexPattern.EventTypeEnum, List<SingleSetting>> settings = new();
            foreach (var type in Enum.GetValues<RegexPattern.EventTypeEnum>())
            {
                settings.Add(type, _settingsCache[type].ToList());
            }
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

            _settingsCache = new Dictionary<RegexPattern.EventTypeEnum, ReactiveCollection<SingleSetting>>();
            foreach (var type in Enum.GetValues<RegexPattern.EventTypeEnum>())
            {
                if (type == RegexPattern.EventTypeEnum.None)
                {
                    continue;
                }
                _settingsCache.Add(type, new ReactiveCollection<SingleSetting>().AddTo(_compositeDisposables));
            }

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
