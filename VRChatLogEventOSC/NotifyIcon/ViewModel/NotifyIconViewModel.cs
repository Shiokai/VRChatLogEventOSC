using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;

using System.Diagnostics;

namespace VRChatLogEventOSC.SystrayIcon
{
    public sealed class NotifyIconViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private NotifyIconWrapper _notifyIcon = new();
        private NotifyIconModel _model;
        private string _iconTextBase = "VRChatLogEventOSC: ";
        private string _statusTextBase = "Status: ";

        private string CurrentStatus => _model.IsLogEventRunning.Value ? "Running" : "Paused";

        private ReactivePropertySlim<string> _status = new(string.Empty);
        public ReadOnlyReactivePropertySlim<string> Status => _status.ToReadOnlyReactivePropertySlim<string>();

        private CompositeDisposable _compositeDisposable = new();

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

            _notifyIcon.OpenSelected?.Subscribe(_ => _model.OpenControlWindow()).AddTo(_compositeDisposable);
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
                _notifyIcon.Text = _iconTextBase + CurrentStatus;
                _status.Value = _statusTextBase + CurrentStatus;
            });

            _notifyIcon.DoubleClicked?.Subscribe(_ => _model.OpenControlWindow()).AddTo(_compositeDisposable);

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
