using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

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
        public ReactiveCollection<TestClass> Test { get; init; } = new()
        {
            new(),
            new(),
            new(){SettingName = "Fuga"}
        };

        public class TestClass
        {
            public string SettingName { get; set; } = "Hoge";
        }

        private readonly CompositeDisposable _conpositeDisposable = new();
        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _conpositeDisposable.Dispose();
        }

        public SettingWindowViewModel()
        {
            _model = SettingWindowModel.Instance;
            EventsButtonCommand = _eventsButtonCommand;
            SelectedTypeSettings = _model.ShownSetting;
            SelectedEvent = _selectedEvent.ToReadOnlyReactivePropertySlim<string>();
            _selectedEvent.AddTo(_conpositeDisposable);
            SelectedEvent.AddTo(_conpositeDisposable);
            foreach (var type in Enum.GetValues<Common.RegexPattern.EventTypeEnum>())
            {
                if (!_eventsButtonCommand.ContainsKey(type))
                {
                    continue;
                }
                var isButtonChecked = _eventsButtonCommand[type];
                isButtonChecked.AddTo(_conpositeDisposable);
                isButtonChecked.SubscribeOnUIDispatcher().Subscribe(_ =>
                {
                    _selectedEvent.Value = type.ToString();
                    _model.ChangeShownSetting(type);
                }).AddTo(_conpositeDisposable);
            }
        }
    }
}
