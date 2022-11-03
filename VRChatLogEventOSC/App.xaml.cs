using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VRChatLogEventOSC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SystrayIcon.NotifyIconViewModel? _notifyIcon;

        public void StartUp(object sender, StartupEventArgs e)
        {
            foreach (var arg in e.Args)
            {
                // 設定のみ行う場合
                if (arg == "--setting" || arg == "-s")
                {
                    ShutdownMode = ShutdownMode.OnLastWindowClose;
                    var settingWindow = new Setting.SettingWindow();
                    settingWindow.Show();
                    break;
                }
            }
            CreateNotifyIcon();
        }
        private void CreateNotifyIcon()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            _notifyIcon = new SystrayIcon.NotifyIconViewModel();
        }
    }
}
