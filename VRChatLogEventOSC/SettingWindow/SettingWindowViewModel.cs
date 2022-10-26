using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Windows.Controls;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using VRChatLogEventOSC.Common;

using System.Diagnostics;


namespace VRChatLogEventOSC
{
    internal class SettingWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private SettingWindowModel _model;

        private ReactivePropertySlim<string> _selectedEvent = new(string.Empty);
        public ReadOnlyReactivePropertySlim<string> SelectedEvent { get; init; }

        private readonly Dictionary<Common.RegexPattern.EventTypeEnum, ReactiveCommand> _eventsButtonCommand = new()
        {
            {Common.RegexPattern.EventTypeEnum.JoinedRoomURL, new()},
            {Common.RegexPattern.EventTypeEnum.JoinedRoomName, new()},
            {Common.RegexPattern.EventTypeEnum.AcceptFriendRequest, new()},
            {Common.RegexPattern.EventTypeEnum.PlayedVideo1, new()},
            {Common.RegexPattern.EventTypeEnum.PlayedVideo2, new()},
            {Common.RegexPattern.EventTypeEnum.AcceptInvite, new()},
            {Common.RegexPattern.EventTypeEnum.AcceptRequestInvite, new()},
            {Common.RegexPattern.EventTypeEnum.OnPlayerJoined, new()},
            {Common.RegexPattern.EventTypeEnum.OnPlayerLeft, new()},
            {Common.RegexPattern.EventTypeEnum.TookScreenshot, new()},
        };
        public IReadOnlyDictionary<Common.RegexPattern.EventTypeEnum, ReactiveCommand> EventsButtonCommand { get; init; }
        public ReadOnlyReactiveCollection<Common.SingleSetting> SelectedTypeSettings { get; set; }

        // public ReactiveCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; init; } = new();
        public ReactivePropertySlim<SingleSetting> SelectedItem { get; init; } = new();
        public ReactivePropertySlim<int> SelectedIndex { get; init; } = new();
        public ReactiveCommand UpCommand { get; init; }
        public ReactiveCommand DownCommand { get; init; }
        public ReactiveCommand AddCommand { get; init; }
        public ReactiveCommand EditCommand { get; init; }
        public ReactiveCommand DeleteCommand { get; init; }
        public ReactiveCommand ApplyCommand { get; init; }
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

        public SettingWindowViewModel()
        {
            _model = SettingWindowModel.Instance;
            EventsButtonCommand = _eventsButtonCommand;
            SelectedTypeSettings = _model.ShownSetting;
            SelectedEvent = _selectedEvent.ToReadOnlyReactivePropertySlim<string>();
            _selectedEvent.AddTo(_compositeDisposable);
            SelectedEvent.AddTo(_compositeDisposable);
            SelectedItem.AddTo(_compositeDisposable);

            UpCommand = new ReactiveCommand().WithSubscribe(() =>
            {
                Debug.Print(SelectedIndex.Value.ToString());
                _model.SwapItem(SelectedIndex.Value, SelectedIndex.Value -1);
            }).AddTo(_compositeDisposable);

            DownCommand = new ReactiveCommand().WithSubscribe(() =>
            {
                Debug.Print(SelectedIndex.Value.ToString());
                _model.SwapItem(SelectedIndex.Value, SelectedIndex.Value + 1);
            }).AddTo(_compositeDisposable);

            AddCommand = new ReactiveCommand().WithSubscribe(() =>
            {
                var editor = new EditorWindow();
                editor.ShowDialog();
            }).AddTo(_compositeDisposable);
            EditCommand = new ReactiveCommand().WithSubscribe(() => { }).AddTo(_compositeDisposable);
            DeleteCommand = new ReactiveCommand().WithSubscribe(() => { }).AddTo(_compositeDisposable);
            ApplyCommand = new ReactiveCommand().WithSubscribe(() =>
            {
                _model.ApplySetting();
            }).AddTo(_compositeDisposable);

            foreach (var type in Enum.GetValues<Common.RegexPattern.EventTypeEnum>())
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
