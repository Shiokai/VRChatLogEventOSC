using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace VRChatLogEventOSC
{
    internal class SettingWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        // public ReactiveCommand JoinedRoomURLButton { get; init; }
        // public ReactiveCommand JoinedRoomNameButton { get; init; }
        // public ReactiveCommand AcceptFriendRequestButton { get; init; }
        // public ReactiveCommand PlayedVideo1Button { get; init; }
        // public ReactiveCommand PlayedVideo2Button { get; init; }
        // public ReactiveCommand AcceptInviteButton { get; init; }
        // public ReactiveCommand AcceptRequestInviteButton { get; init; }
        // public ReactiveCommand OnPlayerJoinedButton { get; init; }
        // public ReactiveCommand OnPlayerLeftButton { get; init; }
        // public ReactiveCommand TookScreenshotButton { get; init; }

        private readonly Dictionary<Common.RegexPattern.EventTypeEnum, ReactivePropertySlim<bool>> _eventsButtonChecked = new()
        {
            {Common.RegexPattern.EventTypeEnum.JoinedRoomURL, new()},
            {Common.RegexPattern.EventTypeEnum.JoinedRoomName, new()},
            {Common.RegexPattern.EventTypeEnum.AcceptFriendRequest, new()},
            {Common.RegexPattern.EventTypeEnum.PlayedVideo1, new()},
            {Common.RegexPattern.EventTypeEnum.PlayedVideo2, new()},
            {Common.RegexPattern.EventTypeEnum.AcceptInvite, new()},
            {Common.RegexPattern.EventTypeEnum.AcceptRequestInvite, new()},
            {Common.RegexPattern.EventTypeEnum.OnPlayerJoined, new()},
            {Common.RegexPattern.EventTypeEnum.OnPlayerLeft, new()},
            {Common.RegexPattern.EventTypeEnum.TookScreenshot, new()},
        };
        public IReadOnlyDictionary<Common.RegexPattern.EventTypeEnum, ReactivePropertySlim<bool>> EventsButtonChecked { get; init; }
    

        // public ReactivePropertySlim<bool> IsJoinedRoomURLButtonChecked { get; init; }
        // public ReactivePropertySlim<bool> IsJoinedRoomNameButtonChecked { get; init; }
        // public ReactivePropertySlim<bool> IsAcceptFriendRequestButtonChecked { get; init; }
        // public ReactivePropertySlim<bool> IsPlayedVideo1ButtonChecked { get; init; }
        // public ReactivePropertySlim<bool> IsPlayedVideo2ButtonChecked { get; init; }
        // public ReactivePropertySlim<bool> IsAcceptInviteButtonChecked { get; init; }
        // public ReactivePropertySlim<bool> IsAcceptRequestInviteButtonChecked { get; init; }
        // public ReactivePropertySlim<bool> IsOnPlayerJoinedButtonChecked { get; init; }
        // public ReactivePropertySlim<bool> IsOnPlayerLeftButtonChecked { get; init; }
        // public ReactivePropertySlim<bool> IsTookScreenshotButtonChecked { get; init; }

        private readonly CompositeDisposable _conpositeDisposable = new();
        private bool _disposed = false;
        public void Dispose()
        {
            _conpositeDisposable.Dispose();
        }

        public SettingWindowViewModel()
        {
            // JoinedRoomURLButton = new ReactiveCommand().WithSubscribe(() => { }).AddTo(_conpositeDisposable);
            // JoinedRoomNameButton = new ReactiveCommand().WithSubscribe(() => { }).AddTo(_conpositeDisposable);
            // AcceptFriendRequestButton = new ReactiveCommand().WithSubscribe(() => { }).AddTo(_conpositeDisposable);
            // PlayedVideo1Button = new ReactiveCommand().WithSubscribe(() => { }).AddTo(_conpositeDisposable);
            // PlayedVideo2Button = new ReactiveCommand().WithSubscribe(() => { }).AddTo(_conpositeDisposable);
            // AcceptInviteButton = new ReactiveCommand().WithSubscribe(() => { }).AddTo(_conpositeDisposable);
            // AcceptRequestInviteButton = new ReactiveCommand().WithSubscribe(() => { }).AddTo(_conpositeDisposable);
            // OnPlayerJoinedButton = new ReactiveCommand().WithSubscribe(() => { }).AddTo(_conpositeDisposable);
            // OnPlayerLeftButton = new ReactiveCommand().WithSubscribe(() => { }).AddTo(_conpositeDisposable);
            // TookScreenshotButton = new ReactiveCommand().WithSubscribe(() => { }).AddTo(_conpositeDisposable);

            // IsJoinedRoomURLButtonChecked = new ReactivePropertySlim<bool>().AddTo(_conpositeDisposable);
            // IsJoinedRoomNameButtonChecked = new ReactivePropertySlim<bool>().AddTo(_conpositeDisposable);
            // IsAcceptFriendRequestButtonChecked = new ReactivePropertySlim<bool>().AddTo(_conpositeDisposable);
            // IsPlayedVideo1ButtonChecked = new ReactivePropertySlim<bool>().AddTo(_conpositeDisposable);
            // IsPlayedVideo2ButtonChecked = new ReactivePropertySlim<bool>().AddTo(_conpositeDisposable);
            // IsAcceptInviteButtonChecked = new ReactivePropertySlim<bool>().AddTo(_conpositeDisposable);
            // IsAcceptRequestInviteButtonChecked = new ReactivePropertySlim<bool>().AddTo(_conpositeDisposable);
            // IsOnPlayerJoinedButtonChecked = new ReactivePropertySlim<bool>().AddTo(_conpositeDisposable);
            // IsOnPlayerLeftButtonChecked = new ReactivePropertySlim<bool>().AddTo(_conpositeDisposable);
            // IsTookScreenshotButtonChecked = new ReactivePropertySlim<bool>().AddTo(_conpositeDisposable);

            // IsJoinedRoomURLButtonChecked.Where(isChecked => isChecked).Subscribe(_ => { });
            // IsJoinedRoomNameButtonChecked.Where(isChecked => isChecked).Subscribe(_ => { });
            // IsAcceptFriendRequestButtonChecked.Where(isChecked => isChecked).Subscribe(_ => { });
            // IsPlayedVideo1ButtonChecked.Where(isChecked => isChecked).Subscribe(_ => { });
            // IsPlayedVideo2ButtonChecked.Where(isChecked => isChecked).Subscribe(_ => { });
            // IsAcceptInviteButtonChecked.Where(isChecked => isChecked).Subscribe(_ => { });
            // IsAcceptRequestInviteButtonChecked.Where(isChecked => isChecked).Subscribe(_ => { });
            // IsOnPlayerJoinedButtonChecked.Where(isChecked => isChecked).Subscribe(_ => { });
            // IsOnPlayerLeftButtonChecked.Where(isChecked => isChecked).Subscribe(_ => { });
            // IsTookScreenshotButtonChecked.Where(isChecked => isChecked).Subscribe(_ => { });

            EventsButtonChecked = _eventsButtonChecked;

            foreach (var type in Enum.GetValues<Common.RegexPattern.EventTypeEnum>())
            {
                foreach (var isButtonChecked in _eventsButtonChecked.Values)
                {
                    isButtonChecked.AddTo(_conpositeDisposable);
                    isButtonChecked.Where(c => c).SubscribeOnUIDispatcher().Subscribe(_ => 
                    {
                        foreach (var buttonChecked in _eventsButtonChecked.Values)
                        {
                            if (buttonChecked == isButtonChecked)
                            {
                                continue;
                            }

                            buttonChecked.Value = false;
                        }
                    }).AddTo(_conpositeDisposable);
                }
            }
        }
    }
}
