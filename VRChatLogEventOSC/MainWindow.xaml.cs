﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;

namespace VRChatLogEventOSC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // private LogFileWatcher _watcher;
        // private LineClassifier _lineClassifier;
        // private OSCSender _oscSender;
        // private EventToOSCConverter _eventToOSCConverter;
        public MainWindow()
        {
            InitializeComponent();
            // _watcher = new LogFileWatcher();
            // _lineClassifier = new LineClassifier(_watcher);
            // _oscSender = new OSCSender();
            // _eventToOSCConverter = new EventToOSCConverter(_lineClassifier, _oscSender);
            // var jsonLoader = new SettingLoader();
            // var test = WholeSetting.CreateEmptyWholeSettingDict(0);
            // var test = WholeSetting.CreateDefaultWholeSettingDict();
            // test[RegexPattern.EventTypeEnum.OnPlayerJoined].Add(new(settingName: "Test"));
            // for (int i = 0; i < 50; i++)
            // {
            //     test[RegexPattern.EventTypeEnum.JoinedRoomURL].Add(new(reqInv: SingleSetting.ReqInvEnum.None, oscValue: i, oscType: SingleSetting.OSCTypeEnum.Toggle, oscAddress: "/avatars/parameters/JoinedRoomURL"));
            // }
            // test[RegexPattern.EventTypeEnum.JoinedRoomURL].Add(new(reqInv: SingleSetting.ReqInvEnum.None, oscValue: 5, oscType: SingleSetting.OSCTypeEnum.Toggle, oscAddress: "/avatars/parameters/JoinedRoomURL"));
            // test[RegexPattern.EventTypeEnum.OnPlayerJoined].Add(new(reqInv: SingleSetting.ReqInvEnum.None, oscValue: true, oscType: SingleSetting.OSCTypeEnum.Button, oscAddress: "/avatars/parameters/OnPlayerJoined"));
            // test[RegexPattern.EventTypeEnum.OnPlayerLeft].Add(new(reqInv: SingleSetting.ReqInvEnum.None, oscValue: true, oscType: SingleSetting.OSCTypeEnum.Button, oscAddress: "/avatars/parameters/OnPlayerLeft"));
            // jsonLoader.SaveSetting(new WholeSetting(test));
            // jsonLoader.WriteAsJson(new SettingBase(settingName: "Test", userName: "Shiokai", message: "hoge"));
            // var setting = jsonLoader.LoadSetting();
            // Debug.Print(setting?.ToString());
            // _eventToOSCConverter.CurrentSetting = setting ?? new(WholeSetting.CreateDefaultWholeSettingDict());
            // _oscSender.ButtomInterval = 5;
            // _oscSender.ChangeClient(9005);
            // _watcher.IsDetectFileCreation = true;
            // _watcher.StartWatchingFromTop();
            // _watcher.StartWatchingFromCurrent();
            // Debug.Print(setting?.ToString());
            



            // _lineClassifier.EventReactiveProperties[RegexPattern.EventTypeEnum.OnPlayerJoined].Subscribe(_ => Debug.Print("============================================"));
        }
    }
}
