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
    public sealed class NotifyIconModel : INotifyPropertyChanged, IDisposable
    {
        private static NotifyIconModel? _instance;
        public static NotifyIconModel Instance => _instance ??= new NotifyIconModel();
        
        public event PropertyChangedEventHandler? PropertyChanged;
        private LogEventModel _logEventModel;
        public ReadOnlyReactivePropertySlim<bool> IsLogEventRunning;

        private CompositeDisposable _compositeDisposable = new();

        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _logEventModel.Dispose();
            _compositeDisposable.Dispose();
        }

        public void PuaseLogWEvent()
        {
            _logEventModel.Pause();
        }

        public void RestartLogEvent()
        {
            _logEventModel.Restart();
        }

        public void OpenControlWindow()
        {
            var controlWindow = new Control.ControlWindow();
            controlWindow.Show();
        }

        private NotifyIconModel()
        {
            _logEventModel = LogEventModel.Instance;

            IsLogEventRunning = _logEventModel.IsRunnging;
            IsLogEventRunning.AddTo(_compositeDisposable);

            Application.Current.Exit += (obj, args) =>
            {
                Dispose();
            };
        }


    }
}
