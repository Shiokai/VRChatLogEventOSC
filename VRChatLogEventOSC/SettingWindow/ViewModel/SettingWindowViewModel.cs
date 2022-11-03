using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using VRChatLogEventOSC.Common;

namespace VRChatLogEventOSC.Setting
{
    internal class SettingWindowViewModel : INotifyPropertyChanged, IDisposable, IClosing
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly SettingWindowModel _model;

        private readonly ReactivePropertySlim<string> _selectedEvent = new(string.Empty);
        private readonly ReactivePropertySlim<bool> _isSelecting;
        private readonly CompositeDisposable _compositeDisposable = new();

        private readonly Dictionary<RegexPattern.EventTypeEnum, ReactiveCommand> _eventsButtonCommand = new();

        public IReadOnlyDictionary<RegexPattern.EventTypeEnum, ReactiveCommand> EventsButtonCommand { get; init; }
        public ReadOnlyReactivePropertySlim<string> SelectedEvent { get; init; }
        public ReadOnlyReactiveCollection<SingleSetting> SelectedTypeSettings { get; set; }

        public ReactivePropertySlim<SingleSetting?> SelectedItem { get; init; } = new();
        public ReactivePropertySlim<int> SelectedIndex { get; init; } = new();
        public ReactiveCommand<MouseEventArgs> ItemDoubleClickCommand { get; init; }
        public ReactiveCommand UpCommand { get; init; }
        public ReactiveCommand DownCommand { get; init; }
        public ReactiveCommand AddCommand { get; init; }
        public ReactiveCommand EditCommand { get; init; }
        public ReactiveCommand DeleteCommand { get; init; }
        public ReactiveCommand ApplyCommand { get; init; }

        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _compositeDisposable.Dispose();
            _disposed = true;
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
            
            foreach (var type in Enum.GetValues<RegexPattern.EventTypeEnum>())
            {
                _eventsButtonCommand.Add(type, new ReactiveCommand());
            }

            EventsButtonCommand = _eventsButtonCommand;
            SelectedTypeSettings = _model.ShownSetting;
            SelectedEvent = _selectedEvent.ToReadOnlyReactivePropertySlim<string>();
            _selectedEvent.AddTo(_compositeDisposable);
            SelectedEvent.AddTo(_compositeDisposable);
            SelectedItem.AddTo(_compositeDisposable);
            _isSelecting = new ReactivePropertySlim<bool>(false).AddTo(_compositeDisposable);

            SelectedItem.Subscribe(item =>
            {
                _model.SelectedSetting = item;
                if (item == null)
                {
                    _isSelecting.Value = false;
                }
                else
                {
                    _isSelecting.Value = true;
                }
            }).AddTo(_compositeDisposable);

            SelectedIndex.Subscribe(index => _model.SelectedIndex = index);

            ItemDoubleClickCommand = new ReactiveCommand<MouseEventArgs>()
            .WithSubscribe(e => 
            {
                _model.OpenEditorAsEdit();
            }).AddTo(_compositeDisposable);

            UpCommand = _isSelecting.ToReactiveCommand()
            .WithSubscribe(() =>
            {
                _model.UpSelectedItem();
            }).AddTo(_compositeDisposable);

            DownCommand = _isSelecting.ToReactiveCommand()
            .WithSubscribe(() =>
            {
                _model.DownSelectedItem();
            }).AddTo(_compositeDisposable);

            AddCommand = new ReactiveCommand()
            .WithSubscribe(() =>
            {
                _model.OpenEditorAsAdd();
            }).AddTo(_compositeDisposable);

            EditCommand = _isSelecting.ToReactiveCommand()
            .WithSubscribe(() =>
            {
                _model.OpenEditorAsEdit();
            }).AddTo(_compositeDisposable);

            DeleteCommand = _isSelecting.ToReactiveCommand()
            .WithSubscribe(() =>
            {
                var result = MessageBox.Show("選択した設定を削除しますか?", "Delete", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    return;
                }

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
                isButtonChecked.Subscribe(_ =>
                {
                    _selectedEvent.Value = type.ToString();
                    _model.ChangeShownSetting(type);
                }).AddTo(_compositeDisposable);
            }
        }
    }
}
