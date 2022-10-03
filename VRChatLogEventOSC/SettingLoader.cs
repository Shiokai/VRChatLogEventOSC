using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace VRChatLogEventOSC
{
    internal class SettingLoader
    {
        public string SettingFilePath { get; set; } = "./setting.json";
        JsonSerializerOptions options = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };
        public void SaveSetting(WholeSetting setting)
        {
            string json = JsonSerializer.Serialize<IReadOnlyDictionary<RegexPattern.EventTypeEnum, IReadOnlyList<SingleSetting>>>(setting.Settings, options);
            File.WriteAllText(SettingFilePath, json);
        }

        // public void SaveSetting(WholeSetting setting)
        // {
        //     string json = JsonSerializer.Serialize<WholeSetting>(setting, options);
        //     File.WriteAllText(SettingFilePath, json);
        // }

        public WholeSetting? LoadSetting()
        {
            string json = File.ReadAllText(SettingFilePath);
            var setting = JsonSerializer.Deserialize<Dictionary<RegexPattern.EventTypeEnum, List<SingleSetting>>>(json);
            return new(setting ?? WholeSetting.CreateDefaultWholeSettingDict());
        }

        // public WholeSetting? LoadSetting()
        // {
        //     string json = File.ReadAllText(SettingFilePath);
        //     var setting = JsonSerializer.Deserialize<WholeSetting>(json);
        //     return setting;
        // }
    }
}
