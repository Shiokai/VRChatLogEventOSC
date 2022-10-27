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
        private Core _core;

        private Dictionary<RegexPattern.EventTypeEnum, ReactiveCollection<SingleSetting>> _settingsCache;

        private ReactiveCollection<SingleSetting> _shownSetting = new();

        public ReadOnlyReactiveCollection<SingleSetting> ShownSetting { get; set; }
        private RegexPattern.EventTypeEnum _shownEventType = RegexPattern.EventTypeEnum.None;
        public RegexPattern.EventTypeEnum ShownEventType => _shownEventType;
        public SingleSetting? SelectedSetting { get; set; }
        public int SelectedIndex { get; set; }

        private bool _isShownDirty = false;
        public bool IsDirty { get; private set; } = false;

        private void UpdateSetting()
        {
            foreach (var type in Enum.GetValues<RegexPattern.EventTypeEnum>())
            {
                if (!_settingsCache.ContainsKey(type))
                {
                    continue;
                }
                _settingsCache[type].Clear();
                var settings = _core.GetCurrentSettingsOfType(type);

                foreach (var setting in settings ?? Enumerable.Empty<SingleSetting>())
                {
                    _settingsCache[type].Add(setting);
                }
            }
        }

        private void ApplyShownDirty()
        {
            if (_isShownDirty)
            {
                var showedSetting = _settingsCache[_shownEventType];
                showedSetting.Clear();
                foreach (var setting in _shownSetting)
                {
                    showedSetting.Add(setting);
                }
                _isShownDirty = false;
            }
        }

        private void LoadShownFromCache(RegexPattern.EventTypeEnum type)
        {
            if (!_settingsCache.ContainsKey(type))
            {
                return;
            }

            _shownSetting.Clear();
            foreach (var setting in _settingsCache[type])
            {
                _shownSetting.Add(setting);
            }
        }

        public void ChangeShownSetting(RegexPattern.EventTypeEnum type)
        {
            ApplyShownDirty();

            if (!_settingsCache.ContainsKey(type))
            {
                _shownEventType = RegexPattern.EventTypeEnum.None;
                return;
            }

            _shownEventType = type;
            LoadShownFromCache(type);
            _isShownDirty = false;
        }

        public void UpSelectedItem()
        {
            SwapItem(SelectedIndex, SelectedIndex - 1);
        }

        public void DownSelectedItem()
        {
            SwapItem(SelectedIndex, SelectedIndex + 1);
        }

        private void SwapItem(int selected, int target)
        {
            if (selected < 0 || target < 0 || target > _shownSetting.Count - 1)
            {
                return;
            }

            (_shownSetting[selected], _shownSetting[target]) = (_shownSetting[target], _shownSetting[selected]);
            _isShownDirty = true;
            IsDirty = true;
        }

        public void ApplySetting()
        {
            ApplyShownDirty();

            Dictionary<RegexPattern.EventTypeEnum, List<SingleSetting>> settings = new();
            foreach (var type in Enum.GetValues<RegexPattern.EventTypeEnum>())
            {
                if (type == RegexPattern.EventTypeEnum.None)
                {
                    continue;
                }
                settings.Add(type, _settingsCache[type].ToList());
            }
            FileLoader.SaveSetting(new WholeSetting(settings));
            _core.LoadCurrentSetting();
            UpdateSetting();
            LoadShownFromCache(_shownEventType);
            IsDirty = false;
        }

        public void OpenEditorAsAdd()
        {
            if (_shownEventType == RegexPattern.EventTypeEnum.None)
            {
                return;
            }

            SelectedSetting = null;

            var editor = new EditorWindow();
            editor.ShowDialog();
        }

        public void OpenEditorAsEdit()
        {
            if (_shownEventType == RegexPattern.EventTypeEnum.None)
            {
                return;
            }

            if (SelectedSetting == null)
            {
                return;
            }

            var editor = new EditorWindow();
            editor.ShowDialog();
        }

        public void AddSetting(SingleSetting setting)
        {
            _shownSetting.Add(setting);
            _isShownDirty = true;
            IsDirty = true;
        }

        public void EditOverrideSetting(SingleSetting setting)
        {
            _shownSetting[SelectedIndex] = setting;
            _isShownDirty = true;
            IsDirty = true;
        }

        public void DeleteSetting()
        {
            _shownSetting.RemoveAt(SelectedIndex);
            _isShownDirty = true;
            IsDirty = true;
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
            _core = Core.Instance;

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
