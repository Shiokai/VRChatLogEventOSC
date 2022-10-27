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
        /// <summary>
        /// Editで開かれた場合に読み込む設定
        /// </summary>
        public SingleSetting? OldSetting => _settingModel.SelectedSetting;

        /// <summary>
        /// 現在編集中の値をSettingWindowに反映します
        /// AddでEditorWindowが開かれた場合は末尾に追加され、Editで開かれた場合選択された設定を上書きします
        /// </summary>
        /// <param name="setting"></param>
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
