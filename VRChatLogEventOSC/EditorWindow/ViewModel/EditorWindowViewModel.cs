﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Reactive.Disposables;
using Reactive.Bindings.Extensions;
using Reactive.Bindings;
using System.Reactive.Linq;

using VRChatLogEventOSC.Common;
using static VRChatLogEventOSC.Common.SingleSetting;

namespace VRChatLogEventOSC.Editor
{
    internal class EditorWindowViewModel : IDisposable, INotifyPropertyChanged, IClosing
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly EditorWindowModel _model = EditorWindowModel.Instance;
        public enum InstanceTypeEnum
        {
            None,
            Public,
            FriendsPlus,
            Friends,
            InvitePlus,
            Invite,
        }

        public enum RegionEnum
        {
            None,
            US,
            USE,
            EU,
            JP,
        }

        private readonly Dictionary<string, OSCValueTypeEnum> _comboOSCValueType = new();
        public IReadOnlyDictionary<string, OSCValueTypeEnum> ComboOSCValueType => _comboOSCValueType;
        private readonly Dictionary<string, OSCTypeEnum> _comboOSCType = new();
        public IReadOnlyDictionary<string, OSCTypeEnum> ComboOSCType => _comboOSCType;
        private readonly Dictionary<string, InstanceTypeEnum> _comboInstanceType = new();
        public IReadOnlyDictionary<string, InstanceTypeEnum> ComboInstanceType => _comboInstanceType;
        private readonly Dictionary<string, RegionEnum> _comboRegion = new();
        public IReadOnlyDictionary<string, RegionEnum> ComboRegion => _comboRegion;

        public ReactivePropertySlim<Visibility> OSCBoolVisibility { get; init; }
        public ReactivePropertySlim<Visibility> OSCIntVisibility { get; init; }
        public ReactivePropertySlim<Visibility> OSCFloatVisibility { get; init; }
        public ReactivePropertySlim<Visibility> OSCStringVisibility { get; init; }

        private readonly ReactivePropertySlim<string> _eventTypeText = new(string.Empty);
        public ReadOnlyReactivePropertySlim<string> EventTypeText { get; init; }


        public ReactivePropertySlim<string> SettingName { get; private set; }
        [Required(ErrorMessage = "Required")]
        public ReactiveProperty<string> OSCAddress { get; private set; }
        public ReactivePropertySlim<bool?> OSCBool { get; private set; }
        [Range(0, 255)]
        public ReactiveProperty<int?> OSCInt { get; private set; }
        private float? OSCFloat { get; set; } = null;
        public ReactiveProperty<string?> OSCFloatAsStr { get; private set; }
        public ReactivePropertySlim<string?> OSCString { get; private set; }
        public ReactivePropertySlim<OSCValueTypeEnum> OSCValueType { get; private set; }
        public ReactivePropertySlim<OSCTypeEnum> OSCType { get; private set; }
        public ReactivePropertySlim<string> UserName { get; private set; }
        public ReactivePropertySlim<string> UserID { get; private set; }
        public ReactivePropertySlim<string> WorldName { get; private set; }
        public ReactivePropertySlim<string> WorldURL { get; private set; }
        public ReactivePropertySlim<string> WorldID { get; private set; }
        public ReactivePropertySlim<string> InstanceID { get; private set; }
        public ReactivePropertySlim<InstanceTypeEnum> InstanceType { get; private set; }
        public ReactivePropertySlim<string> WorldUserID { get; private set; }
        public ReactivePropertySlim<RegionEnum> Region { get; private set; }
        public ReactivePropertySlim<string> Message { get; private set; }
        public ReactivePropertySlim<string> URL { get; private set; }

        private ReactivePropertySlim<bool> _userNameEditable = new();
        public ReadOnlyReactivePropertySlim<bool> UserNameEditable { get; init; }
        private ReactivePropertySlim<bool> _userIdEditable = new();
        public ReadOnlyReactivePropertySlim<bool> UserIDEditable { get; init; }
        private ReactivePropertySlim<bool> _worldNameEditable = new();
        public ReadOnlyReactivePropertySlim<bool> WorldNameEditable { get; init; }
        private ReactivePropertySlim<bool> _worldUrlEditable = new();
        public ReadOnlyReactivePropertySlim<bool> WorldURLEditable { get; init; }
        private ReactivePropertySlim<bool> _worldIdEditable = new();
        public ReadOnlyReactivePropertySlim<bool> WorldIDEditable { get; init; }
        private ReactivePropertySlim<bool> _instanceIdEditable = new();
        public ReadOnlyReactivePropertySlim<bool> InstanceIDEditable { get; init; }
        private ReactivePropertySlim<bool> _instanceTypeEditable = new();
        public ReadOnlyReactivePropertySlim<bool> InstanceTypeEditable { get; init; }
        private ReactivePropertySlim<bool> _worldUserIdEditable = new();
        public ReadOnlyReactivePropertySlim<bool> WorldUserIDEditable { get; init; }
        private ReactivePropertySlim<bool> _regionEditable = new();
        public ReadOnlyReactivePropertySlim<bool> RegionEditable { get; init; }
        private ReactivePropertySlim<bool> _messageEditable = new();
        public ReadOnlyReactivePropertySlim<bool> MessageEditable { get; init; }
        private ReactivePropertySlim<bool> _urlEditable = new();
        public ReadOnlyReactivePropertySlim<bool> URLEditable { get; init; }

        public ReactiveCommand<EditorWindow> OKCommand { get; init; }
        public ReactiveCommand<EditorWindow> CancelCommand { get; init; }
        private bool _isPressedX = true;
        private CompositeDisposable _compositeDisposable = new();
        private bool _disposed = false;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _compositeDisposable.Dispose();
        }

        private SingleSetting ToSingleSetting()
        {
            return new SingleSetting(
                    settingName: SettingName.Value,
                    oscAddress: OSCAddress.Value,
                    oscValue: OSCValueType.Value switch
                    {
                        OSCValueTypeEnum.Bool => OSCBool.Value,
                        OSCValueTypeEnum.Int => OSCInt.Value,
                        OSCValueTypeEnum.Float => OSCFloat,
                        OSCValueTypeEnum.String => OSCString,
                        _ => null
                    },
                    oscType: OSCType.Value,
                    userName: UserName.Value,
                    userID: UserID.Value,
                    worldName: WorldName.Value,
                    worldURL: WorldURL.Value,
                    worldID: WorldID.Value,
                    instanceID: InstanceID.Value,
                    instanceType: InstanceTypeEnumToStr(),
                    reqInv: InstanceTypeEnumToReqInv(),
                    worldUserID: WorldUserID.Value,
                    region: RegionToStr(),
                    message: Message.Value,
                    url: URL.Value
                );
        }

        public void Closing(CancelEventArgs cancelEventArgs)
        {
            if (!_isPressedX)
            {
                return;
            }

            var result = MessageBox.Show("編集内容を適用しますか?", "Closing", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel)
            {
                cancelEventArgs.Cancel = true;
                return;
            }
            else if (result == MessageBoxResult.Yes)
            {
                var setting = ToSingleSetting();
                _model.ApplyEdited(setting);
                return;
            }
        }

        private void EventPropertyEditable(RegexPattern.EventTypeEnum eventType)
        {   
            (
                _userNameEditable.Value,
                _userIdEditable.Value,
                _worldNameEditable.Value,
                _worldUrlEditable.Value,
                _worldIdEditable.Value,
                _instanceIdEditable.Value,
                _instanceTypeEditable.Value,
                _worldUserIdEditable.Value,
                _regionEditable.Value,
                _messageEditable.Value,
                _urlEditable.Value
            ) = eventType switch
            {
                RegexPattern.EventTypeEnum.None => (
                    userName: false,
                    userId: false,
                    worldName: false,
                    worldUrl: false,
                    worldId: false,
                    instanceId: false,
                    instanceType: false,
                    worldUserId: false,
                    region: false,
                    message: false,
                    url: false
                ),
                RegexPattern.EventTypeEnum.JoinedRoomURL => (
                    userName: false,
                    userId: false,
                    worldName: false,
                    worldUrl: true,
                    worldId: true,
                    instanceId: true,
                    instanceType: true,
                    worldUserId: true,
                    region: true,
                    message: false,
                    url: false
                ),
                RegexPattern.EventTypeEnum.JoinedRoomName => (
                    userName: false,
                    userId: false,
                    worldName: true,
                    worldUrl: false,
                    worldId: false,
                    instanceId: false,
                    instanceType: false,
                    worldUserId: false,
                    region: false,
                    message: false,
                    url: false
                ),
                RegexPattern.EventTypeEnum.AcceptFriendRequest => (
                    userName: true,
                    userId: true,
                    worldName: false,
                    worldUrl: false,
                    worldId: false,
                    instanceId: false,
                    instanceType: false,
                    worldUserId: false,
                    region: false,
                    message: false,
                    url: false
                ),
                RegexPattern.EventTypeEnum.PlayedVideo1 => (
                    userName: false,
                    userId: false,
                    worldName: false,
                    worldUrl: false,
                    worldId: false,
                    instanceId: false,
                    instanceType: false,
                    worldUserId: false,
                    region: false,
                    message: false,
                    url: true
                ),
                RegexPattern.EventTypeEnum.PlayedVideo2 => (
                    userName: false,
                    userId: false,
                    worldName: false,
                    worldUrl: false,
                    worldId: false,
                    instanceId: false,
                    instanceType: false,
                    worldUserId: false,
                    region: false,
                    message: false,
                    url: true
                ),
                RegexPattern.EventTypeEnum.AcceptInvite => (
                    userName: true,
                    userId: true,
                    worldName: true,
                    worldUrl: true,
                    worldId: true,
                    instanceId: true,
                    instanceType: true,
                    worldUserId: true,
                    region: true,
                    message: true,
                    url: false
                ),
                RegexPattern.EventTypeEnum.AcceptRequestInvite => (
                    userName: true,
                    userId: true,
                    worldName: false,
                    worldUrl: false,
                    worldId: false,
                    instanceId: false,
                    instanceType: false,
                    worldUserId: false,
                    region: false,
                    message: true,
                    url: false
                ),
                RegexPattern.EventTypeEnum.OnPlayerJoined => (
                    userName: true,
                    userId: false,
                    worldName: false,
                    worldUrl: false,
                    worldId: false,
                    instanceId: false,
                    instanceType: false,
                    worldUserId: false,
                    region: false,
                    message: false,
                    url: false
                ),
                RegexPattern.EventTypeEnum.OnPlayerLeft => (
                    userName: true,
                    userId: false,
                    worldName: false,
                    worldUrl: false,
                    worldId: false,
                    instanceId: false,
                    instanceType: false,
                    worldUserId: false,
                    region: false,
                    message: false,
                    url: false
                ),
                RegexPattern.EventTypeEnum.TookScreenshot => (
                    userName: false,
                    userId: false,
                    worldName: false,
                    worldUrl: false,
                    worldId: false,
                    instanceId: false,
                    instanceType: false,
                    worldUserId: false,
                    region: false,
                    message: false,
                    url: false
                ),
                _ => (
                    userName: false,
                    userId: false,
                    worldName: false,
                    worldUrl: false,
                    worldId: false,
                    instanceId: false,
                    instanceType: false,
                    worldUserId: false,
                    region: false,
                    message: false,
                    url: false
                ),
            };
        }
        private void LoadOldSetting()
        {
            var oldSetting = _model.OldSetting;
            if (oldSetting == null)
            {
                return;
            }

            SettingName.Value = oldSetting.SettingName;
            OSCAddress.Value = oldSetting.OSCAddress;
            OSCBool.Value = oldSetting.OSCBool;
            OSCInt.Value = oldSetting.OSCInt;
            OSCFloat = oldSetting.OSCFloat;
            OSCString.Value = oldSetting.OSCString;
            OSCValueType.Value = oldSetting.OSCValueType;
            OSCType.Value = oldSetting.OSCType;
            UserName.Value = oldSetting.UserName;
            UserID.Value = oldSetting.UserID;
            WorldName.Value = oldSetting.WorldName;
            WorldURL.Value = oldSetting.WorldURL;
            WorldID.Value = oldSetting.WorldID;
            InstanceID.Value = oldSetting.InstanceID;
            InstanceType.Value = SettingToInstanceTypeEnum(oldSetting.InstanceType, oldSetting.ReqInv);
            WorldUserID.Value = oldSetting.WorldUserID;
            Region.Value = SettingToRegionEnum(oldSetting.Region);
            Message.Value = oldSetting.Message;
            URL.Value = oldSetting.URL;

        }


        private string InstanceTypeEnumToStr()
        {
            return InstanceType.Value switch
            {
                InstanceTypeEnum.Public => "public",
                InstanceTypeEnum.FriendsPlus => "hidden",
                InstanceTypeEnum.Friends => "friends",
                InstanceTypeEnum.InvitePlus => "private",
                InstanceTypeEnum.Invite => "private",
                _ => string.Empty,
            };
        }

        private static InstanceTypeEnum SettingToInstanceTypeEnum(string instanceType, ReqInvEnum reqInv)
        {
            return (instanceType, reqInv) switch
            {
                ("public", _) => InstanceTypeEnum.Public,
                ("hidden", _) => InstanceTypeEnum.FriendsPlus,
                ("friends", _) => InstanceTypeEnum.Friends,
                ("private", ReqInvEnum.CanRequestInvite) => InstanceTypeEnum.InvitePlus,
                ("private", ReqInvEnum.None) => InstanceTypeEnum.Invite,
                (_, _) => InstanceTypeEnum.None,
            };
        }

        private static RegionEnum SettingToRegionEnum(string region)
        {
            return region switch
            {
                "us" => RegionEnum.US,
                "use" => RegionEnum.USE,
                "eu" => RegionEnum.EU,
                "jp" => RegionEnum.JP,
                _ => RegionEnum.None,
            };
        }

        private ReqInvEnum InstanceTypeEnumToReqInv()
        {
            return InstanceType.Value switch
            {
                InstanceTypeEnum.InvitePlus => ReqInvEnum.CanRequestInvite,
                InstanceTypeEnum.None => ReqInvEnum.NotSpecified,
                _ => ReqInvEnum.None,
            };
        }

        private string RegionToStr()
        {
            return Region.Value switch
            {
                RegionEnum.None => string.Empty,
                _ => Region.Value.ToString().ToLower(),
            };
        }

        public EditorWindowViewModel()
        {
            var eventType = _model.EventType;
            EventTypeText = _eventTypeText.ToReadOnlyReactivePropertySlim<string>();
            _eventTypeText.Value = eventType.ToString();

            foreach (var oscvtype in Enum.GetValues<OSCValueTypeEnum>())
            {
                _comboOSCValueType.Add(oscvtype.ToString(), oscvtype);
            }

            foreach (var osctype in Enum.GetValues<OSCTypeEnum>())
            {
                _comboOSCType.Add(osctype.ToString(), osctype);
            }

            foreach (var itype in Enum.GetValues<InstanceTypeEnum>())
            {
                _comboInstanceType.Add(itype.ToString().Replace("Plus", "+"), itype);
            }

            foreach (var region in Enum.GetValues<RegionEnum>())
            {
                _comboRegion.Add(region.ToString(), region);
            }

            UserNameEditable = _userNameEditable.ToReadOnlyReactivePropertySlim<bool>();
            UserIDEditable = _userIdEditable.ToReadOnlyReactivePropertySlim<bool>();
            WorldNameEditable = _worldNameEditable.ToReadOnlyReactivePropertySlim<bool>();
            WorldURLEditable = _worldUrlEditable.ToReadOnlyReactivePropertySlim<bool>();
            WorldIDEditable = _worldIdEditable.ToReadOnlyReactivePropertySlim<bool>();
            InstanceIDEditable = _instanceIdEditable.ToReadOnlyReactivePropertySlim<bool>();
            InstanceTypeEditable = _instanceTypeEditable.ToReadOnlyReactivePropertySlim<bool>();
            WorldUserIDEditable = _worldUserIdEditable.ToReadOnlyReactivePropertySlim<bool>();
            RegionEditable = _regionEditable.ToReadOnlyReactivePropertySlim<bool>();
            MessageEditable = _messageEditable.ToReadOnlyReactivePropertySlim<bool>();
            URLEditable = _urlEditable.ToReadOnlyReactivePropertySlim<bool>();

            EventPropertyEditable(eventType);

            OSCBoolVisibility = new ReactivePropertySlim<Visibility>();
            OSCIntVisibility = new ReactivePropertySlim<Visibility>();
            OSCFloatVisibility = new ReactivePropertySlim<Visibility>();
            OSCStringVisibility = new ReactivePropertySlim<Visibility>();

            SettingName = new ReactivePropertySlim<string>(string.Empty).AddTo(_compositeDisposable);

            OSCAddress = new ReactiveProperty<string>(string.Empty)
            .SetValidateAttribute(() => OSCAddress)
            .AddTo(_compositeDisposable);
            OSCAddress.Value = $"/avatar/parameters/{eventType}";

            OSCBool = new ReactivePropertySlim<bool?>((bool?)null).AddTo(_compositeDisposable);

            OSCInt = new ReactiveProperty<int?>((int?)null)
            .SetValidateAttribute(() => OSCInt).AddTo(_compositeDisposable);

            OSCFloatAsStr = this.ToReactivePropertyAsSynchronized(
                t => t.OSCFloat, f => f?.ToString(),
                s => float.TryParse(s, out var f) ? f : null,
                ignoreValidationErrorValue: true)
                .SetValidateNotifyError(s =>
                {
                    if (string.IsNullOrWhiteSpace(s))
                    {
                        return null;
                    }

                    if (float.TryParse(s, out var f))
                    {
                        if (f < -1.0f || f > 1.0f)
                        {
                            return "Invalid";
                        }

                        return null;
                    }

                    return "Invalid";
                }).AddTo(_compositeDisposable);

            OSCString = new ReactivePropertySlim<string?>((string?)null).AddTo(_compositeDisposable);
            OSCValueType = new ReactivePropertySlim<OSCValueTypeEnum>(OSCValueTypeEnum.Bool).AddTo(_compositeDisposable);
            OSCType = new ReactivePropertySlim<OSCTypeEnum>(OSCTypeEnum.Button).AddTo(_compositeDisposable);
            UserName = new ReactivePropertySlim<string>(string.Empty).AddTo(_compositeDisposable);
            UserID = new ReactivePropertySlim<string>(string.Empty).AddTo(_compositeDisposable);
            WorldName = new ReactivePropertySlim<string>(string.Empty).AddTo(_compositeDisposable);
            WorldURL = new ReactivePropertySlim<string>(string.Empty).AddTo(_compositeDisposable);
            WorldID = new ReactivePropertySlim<string>(string.Empty).AddTo(_compositeDisposable);
            InstanceID = new ReactivePropertySlim<string>(string.Empty).AddTo(_compositeDisposable);
            InstanceType = new ReactivePropertySlim<InstanceTypeEnum>(InstanceTypeEnum.None).AddTo(_compositeDisposable);
            WorldUserID = new ReactivePropertySlim<string>(string.Empty).AddTo(_compositeDisposable);
            Region = new ReactivePropertySlim<RegionEnum>(RegionEnum.None).AddTo(_compositeDisposable);
            Message = new ReactivePropertySlim<string>(string.Empty).AddTo(_compositeDisposable);
            URL = new ReactivePropertySlim<string>(string.Empty).AddTo(_compositeDisposable);

            LoadOldSetting();

            OSCValueType.Subscribe(vtype =>
            {
                (OSCBoolVisibility.Value, OSCIntVisibility.Value, OSCFloatVisibility.Value, OSCStringVisibility.Value) = vtype switch
                {
                    OSCValueTypeEnum.Bool => (Visibility.Visible, Visibility.Hidden, Visibility.Hidden, Visibility.Hidden),
                    OSCValueTypeEnum.Int => (Visibility.Hidden, Visibility.Visible, Visibility.Hidden, Visibility.Hidden),
                    OSCValueTypeEnum.Float => (Visibility.Hidden, Visibility.Hidden, Visibility.Visible, Visibility.Hidden),
                    OSCValueTypeEnum.String => (Visibility.Hidden, Visibility.Hidden, Visibility.Hidden, Visibility.Visible),
                    _ => (Visibility.Hidden, Visibility.Hidden, Visibility.Hidden, Visibility.Hidden),
                };
            }).AddTo(_compositeDisposable);

            OKCommand = Observable.Merge(OSCAddress.ObserveHasErrors.ToUnit(), OSCInt.ObserveHasErrors.ToUnit(), OSCFloatAsStr.ObserveHasErrors.ToUnit())
            .Select(_ => OSCAddress.HasErrors || OSCInt.HasErrors || OSCFloatAsStr.HasErrors)
            .Inverse()
            .ToReactiveCommand<EditorWindow>()
            .WithSubscribe(w =>
            {
                _isPressedX = false;
                w.DialogResult = true;
                var setting = ToSingleSetting();
                _model.ApplyEdited(setting);
            }).AddTo(_compositeDisposable);

            CancelCommand = new ReactiveCommand<EditorWindow>().WithSubscribe(w =>
            {
                var result = MessageBox.Show("編集中の内容を破棄しますか?", "Cancel", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _isPressedX = false;
                    w.DialogResult = false;
                }
            }).AddTo(_compositeDisposable);

        }
    }
}
