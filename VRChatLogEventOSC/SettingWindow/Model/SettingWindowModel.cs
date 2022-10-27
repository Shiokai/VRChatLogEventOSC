using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using System.Reactive.Disposables;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using System.Windows;

using VRChatLogEventOSC.Core;
using VRChatLogEventOSC.Editor;

using VRChatLogEventOSC.Common;

namespace VRChatLogEventOSC.Setting
{
    internal class SettingWindowModel : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private static SettingWindowModel? _instance;
        public static SettingWindowModel Instance => _instance ??= new SettingWindowModel();
        private LogEventCore _core;

        private Dictionary<RegexPattern.EventTypeEnum, ReactiveCollection<SingleSetting>> _settingsCache;

        private ReactiveCollection<SingleSetting> _shownSetting = new();

        public ReadOnlyReactiveCollection<SingleSetting> ShownSetting { get; set; }
        private RegexPattern.EventTypeEnum _shownEventType = RegexPattern.EventTypeEnum.None;
        public RegexPattern.EventTypeEnum ShownEventType => _shownEventType;
        public SingleSetting? SelectedSetting { get; set; }
        public int SelectedIndex { get; set; }

        private bool _isShownDirty = false;
        public bool IsDirty { get; private set; } = false;

        /// <summary>
        /// 設定のキャッシュを現在読み込まれている設定に更新します
        /// </summary>
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

        /// <summary>
        /// 表示中の設定が変更されていた場合、設定のキャッシュに書き戻します
        /// </summary>
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

        /// <summary>
        /// 設定のキャッシュから表示中の設定を更新します
        /// </summary>
        /// <param name="type"></param>
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

        /// <summary>
        /// 表示する設定を変更します
        /// </summary>
        /// <param name="type"></param>
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

        /// <summary>
        /// 選択中の設定を上に移動します
        /// </summary>
        public void UpSelectedItem()
        {
            SwapItem(SelectedIndex, SelectedIndex - 1);
        }

        /// <summary>
        /// 選択中の設定を下に移動します
        /// </summary>
        public void DownSelectedItem()
        {
            SwapItem(SelectedIndex, SelectedIndex + 1);
        }

        /// <summary>
        /// 指定したIndexの設定を入れ替えます
        /// </summary>
        /// <param name="selected">入れ替える設定</param>
        /// <param name="target">入れ替える設定</param>
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

        /// <summary>
        /// 設定を保存、読み込みし、キャッシュと表示を更新します
        /// </summary>
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

            try
            {
                FileLoader.SaveSetting(new WholeSetting(settings));
            }
            catch (System.IO.IOException e)
            {
                MessageBox.Show($"設定ファイルの書き込みに失敗しました\n{e.Message}", "IOException", MessageBoxButton.OK);
                return;
            }
            catch(UnauthorizedAccessException e)
            {
                MessageBox.Show($"設定ファイルへのアクセスが拒否されました\n{e.Message}", "UnauthorizedAccessException", MessageBoxButton.OK);
                return;
            }

            try
            {
                _core.LoadCurrentSetting();
            }
            catch (System.IO.IOException e)
            {
                MessageBox.Show($"設定ファイルの書き込みに失敗しました\n{e.Message}", "IOException", MessageBoxButton.OK);
                return;
            }
            catch(UnauthorizedAccessException e)
            {
                MessageBox.Show($"設定ファイルへのアクセスが拒否されました\n{e.Message}", "UnauthorizedAccessException", MessageBoxButton.OK);
                return;
            }
            
            UpdateSetting();
            LoadShownFromCache(_shownEventType);
            IsDirty = false;
        }

        /// <summary>
        /// EditorWindowを設定の追加として表示します
        /// </summary>
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

        /// <summary>
        /// EditorWindowを、選択している設定の編集として開きます
        /// </summary>
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

        /// <summary>
        /// 現在表示中の設定の末尾に設定を追加します
        /// </summary>
        /// <param name="setting">追加する設定</param>
        public void AddSetting(SingleSetting setting)
        {
            _shownSetting.Add(setting);
            _isShownDirty = true;
            IsDirty = true;
        }

        /// <summary>
        /// 選択中の設定を上書きします
        /// </summary>
        /// <param name="setting">上書きする設定</param>
        public void EditOverrideSetting(SingleSetting setting)
        {
            _shownSetting[SelectedIndex] = setting;
            _isShownDirty = true;
            IsDirty = true;
        }

        /// <summary>
        /// 選択中の設定を削除します
        /// </summary>
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
            _core = LogEventCore.Instance;

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
