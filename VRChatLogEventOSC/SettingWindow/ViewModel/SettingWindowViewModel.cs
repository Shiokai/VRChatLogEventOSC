using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Windows.Controls;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using VRChatLogEventOSC.Common;

using System.Diagnostics;


namespace VRChatLogEventOSC.Setting
{
    internal class SettingWindowViewModel : INotifyPropertyChanged, IDisposable, IClosing
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private SettingWindowModel _model;

        private ReactivePropertySlim<string> _selectedEvent = new(string.Empty);
        public ReadOnlyReactivePropertySlim<string> SelectedEvent { get; init; }

        private readonly Dictionary<RegexPattern.EventTypeEnum, ReactiveCommand> _eventsButtonCommand = new()
        {
            {RegexPattern.EventTypeEnum.JoinedRoomURL, new()},
            {RegexPattern.EventTypeEnum.JoinedRoomName, new()},
            {RegexPattern.EventTypeEnum.AcceptFriendRequest, new()},
            {RegexPattern.EventTypeEnum.PlayedVideo1, new()},
            {RegexPattern.EventTypeEnum.PlayedVideo2, new()},
            {RegexPattern.EventTypeEnum.AcceptInvite, new()},
            {RegexPattern.EventTypeEnum.AcceptRequestInvite, new()},
            {RegexPattern.EventTypeEnum.OnPlayerJoined, new()},
            {RegexPattern.EventTypeEnum.OnPlayerLeft, new()},
            {RegexPattern.EventTypeEnum.TookScreenshot, new()},
        };
        public IReadOnlyDictionary<RegexPattern.EventTypeEnum, ReactiveCommand> EventsButtonCommand { get; init; }
        public ReadOnlyReactiveCollection<SingleSetting> SelectedTypeSettings { get; set; }

        // public ReactiveCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; init; } = new();
        public ReactivePropertySlim<SingleSetting?> SelectedItem { get; init; } = new();
        public ReactivePropertySlim<int> SelectedIndex { get; init; } = new();
        public ReactiveCommand UpCommand { get; init; }
        public ReactiveCommand DownCommand { get; init; }
        public ReactiveCommand AddCommand { get; init; }
        public ReactiveCommand EditCommand { get; init; }
        public ReactiveCommand DeleteCommand { get; init; }
        public ReactiveCommand ApplyCommand { get; init; }

        private ReactivePropertySlim<bool> _isSelected;
        private readonly CompositeDisposable _compositeDisposable = new();
        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _compositeDisposable.Dispose();
        }

        public void Closing(CancelEventArgs cancelEventArgs)
        {
            if (!_model.IsDirty)
            {
                return;
            }

            var result = MessageBox.Show("現在の設定を保存しますか?", "Closing", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel)
            {
                cancelEventArgs.Cancel = true;
                return;
            }
            else if (result == MessageBoxResult.Yes)
            {
                _model.ApplySetting();
                MessageBox.Show("設定が適用されました", "Apply", MessageBoxButton.OK);
                return;
            }
        }

        public SettingWindowViewModel()
        {
            _model = SettingWindowModel.Instance;
            EventsButtonCommand = _eventsButtonCommand;
            SelectedTypeSettings = _model.ShownSetting;
            SelectedEvent = _selectedEvent.ToReadOnlyReactivePropertySlim<string>();
            _selectedEvent.AddTo(_compositeDisposable);
            SelectedEvent.AddTo(_compositeDisposable);
            SelectedItem.AddTo(_compositeDisposable);
            _isSelected = new ReactivePropertySlim<bool>(false).AddTo(_compositeDisposable);

            SelectedItem.Subscribe(item => {
                _model.SelectedSetting = item;
                if (item == null)
                {
                    _isSelected.Value = false;
                }
                else
                {
                    _isSelected.Value = true;
                }
            }).AddTo(_compositeDisposable);

            SelectedIndex.Subscribe(index => _model.SelectedIndex = index);

            UpCommand = _isSelected.ToReactiveCommand()
            .WithSubscribe(() =>
            {
                _model.UpSelectedItem();
            }).AddTo(_compositeDisposable);

            DownCommand = _isSelected.ToReactiveCommand()
            .WithSubscribe(() =>
            {
                _model.DownSelectedItem();
            }).AddTo(_compositeDisposable);

            AddCommand = new ReactiveCommand()
            .WithSubscribe(() =>
            {
                _model.OpenEditorAsAdd();
            }).AddTo(_compositeDisposable);

            EditCommand = _isSelected.ToReactiveCommand()
            .WithSubscribe(() =>
            {
                _model.OpenEditorAsEdit();
            }).AddTo(_compositeDisposable);
            
            DeleteCommand = _isSelected.ToReactiveCommand()
            .WithSubscribe(() =>
            {
                _model.DeleteSetting();
            }).AddTo(_compositeDisposable);

            ApplyCommand = new ReactiveCommand()
            .WithSubscribe(() =>
            {
                _model.ApplySetting();
                MessageBox.Show("設定が適用されました", "Apply", MessageBoxButton.OK);
            }).AddTo(_compositeDisposable);

            foreach (var type in Enum.GetValues<RegexPattern.EventTypeEnum>())
            {
                if (!_eventsButtonCommand.ContainsKey(type))
                {
                    continue;
                }
                var isButtonChecked = _eventsButtonCommand[type];
                isButtonChecked.AddTo(_compositeDisposable);
                isButtonChecked.SubscribeOnUIDispatcher().Subscribe(_ =>
                {
                    _selectedEvent.Value = type.ToString();
                    _model.ChangeShownSetting(type);
                }).AddTo(_compositeDisposable);
            }
        }
    }
}
