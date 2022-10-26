using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        public ReactivePropertySlim<SingleSetting> EdittingSetting { get; init; } = new();
        public enum InstanceTypeEnum
        {
            Public,
            FriendPlus,
            Friend,
            InvitePlus,
            Invite,
        }
        public ReactivePropertySlim<string> SettingName { get; private set; } = new ReactivePropertySlim<string>(string.Empty);
        // [Required(ErrorMessage = "Required")]
        public ReactiveProperty<string> OSCAddress { get; private set; } = new ReactiveProperty<string>(string.Empty);
        public ReactivePropertySlim<bool?> OSCBool { get; private set; } = new ReactivePropertySlim<bool?>((bool?)null);
        public ReactivePropertySlim<int?> OSCInt { get; private set; } = new ReactivePropertySlim<int?>((int?)null);
        public ReactivePropertySlim<float?> OSCFloat { get; private set; } = new ReactivePropertySlim<float?>((float?)null);
        public ReactivePropertySlim<string?> OSCString { get; private set; } = new ReactivePropertySlim<string?>((string?)null);
        public ReactivePropertySlim<OSCValueTypeEnum> OSCValueType { get; private set; } = new ReactivePropertySlim<OSCValueTypeEnum>(OSCValueTypeEnum.Int);
        public ReactivePropertySlim<OSCTypeEnum> OSCType { get; private set; } = new ReactivePropertySlim<OSCTypeEnum>(OSCTypeEnum.Button);
        public ReactivePropertySlim<string> UserName { get; private set; } = new ReactivePropertySlim<string>(string.Empty);
        public ReactivePropertySlim<string> UserID { get; private set; } = new ReactivePropertySlim<string>(string.Empty);
        public ReactivePropertySlim<string> WorldName { get; private set; } = new ReactivePropertySlim<string>(string.Empty);
        public ReactivePropertySlim<string> WorldURL { get; private set; } = new ReactivePropertySlim<string>(string.Empty);
        public ReactivePropertySlim<string> WorldID { get; private set; } = new ReactivePropertySlim<string>(string.Empty);
        public ReactivePropertySlim<string> InstanceID { get; private set; } = new ReactivePropertySlim<string>(string.Empty);
        public ReactivePropertySlim<InstanceTypeEnum> InstanceType { get; private set; } = new ReactivePropertySlim<InstanceTypeEnum>(InstanceTypeEnum.Public);
        public ReactivePropertySlim<string> WorldUserID { get; private set; } = new ReactivePropertySlim<string>(string.Empty);
        public ReactivePropertySlim<string> Region { get; private set; } = new ReactivePropertySlim<string>(string.Empty);
        public ReactivePropertySlim<string> Message { get; private set; } = new ReactivePropertySlim<string>(string.Empty);
        public ReactivePropertySlim<string> URL { get; private set; } = new ReactivePropertySlim<string>(string.Empty);

        public ReactiveCommand OKCommand = new();
        public ReactiveCommand CancelCommand = new();
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

        public EditorWindowViewModel()
        {
            OSCAddress.SetValidateAttribute(() => OSCAddress).AddTo(_compositeDisposable);
            // OSCBool.AddTo(_compositeDisposable);
            // OSCInt.AddTo(_compositeDisposable);
            // OSCFloat.AddTo(_compositeDisposable);
            // OSCString.AddTo(_compositeDisposable);
            // OSCValueType.AddTo(_compositeDisposable);
            // OSCType.AddTo(_compositeDisposable);
            // UserName.AddTo(_compositeDisposable);
            // UserID.AddTo(_compositeDisposable);
            // WorldName.AddTo(_compositeDisposable);
            // WorldURL.AddTo(_compositeDisposable);
            // WorldID.AddTo(_compositeDisposable);
            // InstanceID.AddTo(_compositeDisposable);
            // InstanceType.AddTo(_compositeDisposable);
            // WorldUserID.AddTo(_compositeDisposable);
            // Region.AddTo(_compositeDisposable);
            // Message.AddTo(_compositeDisposable);
            // URL.AddTo(_compositeDisposable);

            EdittingSetting.Value = new SingleSetting();
            EdittingSetting.Subscribe(setting => Debug.Print(setting.ToString())).AddTo(_compositeDisposable);
            OSCBool.Subscribe(val => Debug.Print("Bool: " + val?.ToString())).AddTo(_compositeDisposable);
            OSCInt.Subscribe(val => Debug.Print("Int: " + val?.ToString())).AddTo(_compositeDisposable);
            OSCFloat.Subscribe(val => Debug.Print("Float: " + val?.ToString())).AddTo(_compositeDisposable);
            OSCString.Subscribe(val => Debug.Print("String: " + val?.ToString())).AddTo(_compositeDisposable);
            OSCAddress.Subscribe(val => Debug.Print("Address: " + val?.ToString())).AddTo(_compositeDisposable);
            Message.Subscribe(val => Debug.Print("Message: " + val?.ToString())).AddTo(_compositeDisposable);
            SettingName.Subscribe(val => Debug.Print("SettingName: " + val?.ToString())).AddTo(_compositeDisposable);
        }
    }
}
