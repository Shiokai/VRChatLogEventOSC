using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VRChatLogEventOSC.Common
{
    internal sealed record class SingleSetting
    {
        public enum OSCTypeEnum
        {
            Button,
            Toggle,
        }
        public enum OSCValueTypeEnum
        {
            Bool,
            Int,
            Float,
            String,
        }

        public enum ReqInvEnum
        {
            /// <summary>
            /// 指定しない
            /// </summary>
            NotSpecified,
            /// <summary>
            /// Invite+以外
            /// </summary>
            None,
            /// <summary>
            /// Invite+
            /// </summary>
            CanRequestInvite,
        }

        public string SettingName { get; private set; } = string.Empty;
        public string OSCAddress { get; private set; } = string.Empty;
        public bool? OSCBool {get; private set;} = null;
        public int? OSCInt {get; private set;} = null;
        public float? OSCFloat {get; private set;} = null;
        public string? OSCString {get; private set;} = null;

        /// <summary>
        /// 種類と値を自動的に振り分けて代入します
        /// </summary>
        [JsonIgnore]
        public object? OSCValue
        {
            get
            {
                return OSCValueType switch
                {
                    OSCValueTypeEnum.Bool => OSCBool,
                    OSCValueTypeEnum.Int => OSCInt,
                    OSCValueTypeEnum.Float => OSCFloat,
                    OSCValueTypeEnum.String => OSCString,
                    _ => null
                };
            }
            private set
            {
                switch (value)
                {
                    case bool bval:
                        OSCValueType = OSCValueTypeEnum.Bool;
                        OSCBool = bval;
                        break;
                    case int ival:
                        OSCValueType = OSCValueTypeEnum.Int;
                        OSCInt = ival;
                        break;
                    case float fval:
                        OSCValueType = OSCValueTypeEnum.Float;
                        OSCFloat = fval;
                        break;
                    case string sval:
                        OSCValueType = OSCValueTypeEnum.String;
                        OSCString = sval;
                        break;
                    case null:
                    {
                        switch (OSCValueType)
                        {
                            case OSCValueTypeEnum.Bool:
                                OSCBool = null;
                                break;
                            case OSCValueTypeEnum.Int:
                                OSCInt = null;
                                break;
                            case OSCValueTypeEnum.Float:
                                OSCFloat = null;
                                break;
                            case OSCValueTypeEnum.String:
                                OSCString = null;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                    default:
                        break;
                }
            }
        }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OSCValueTypeEnum OSCValueType { get; private set; } = OSCValueTypeEnum.Int;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OSCTypeEnum OSCType { get; private set; } = OSCTypeEnum.Button;
        public string UserName { get; private set; } = string.Empty;
        public string UserID { get; private set; } = string.Empty;
        public string WorldName { get; private set; } = string.Empty;
        public string WorldURL { get; private set; } = string.Empty;
        public string WorldID { get; private set; } = string.Empty;
        public string InstanceID { get; private set; } = string.Empty;
        public string InstanceType { get; private set; } = string.Empty;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ReqInvEnum ReqInv {get; private set;} = ReqInvEnum.NotSpecified;
        public string WorldUserID { get; private set; } = string.Empty;
        public string Region { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public string URL { get; private set; } = string.Empty;

        private readonly Dictionary<string, string> _nameToProperty;

        /// <summary>
        /// 名前付きグループの名前と設定値の対応付け
        /// </summary>
        /// <returns>対応付けされたDictionary</returns>
        private Dictionary<string, string> MapNameToProperty()
        {
            return new()
            {
                {nameof(SettingName), SettingName},
                {nameof(UserName), UserName},
                {nameof(UserID), UserID},
                {nameof(WorldName), WorldName},
                {nameof(WorldURL), WorldURL},
                {nameof(WorldID), WorldID},
                {nameof(InstanceID), InstanceID},
                {nameof(InstanceType), InstanceType},
                {nameof(ReqInv), ReqInv.ToString()},
                {nameof(WorldUserID), WorldUserID},
                {nameof(Region), Region},
                {nameof(Message), Message},
                {nameof(URL), URL},
            };
        }

        /// <summary>
        /// 名前付きグループに対応する設定値
        /// </summary>
        /// <param name="capture">名前付きグループの名前</param>
        /// <returns>対応する設定値</returns>
        public string CaptureProperty(string capture)
        {
            return _nameToProperty[capture];
        }

        public SingleSetting(
            string settingName = "",
            string oscAddress = "/avatar/parameters/empty",
            object? oscValue = null,
            OSCTypeEnum oscType = OSCTypeEnum.Button,
            string userName = "",
            string userID = "",
            string worldName = "",
            string worldURL = "",
            string worldID = "",
            string instanceID = "",
            string instanceType = "",
            ReqInvEnum reqInv = ReqInvEnum.NotSpecified,
            string worldUserID = "",
            string region = "",
            string message = "",
            string url = ""
        )
        {
            SettingName = settingName;
            OSCAddress = oscAddress;
            OSCValue = oscValue;
            OSCType = oscType;
            UserName = userName;
            UserID = userID;
            WorldName = worldName;
            WorldURL = worldURL;
            WorldID = worldID;
            InstanceID = instanceID;
            InstanceType = instanceType;
            ReqInv = reqInv;
            WorldUserID = worldUserID;
            Region = region;
            Message = message;
            URL = url;

            _nameToProperty = MapNameToProperty();
        }
        /// <summary>
        /// Only for Json Constructor.
        /// </summary>
        [JsonConstructor]
        public SingleSetting(
            string settingName,
            string oscAddress,
            bool? oscBool,
            int? oscInt,
            float? oscFloat,
            string? oscString,
            OSCValueTypeEnum oscValueType,
            OSCTypeEnum oscType,
            string userName,
            string userID,
            string worldName,
            string worldURL,
            string worldID,
            string instanceID,
            string instanceType,
            ReqInvEnum reqInv,
            string worldUserID,
            string region,
            string message,
            string url
        )
        {
            SettingName = settingName;
            OSCAddress = oscAddress;
            OSCBool = oscBool;
            OSCInt = oscInt;
            OSCFloat = oscFloat;
            OSCString = oscString;
            OSCValueType = oscValueType;
            OSCType = oscType;
            UserName = userName;
            UserID = userID;
            WorldName = worldName;
            WorldURL = worldURL;
            WorldID = worldID;
            InstanceID = instanceID;
            InstanceType = instanceType;
            ReqInv = reqInv;
            WorldUserID = worldUserID;
            Region = region;
            Message = message;
            URL = url;

            _nameToProperty = MapNameToProperty();
        }

    }
}
