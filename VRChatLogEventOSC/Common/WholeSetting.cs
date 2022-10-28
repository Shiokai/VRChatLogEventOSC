using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using static VRChatLogEventOSC.Common.RegexPattern;

namespace VRChatLogEventOSC.Common
{
    internal sealed record class WholeSetting
    {
        public int JsonVersion { get; init; } = 0;

        private readonly Dictionary<EventTypeEnum, List<SingleSetting>> _settings;

        public IReadOnlyDictionary<EventTypeEnum, IReadOnlyList<SingleSetting>> Settings { get; }

        public override string ToString()
        {
            StringBuilder result = new();
            foreach (var kvp in Settings)
            {
                result.Append(kvp.Key.ToString());
                result.Append(": {");
                foreach (var val in kvp.Value)
                {
                    result.Append(val.ToString());
                }
                result.Append("}\n");
            }
            return result.ToString();
        }

        /// <summary>
        /// イベント毎の設定の一覧が空の設定
        /// </summary>
        /// <returns>空の設定</returns>
        public static Dictionary<EventTypeEnum, List<SingleSetting>> CreateEmptyWholeSettingDict()
        {
            var settings = new Dictionary<EventTypeEnum, List<SingleSetting>>();
            
            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }
                settings.Add(type, new(0));
            }
            return settings;
        }

        /// <summary>
        /// イベント毎の設定にデフォルト値で一つ追加された設定
        /// </summary>
        /// <returns>デフォルト値一つで構成された設定</returns>
        public static Dictionary<EventTypeEnum, List<SingleSetting>> CreateDefaultWholeSettingDict()
        {
            var settings = new Dictionary<EventTypeEnum, List<SingleSetting>>();
            
            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }
                settings.Add(type, new(){new(oscAddress: "/avatar/parameters/" + type.ToString())});
            }
            return settings;
        }

        public WholeSetting()
        {
            _settings = CreateEmptyWholeSettingDict();
            var settingsBase = new Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>>();
            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }
                settingsBase.Add(type, _settings[type].AsReadOnly());
            }
            Settings = settingsBase;
        }

        public WholeSetting(Dictionary<EventTypeEnum, List<SingleSetting>> settings)
        {
            _settings = settings;
            var settingsBase = new Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>>();
            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }
                settingsBase.Add(type, _settings[type].AsReadOnly());
            }
            Settings = settingsBase;
        }

        [JsonConstructor]
        public WholeSetting(int jsonVersion, IReadOnlyDictionary<EventTypeEnum, IReadOnlyList<SingleSetting>> settings)
        {
            // 設定ファイルの書式が変わった場合バージョンを見てマイグレート
            // if (jsonVersion < JsonVersion)
            // {
            //     MigrateSetting();
            // }
            // else
            // {
            //     JsonVersion = jsonVersion;
            // }

            var convertedSetting = new Dictionary<EventTypeEnum, List<SingleSetting>>();
            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }
                
                if (settings.TryGetValue(type, out var value))
                {
                    convertedSetting.Add(type, value.ToList());
                }
                else
                {
                    convertedSetting.Add(type, new List<SingleSetting>(0));
                }
            }

            _settings = convertedSetting;

            var settingsBase = new Dictionary<EventTypeEnum, IReadOnlyList<SingleSetting>>();
            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None)
                {
                    continue;
                }
                settingsBase.Add(type, _settings[type].AsReadOnly());
            }

            Settings = settingsBase;
        }

    }
}
