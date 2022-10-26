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
        [Required(ErrorMessage = "Required")]
        public ReactiveProperty<string> OSCAddress { get; private set; } = new ReactiveProperty<string>(string.Empty);
        public ReactivePropertySlim<bool?> OSCBool { get; private set; } = new ReactivePropertySlim<bool?>((bool?)null);
        public ReactivePropertySlim<int?> OSCInt { get; private set; } = new ReactivePropertySlim<int?>((int?)null);
        public ReactivePropertySlim<float?> OSCFloat { get; private set; } = new ReactivePropertySlim<float?>((float?)null);
        public ReactivePropertySlim<string?> OSCString { get; private set; } = new ReactivePropertySlim<string?>((string?)null);
        public ReactivePropertySlim<SingleSetting.OSCValueTypeEnum> OSCValueType { get; private set; } = new ReactivePropertySlim<SingleSetting.OSCValueTypeEnum>(SingleSetting.OSCValueTypeEnum.Int);
        public ReactivePropertySlim<SingleSetting.OSCTypeEnum> OSCType { get; private set; } = new ReactivePropertySlim<SingleSetting.OSCTypeEnum>(SingleSetting.OSCTypeEnum.Button);
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
            EdittingSetting.Value = new SingleSetting();
            EdittingSetting.Subscribe(setting => Debug.Print(setting.ToString()));
        }
    }
}
