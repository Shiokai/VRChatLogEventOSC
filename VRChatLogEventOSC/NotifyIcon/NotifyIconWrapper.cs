using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using System.Reactive.Linq;

namespace VRChatLogEventOSC.SystrayIcon
{
    public class NotifyIconWrapper : FrameworkElement, IDisposable
    {
        private readonly NotifyIcon? _notifyIcon;
        private readonly ContextMenuStrip? _contextMenuStrip;

        public string Text
        {
            get => _notifyIcon?.Text ?? string.Empty;
            set
            {
                if (_notifyIcon == null)
                {
                    return;
                }
                _notifyIcon.Text = value;
            }
        }

        private bool _isPaused = false;

        public void RequestNotify(NotifyRequestRecord record)
        {
            _notifyIcon?.ShowBalloonTip(record.Duration, record.Title, record.Text, record.Icon);
        }

        public void Dispose()
        {
            _notifyIcon?.Dispose();
        }

        public IObservable<EventArgs>? OpenSelected;
        public IObservable<EventArgs>? QuitSelected;
        public IObservable<EventArgs>? DoubleClicked;
        public IObservable<EventArgs>? PauseSelected;

        private ToolStripMenuItem _openItem = new("Open Control");
        private ToolStripMenuItem _quitItem = new("Quit");
        private ToolStripMenuItem _pauseItem = new("Pause [ ]");

        private ContextMenuStrip CreateContextMenu()
        {
            OpenSelected = Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => _openItem.Click += h,
                h => _openItem.Click -= h
            );
            PauseSelected = Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => _pauseItem.Click += h,
                h => _pauseItem.Click -= h
            );
            QuitSelected = Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => _quitItem.Click += h,
                h => _quitItem.Click -= h
            );
            var separator = new ToolStripSeparator();
            var contextMenu = new ContextMenuStrip { Items = { _openItem, _pauseItem, separator, _quitItem } };
            return contextMenu;
        }

        public NotifyIconWrapper()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
                
            _contextMenuStrip = CreateContextMenu();

            _notifyIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location),
                Visible = true,
                ContextMenuStrip = _contextMenuStrip,
            };
            
            DoubleClicked = Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => _notifyIcon.DoubleClick += h,
                h => _notifyIcon.DoubleClick -= h
            );

            PauseSelected?.Subscribe(_ => 
            {
                _isPaused = !_isPaused;
                _pauseItem.Text = $"Pause [{(_isPaused ? "✓" : " ")}]";
            });

            Application.Current.Exit += (obj, args) => { _notifyIcon.Dispose(); };
        }
        public class NotifyRequestRecord
        {
            public string Title { get; set; } = "";
            public string Text { get; set; } = "";
            public int Duration { get; set; } = 1000;
            public ToolTipIcon Icon { get; set; } = ToolTipIcon.Info;
        }
    }
}
