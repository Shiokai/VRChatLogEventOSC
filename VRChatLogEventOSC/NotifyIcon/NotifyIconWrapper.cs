using System;
using System.ComponentModel;
using System.Drawing;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

using Application = System.Windows.Application;

namespace VRChatLogEventOSC.SystrayIcon
{
    internal class NotifyIconWrapper : FrameworkElement, IDisposable
    {
        private readonly NotifyIcon? _notifyIcon;
        private readonly ContextMenuStrip? _contextMenuStrip;
        private readonly ToolStripMenuItem _openControlItem = new("Open Control");
        private readonly ToolStripMenuItem _openSetingItem = new("Open Setting");
        private readonly ToolStripMenuItem _quitItem = new("Quit");
        private readonly ToolStripMenuItem _pauseItem = new("Pause [ ]");

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
        public string PauseItemText
        {
            get => _pauseItem.Text;
            set => _pauseItem.Text = value;
        }
        public IObservable<EventArgs>? OpenControlSelected { get; private set; }
        public IObservable<EventArgs>? OpenSettingSelected { get; private set; }
        public IObservable<EventArgs>? QuitSelected { get; private set; }
        public IObservable<EventArgs>? DoubleClicked { get; private set; }
        public IObservable<EventArgs>? PauseSelected { get; private set; }

        public void RequestNotify(NotifyRequestRecord record)
        {
            _notifyIcon?.ShowBalloonTip(record.Duration, record.Title, record.Text, record.Icon);
        }

        public void Dispose()
        {
            _notifyIcon?.Dispose();
        }
        private ContextMenuStrip CreateContextMenu()
        {
            OpenControlSelected = Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => _openControlItem.Click += h,
                h => _openControlItem.Click -= h
            );
            OpenSettingSelected = Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => _openSetingItem.Click += h,
                h => _openSetingItem.Click -= h
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
            // Quitは間隔を空ける
            var contextMenu = new ContextMenuStrip { Items = { _openControlItem, _openSetingItem, _pauseItem, separator, _quitItem } };
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
