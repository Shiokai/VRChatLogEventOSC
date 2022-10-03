using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VRChatLogEventOSC
{
    public class SingleSetting
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
            NotSpecified,
            None,
            CanRequestInvite,
        }

        private static readonly Dictionary<ReqInvEnum, string> _reqInvDict = new()
        {
            {ReqInvEnum.NotSpecified, ""},
            {ReqInvEnum.None, "None"},
            {ReqInvEnum.CanRequestInvite, "~canRequestInvite"}
        };

        public string Version { get; } = "0.0.0";
        public string SettingName { get; private set; } = string.Empty;
        public string OSCAddress { get; private set; } = string.Empty;
        public int? OSCInt {get; private set;} = null;
        public float? OSCFloat {get; private set;} = null;
        public bool? OSCBool {get; private set;} = null;
        public string? OSCString {get; private set;} = null;
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
                    case bool val:
                        OSCBool = val;
                        break;
                    case int val:
                        OSCInt = val;
                        break;
                    case float val:
                        OSCFloat = val;
                        break;
                    case string val:
                        OSCString = val;
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
        public OSCValueTypeEnum OSCValueType { get; private set; } = OSCValueTypeEnum.Int;
        public OSCTypeEnum OSCType { get; private set; } = OSCTypeEnum.Button;
        public string UserName { get; private set; } = string.Empty;
        public string UserID { get; private set; } = string.Empty;
        public string WorldName { get; private set; } = string.Empty;
        public string WorldURL { get; private set; } = string.Empty;
        public string WorldID { get; private set; } = string.Empty;
        public string InstanceID { get; private set; } = string.Empty;
        public string InstanceType { get; private set; } = string.Empty;
        public string ReqInv {get; private set;} = string.Empty;
        public string WorldUserID { get; private set; } = string.Empty;
        public string Region { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public string URL { get; private set; } = string.Empty;

        private Dictionary<string, string> nameToProperty;
        public string CaptureProperty(string capture)
        {
            return nameToProperty[capture];
        }

        public override string ToString()
        {
            return ""
            + nameof(SettingName) + ": " + SettingName + "\n"
            + nameof(OSCAddress) + ": " + OSCAddress + "\n"
            + nameof(OSCValue) + ": " + OSCValue + "\n"
            + nameof(OSCValueType) + ": " + OSCValueType + "\n"
            + nameof(OSCType) + ": " + OSCType + "\n"
            + nameof(UserName) + ": " + UserName + "\n"
            + nameof(UserID) + ": " + UserID + "\n"
            + nameof(WorldName) + ": " + WorldName + "\n"
            + nameof(WorldURL) + ": " + WorldURL + "\n"
            + nameof(WorldID) + ": " + WorldID + "\n"
            + nameof(InstanceID) + ": " + InstanceID + "\n"
            + nameof(InstanceType) + ": " + InstanceType + "\n"
            + nameof(ReqInv) + ": " + ReqInv + "\n"
            + nameof(WorldUserID) + ": " + WorldUserID + "\n"
            + nameof(Region) + ": " + Region + "\n"
            + nameof(Message) + ": " + Message + "\n"
            + nameof(URL) + ": " + URL + "\n"
            ;
        }

        public SingleSetting(
            string settingName = "Empty",
            string oSCAddress = "/avatar/parameters/empty",
            // string oSCValue = "empty",
            bool? oSCBool = null,
            int? oSCInt = null,
            float? oSCFloat = null,
            string? oSCString = null,
            OSCValueTypeEnum oSCValueType = OSCValueTypeEnum.Int,
            OSCTypeEnum oSCType = OSCTypeEnum.Button,
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
            OSCAddress = oSCAddress;
            // OSCValue = oSCValue;
            OSCBool = oSCBool;
            OSCInt = oSCInt;
            OSCFloat = oSCFloat;
            OSCString = oSCString;
            OSCValueType = oSCValueType;
            OSCType = oSCType;
            UserName = userName;
            UserID = userID;
            WorldName = worldName;
            WorldURL = worldURL;
            WorldID = worldID;
            InstanceID = instanceID;
            InstanceType = instanceType;
            ReqInv = _reqInvDict[reqInv];
            WorldUserID = worldUserID;
            Region = region;
            Message = message;
            URL = url;

            nameToProperty = new()
            {
                {nameof(SettingName), SettingName},
                // {nameof(OSCAddress), OSCAddress},
                // {nameof(OSCValue), OSCValue},
                // {nameof(OSCValueType), OSCValueType},
                // {nameof(OSCType), OSCType},
                {nameof(UserName), UserName},
                {nameof(UserID), UserID},
                {nameof(WorldName), WorldName},
                {nameof(WorldURL), WorldURL},
                {nameof(WorldID), WorldID},
                {nameof(InstanceID), InstanceID},
                {nameof(InstanceType), InstanceType},
                {nameof(ReqInv), ReqInv},
                {nameof(WorldUserID), WorldUserID},
                {nameof(Region), Region},
                {nameof(Message), Message},
                {"DisplayName", UserName},
                {nameof(URL), URL},
            };
        }
        /// <summary>
        /// Only for Json Constructor.
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="oSCAddress"></param>
        /// <param name="oSCValue"></param>
        /// <param name="oSCValueType"></param>
        /// <param name="oSCType"></param>
        /// <param name="userName"></param>
        /// <param name="userID"></param>
        /// <param name="worldName"></param>
        /// <param name="worldURL"></param>
        /// <param name="worldID"></param>
        /// <param name="instanceID"></param>
        /// <param name="instanceType"></param>
        /// <param name="reqInv"></param>
        /// <param name="worldUserID"></param>
        /// <param name="region"></param>
        /// <param name="message"></param>
        /// <param name="url"></param>
        [JsonConstructor]
        public SingleSetting(
            string settingName = "Empty",
            string oSCAddress = "/avatar/parameters/empty",
            // string oSCValue = "empty",
            bool? oSCBool = null,
            int? oSCInt = null,
            float? oSCFloat = null,
            string? oSCString = null,
            OSCValueTypeEnum oSCValueType = OSCValueTypeEnum.Int,
            OSCTypeEnum oSCType = OSCTypeEnum.Button,
            string userName = "",
            string userID = "",
            string worldName = "",
            string worldURL = "",
            string worldID = "",
            string instanceID = "",
            string instanceType = "",
            string reqInv = "",
            string worldUserID = "",
            string region = "",
            string message = "",
            string url = ""
        )
        {
            SettingName = settingName;
            OSCAddress = oSCAddress;
            // OSCValue = oSCValue;
            OSCBool = oSCBool;
            OSCInt = oSCInt;
            OSCFloat = oSCFloat;
            OSCString = oSCString;
            OSCValueType = oSCValueType;
            OSCType = oSCType;
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

            nameToProperty = new()
            {
                {nameof(SettingName), SettingName},
                // {nameof(OSCAddress), OSCAddress},
                // {nameof(OSCValue), OSCValue},
                // {nameof(OSCValueType), OSCValueType},
                // {nameof(OSCType), OSCType},
                {nameof(UserName), UserName},
                {nameof(UserID), UserID},
                {nameof(WorldName), WorldName},
                {nameof(WorldURL), WorldURL},
                {nameof(WorldID), WorldID},
                {nameof(InstanceID), InstanceID},
                {nameof(InstanceType), InstanceType},
                {nameof(ReqInv), ReqInv},
                {nameof(WorldUserID), WorldUserID},
                {nameof(Region), Region},
                {nameof(Message), Message},
                {"DisplayName", UserName},
                {nameof(URL), URL},
            };
        }

    }
}
