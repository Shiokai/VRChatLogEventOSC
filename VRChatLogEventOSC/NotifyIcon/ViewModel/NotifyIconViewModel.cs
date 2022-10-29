using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace VRChatLogEventOSC.SystrayIcon
{
    internal sealed class NotifyIconViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly NotifyIconWrapper _notifyIcon = new();
        private readonly NotifyIconModel _model;
        private readonly string _iconTextBase = "VRChatLogEventOSC: ";
        private readonly string _statusTextBase = "Status: ";
        private readonly ReactivePropertySlim<string> _status = new(string.Empty);
        private readonly CompositeDisposable _compositeDisposable = new();

        private string CurrentStatus => _model.IsLogEventRunning.Value ? "Running" : "Paused";

        public ReadOnlyReactivePropertySlim<string> Status => _status.ToReadOnlyReactivePropertySlim<string>();


        private bool _disposed = false;


        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _notifyIcon.Dispose();
            _model.Dispose();
            _compositeDisposable.Dispose();
            _disposed = true;
        }
        private void Notify(string message)
        {
            _notifyIcon.RequestNotify(new NotifyIconWrapper.NotifyRequestRecord
            {
                Title = "Notify",
                Text = message,
                Duration = 1000
            });
        }

        private void Notify(string message, string title)
        {
            _notifyIcon.RequestNotify(new NotifyIconWrapper.NotifyRequestRecord
            {
                Title = title,
                Text = message,
                Duration = 1000
            });
        }

        public NotifyIconViewModel()
        {
            _model = NotifyIconModel.Instance;

            _notifyIcon.OpenControlSelected?.Subscribe(_ => NotifyIconModel.OpenControlWindow()).AddTo(_compositeDisposable);
            _notifyIcon.OpenSettingSelected?.Subscribe(_ => NotifyIconModel.OpenSettingWindow()).AddTo(_compositeDisposable);
            _notifyIcon.QuitSelected?.Subscribe(_ => Application.Current.Shutdown()).AddTo(_compositeDisposable);
            _notifyIcon.PauseSelected?.Subscribe(_ => 
            {
                if (_model.IsLogEventRunning.Value)
                {
                    _model.PuaseLogWEvent();
                }
                else
                {
                    _model.RestartLogEvent();
                }
            }).AddTo(_compositeDisposable);

            _model.IsLogEventRunning.Subscribe(running => 
            {
                // ControlWindowからの変更に合わせるため、PauseSelectedではなくこちらで表示変更
                // PauseSelectedでは間接的にIsLogEventRunningが変更される
                _notifyIcon.Text = _iconTextBase + CurrentStatus;
                _status.Value = _statusTextBase + CurrentStatus;
                _notifyIcon.PauseItemText = $"Pause [{(running ? " " : "✓")}]";
            });

            _notifyIcon.DoubleClicked?.Subscribe(_ => NotifyIconModel.OpenControlWindow()).AddTo(_compositeDisposable);

            _notifyIcon.Text = _iconTextBase + CurrentStatus;
            _status.Value = _statusTextBase + CurrentStatus;

            _compositeDisposable.Add(_status);
            _compositeDisposable.Add(Status);

            Application.Current.Exit += (obj, args) =>
            {
                Dispose();
            };
        }


    }
}
