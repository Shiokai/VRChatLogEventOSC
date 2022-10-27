using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VRChatLogEventOSC.Common;
using VRChatLogEventOSC.Setting;

namespace VRChatLogEventOSC.Editor
{
    internal class EditorWindowModel
    {
        private static EditorWindowModel? _instance;
        public static EditorWindowModel Instance => _instance ??= new EditorWindowModel();

        private readonly SettingWindowModel _settingModel = SettingWindowModel.Instance;
        public RegexPattern.EventTypeEnum EventType => _settingModel.ShownEventType;
        public SingleSetting? OldSetting => _settingModel.SelectedSetting;

        public void ApplyEdited(SingleSetting setting)
        {
            if (OldSetting == null)
            {
                _settingModel.AddSetting(setting);
            }
            else
            {
                _settingModel.EditOverrideSetting(setting);
            }
        }

        private EditorWindowModel()
        {
            
        }
    }
}
