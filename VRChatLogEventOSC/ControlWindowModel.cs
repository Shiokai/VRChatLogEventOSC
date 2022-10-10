using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VRChatLogEventOSC
{
    internal class ControlWindowModel
    {
        private static ControlWindowModel? _instance;
        public static ControlWindowModel Instance => _instance ??= new ControlWindowModel();
        private LogEventModel _logEventModel;

        public void PuaseLogWEvent()
        {
            _logEventModel.Pause();
        }

        public void RestartLogEvent()
        {
            _logEventModel.Restart();
        }

        public void RestartLogEventWithScan()
        {
            _logEventModel.RestartWithScan();
        }

        public void Rescan()
        {
            _logEventModel.Rescan();
        }

        public void QuitApplication()
        {
            Application.Current.Shutdown();
        }

        private ControlWindowModel()
        {
            _logEventModel = LogEventModel.Instance;
        }
    }
}
