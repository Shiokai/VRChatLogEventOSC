using System;
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
        private LogFileWatcher _watcher;
        private LineClassifier _lineClassifier;
        private OSCSender _oscSender;
        private EventToOSCConverter _eventToOSCConverter;
        public MainWindow()
        {
            InitializeComponent();
            _watcher = new LogFileWatcher();
            _lineClassifier = new LineClassifier(_watcher);
            _oscSender = new OSCSender();
            _eventToOSCConverter = new EventToOSCConverter(_lineClassifier, _oscSender);
            var jsonLoader = new SettingLoader();
            // var test = WholeSetting.CreateEmptyWholeSettingDict(0);
            var test = WholeSetting.CreateDefaultWholeSettingDict();
            // test[RegexPattern.EventTypeEnum.OnPlayerJoined].Add(new(settingName: "Test"));
            test[RegexPattern.EventTypeEnum.JoinedRoom1Detail].Add(new(reqInv: SingleSetting.ReqInvEnum.None, oSCInt: 1));
            test[RegexPattern.EventTypeEnum.OnPlayerJoined].Add(new(reqInv: SingleSetting.ReqInvEnum.None, oSCInt: 1));
            jsonLoader.SaveSetting(new WholeSetting(test));
            // jsonLoader.WriteAsJson(new SettingBase(settingName: "Test", userName: "Shiokai", message: "hoge"));
            var setting = jsonLoader.LoadSetting();
            _eventToOSCConverter.CurrentSetting = setting ?? new(WholeSetting.CreateDefaultWholeSettingDict());
            _watcher.StartWatchingFromTop();
            // Debug.Print(setting?.ToString());



            // _lineClassifier.EventReactiveProperties[RegexPattern.EventTypeEnum.OnPlayerJoined].Subscribe(_ => Debug.Print("============================================"));
        }
    }
}
