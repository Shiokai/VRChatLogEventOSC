using System;
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

using System.Diagnostics;

using VRChatLogEventOSC.Common;
using static VRChatLogEventOSC.Common.SingleSetting;

namespace VRChatLogEventOSC
{
    internal class EditorWindowViewModel : IDisposable, INotifyPropertyChanged
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

        public ReactiveCommand<EditorWindow> OKCommand { get; init; }
        public ReactiveCommand<EditorWindow> CancelCommand { get; init; }
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

        private string InstanceTypeTpStr()
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

        private ReqInvEnum InstanceTypeToReqInv()
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
                w.DialogResult = true;
                var setting = new SingleSetting(
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
                    instanceType: InstanceTypeTpStr(),
                    reqInv: InstanceTypeToReqInv(),
                    worldUserID: WorldUserID.Value,
                    region: RegionToStr(),
                    message: Message.Value,
                    url: URL.Value
                );
                _model.AddSetting(eventType, setting);
            }).AddTo(_compositeDisposable);

            CancelCommand = new ReactiveCommand<EditorWindow>().WithSubscribe(w => w.DialogResult = false).AddTo(_compositeDisposable);

            // Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ => Debug.WriteLine(OSCFloat)).AddTo(_compositeDisposable);
        }
    }
}
