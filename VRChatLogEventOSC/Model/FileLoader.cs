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
            using var stream = new FileStream(_configFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            JsonSerializer.Serialize<ConfigData>(stream, config, _options);
        }

        public static Task SaveConfigAsync(ConfigData config)
        {
            using var stream = new FileStream(_settingFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            var task = JsonSerializer.SerializeAsync<ConfigData>(stream, config, _options);
            return task;
        }

        public static WholeSetting? LoadSetting()
        {
            if (File.Exists(_settingFilePath))
            {
                SaveSetting(new WholeSetting(WholeSetting.CreateEmptyWholeSettingDict()));
            }

            using var stream = new FileStream(_settingFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var setting = JsonSerializer.Deserialize<WholeSetting>(stream);
            return setting;
        }

        public static ValueTask<WholeSetting?> LoadSettingAsync()
        {
            if (File.Exists(_settingFilePath))
            {
                SaveSetting(new WholeSetting(WholeSetting.CreateEmptyWholeSettingDict()));
            }

            using var stream = new FileStream(_settingFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var setting = JsonSerializer.DeserializeAsync<WholeSetting>(stream);
            return setting;
        }

        public static ConfigData? LoadConfig()
        {
            if (File.Exists(_configFilePath))
            {
                SaveConfig(new ConfigData());
            }

            using var stream = new FileStream(_configFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var config = JsonSerializer.Deserialize<ConfigData>(stream);
            return config;
        }

        public static ValueTask<ConfigData?> LoadConfigAsync()
        {
            if (File.Exists(_configFilePath))
            {
                SaveConfig(new ConfigData());
            }

            using var stream = new FileStream(_configFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var config = JsonSerializer.DeserializeAsync<ConfigData>(stream);
            return config;
        }
    }
}
