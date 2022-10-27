using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VRChatLogEventOSC.Common;

namespace VRChatLogEventOSC
{
    internal class EditorWindowModel
    {
        private static EditorWindowModel? _instance;
        public static EditorWindowModel Instance => _instance ??= new EditorWindowModel();

        private readonly SettingWindowModel _settingModel = SettingWindowModel.Instance;
        public RegexPattern.EventTypeEnum EventType => _settingModel.ShownEventType;

        public void AddSetting(RegexPattern.EventTypeEnum eventType, SingleSetting setting)
        {
            _settingModel.AddSetting(eventType, setting);
        }

        private EditorWindowModel()
        {

        }
    }
}
