using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace VRChatLogEventOSC
{
    internal class ControlWindowViewModel
    {
        private ControlWindowModel _model;
        private ReactivePropertySlim<bool> _isPaused = new(false);
        public ReactiveCommand PauseCommand { get; init; }
        public ReactiveCommand RestartCommand { get; init; }
        public ReactiveCommand RescanCommand { get; init; }
        public ReactiveCommand RestartWithScanCommand { get; init; }

        public ReactiveCommand QuitApplicationCommand { get; init; }
        private CompositeDisposable _compositeDisposable = new();

        public ControlWindowViewModel()
        {
            _model = ControlWindowModel.Instance;

            PauseCommand = _isPaused.Select(p => !p).ToReactiveCommand().WithSubscribe(() =>
            {
                _model.PuaseLogWEvent();
                _isPaused.Value = true;
            }).AddTo(_compositeDisposable);

            RestartCommand = _isPaused.ToReactiveCommand().WithSubscribe(() =>
            {
                _model.RestartLogEvent();
                _isPaused.Value = false;
            }).AddTo(_compositeDisposable);

            RescanCommand = new ReactiveCommand().WithSubscribe(() => _model.Rescan()).AddTo(_compositeDisposable);

            RestartWithScanCommand = _isPaused.ToReactiveCommand().WithSubscribe(() =>
            {
                _model.RestartLogEventWithScan();
                _isPaused.Value = false;
            }).AddTo(_compositeDisposable);

            QuitApplicationCommand = new ReactiveCommand().WithSubscribe(() => _model.QuitApplication()).AddTo(_compositeDisposable);
        }

    }
}
