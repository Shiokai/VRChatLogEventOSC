using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

using VRChatLogEventOSC.Model;

namespace VRChatLogEventOSC
{
    internal static class FileLoader
    {
        private static readonly string _settingFilePath = "./setting.json";
        private static readonly string _configFilePath = "./config.json";
        private static readonly JsonSerializerOptions _options = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };
        public static void SaveSetting(WholeSetting setting)
        {
            using var stream = new FileStream(_settingFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            JsonSerializer.Serialize<WholeSetting>(stream, setting, _options);
        }

        public static Task SaveSettingAsync(WholeSetting setting)
        {
            using var stream = new FileStream(_settingFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            var task = JsonSerializer.SerializeAsync<WholeSetting>(stream, setting, _options);
            return task;
        }

        public static void SaveConfig(ConfigData config)
        {
            using var stream = new FileStream(_configFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            JsonSerializer.Serialize<ConfigData>(stream, config, _options);
        }

        public static Task SaveConfigAsync(ConfigData config)
        {
            using var stream = new FileStream(_settingFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            var task = JsonSerializer.SerializeAsync<ConfigData>(stream, config, _options);
            return task;
        }

        public static WholeSetting? LoadSetting()
        {
            string json = File.ReadAllText(_settingFilePath);
            var setting = JsonSerializer.Deserialize<WholeSetting>(json);
            return setting;
        }

        public static ValueTask<WholeSetting?> LoadSettingAsync()
        {
            using var stream = new FileStream(_settingFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            var setting = JsonSerializer.DeserializeAsync<WholeSetting>(stream);
            return setting;
        }

        public static ConfigData? LoadConfig()
        {
            string json = File.ReadAllText(_configFilePath);
            var config = JsonSerializer.Deserialize<ConfigData>(json);
            return config;
        }

        public static ValueTask<ConfigData?> LoadConfigAsync()
        {
            using var stream = new FileStream(_configFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            var config = JsonSerializer.DeserializeAsync<ConfigData>(stream);
            return config;
        }
    }
}
