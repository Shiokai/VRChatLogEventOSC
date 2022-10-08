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
            using var stream = new FileStream(SettingFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            JsonSerializer.Serialize<WholeSetting>(stream, setting, options);
        }

        public Task SaveSettingAsync(WholeSetting setting)
        {
            using var stream = new FileStream(SettingFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            var task = JsonSerializer.SerializeAsync<WholeSetting>(stream, setting, options);
            return task;
        }

        public WholeSetting? LoadSetting()
        {
            string json = File.ReadAllText(SettingFilePath);
            var setting = JsonSerializer.Deserialize<WholeSetting>(json);
            return setting;
        }

        public ValueTask<WholeSetting?> LoadSettingAsync()
        {
            using var stream = new FileStream(SettingFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            var setting = JsonSerializer.DeserializeAsync<WholeSetting>(stream);
            return setting;
        }
    }
}
