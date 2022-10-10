using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Reactive.Bindings;

using System.Diagnostics;

namespace VRChatLogEventOSC
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private LogEventModel _model;
        private NotifyIconWrapper _notifyIcon = new();
        private bool _isRunning = true;

        public MainWindowViewModel()
        {
            _model = LogEventModel.Instance;
            LoadedCommand = new ReactiveCommand();
            LoadedCommand.Subscribe(Loaded);
            ClosingCommand = new ReactiveCommand<CancelEventArgs?>();
            ClosingCommand.Subscribe(e => Closing(e));
            NotifyCommand = new ReactiveCommand();
            NotifyCommand.Subscribe(() => Notify("Hello world!"));
            _notifyIcon.OpenSelected?.Subscribe(_ => WindowStateReactive.Value = WindowState.Normal);
            _notifyIcon.QuitSelected?.Subscribe(_ => Application.Current.Shutdown());
            _isRunning = _model.IsRunnging.Value;
            _notifyIcon.PauseSelected?.Subscribe(_ => 
            {
                if (_isRunning)
                {
                    _model.Pause();
                }
                else
                {
                    _model.Restart();
                }
            });

            PauseCommand.Subscribe(_model.Pause);

            WindowStateReactive.Subscribe(state =>
            {
                ShowInTaskbar.Value = true;
                ShowInTaskbar.Value = state != WindowState.Minimized;
                Style.Value = state != WindowState.Minimized ? WindowStyle.SingleBorderWindow : WindowStyle.ToolWindow;
            });

            _notifyIcon.Text = _isRunning ? "VRChatLogEventOSC: Running" : "VRChatLogEventOSC: Paused";

            _model.IsRunnging.Subscribe(running =>
            {
                if (running)
                {
                    _notifyIcon.Text = "VRChatLogEventOSC: Running";

                }
                else
                {
                    _notifyIcon.Text = "VRChatLogEventOSC: Paused";
                }
                _isRunning = running;
            });

            Application.Current.Exit += (obj, args) =>
            {
                _notifyIcon.Dispose();
                _model.Dispose();
            };
        }

        public ReactiveCommand LoadedCommand { get; }
        public ReactiveCommand<CancelEventArgs?> ClosingCommand { get; }
        public ReactiveCommand NotifyCommand { get; }
        public ReactiveCommand PauseCommand { get; } = new();

        public ReactiveProperty<WindowState> WindowStateReactive { get; set; } = new();

        public ReactiveProperty<bool> ShowInTaskbar { get; set; } = new();
        public ReactiveProperty<WindowStyle> Style { get; set; } = new(WindowStyle.ToolWindow);

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

        private void Loaded()
        {
            WindowStateReactive.Value = WindowState.Minimized;
        }

        private void Closing(CancelEventArgs? e)
        {
            if (e == null)
                return;
            e.Cancel = true;
            WindowStateReactive.Value = WindowState.Minimized;
        }
    }
}