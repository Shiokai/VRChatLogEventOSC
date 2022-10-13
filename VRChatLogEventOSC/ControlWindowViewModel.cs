using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Net;
using System.Windows.Forms;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using System.Diagnostics;

namespace VRChatLogEventOSC
{
    internal sealed class ControlWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly ControlWindowModel _model;
        private readonly ReactivePropertySlim<bool> _isPaused = new(false);
        public ReactiveCommand PauseCommand { get; init; }
        public ReactiveCommand RestartCommand { get; init; }
        public ReactiveCommand RescanCommand { get; init; }
        public ReactiveCommand RestartWithScanCommand { get; init; }

        public ReactiveCommand QuitApplicationCommand { get; init; }

        public ReactiveCommand FolderBrowseCommand { get; init; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\.){3}(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])$", ErrorMessage = "不正なIPアドレスです")]
        public ReactiveProperty<string> ConfigIPAdress { get; init; }
        public ReadOnlyReactivePropertySlim<string> ConfigIPAdressError { get; init; }
        [Required(ErrorMessage = "Required")]
        [Range(0, 65535, ErrorMessage = "ポート番号の範囲は0~65535です")]
        public ReactiveProperty<int> ConfigPort { get; init; }
        public ReadOnlyReactivePropertySlim<string> ConfigPortError { get; init; }
        [Required(ErrorMessage = "Required")]
        public ReactiveProperty<string> ConfigDirectoryPath { get; init; }
        public ReadOnlyReactivePropertySlim<string> ConfigDirectoryPathError { get; init; }

        public ReactiveCommand SaveAndLoadCommand { get; init; }

        private readonly CompositeDisposable _compositeDisposable = new();

        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _compositeDisposable.Dispose();
            _disposed = true;
        }

        public ControlWindowViewModel()
        {
            _model = ControlWindowModel.Instance;

            PauseCommand = _isPaused.Inverse().ToReactiveCommand().WithSubscribe(() =>
            {
                _model.PuaseLogWEvent();
                _isPaused.Value = true;
            }).AddTo(_compositeDisposable);

            RestartCommand = _isPaused.ToReactiveCommand().WithSubscribe(() =>
            {
                _model.RestartLogEvent();
                _isPaused.Value = false;
            }).AddTo(_compositeDisposable);

            RescanCommand = _isPaused.ToReactiveCommand().WithSubscribe(() => 
            {
                _model.Rescan();
                _isPaused.Value = false;
            }).AddTo(_compositeDisposable);

            RestartWithScanCommand = _isPaused.ToReactiveCommand().WithSubscribe(() =>
            {
                _model.RestartLogEventWithScan();
                _isPaused.Value = false;
            }).AddTo(_compositeDisposable);

            QuitApplicationCommand = new ReactiveCommand().WithSubscribe(() => ControlWindowModel.QuitApplication()).AddTo(_compositeDisposable);


            ConfigIPAdress = new ReactiveProperty<string>(IPAddress.Loopback.ToString())
            .SetValidateAttribute(() => ConfigIPAdress)
            .AddTo(_compositeDisposable);

            ConfigIPAdressError = ConfigIPAdress.ObserveErrorChanged
            .Select(error => error?.Cast<string>().FirstOrDefault() ?? string.Empty)
            .ToReadOnlyReactivePropertySlim<string>()
            .AddTo(_compositeDisposable);

            ConfigPort = new ReactiveProperty<int>(9000)
            .SetValidateAttribute(() => ConfigPort)
            .AddTo(_compositeDisposable);

            ConfigPortError = ConfigPort.ObserveErrorChanged
            .Select(error => error?.Cast<string>().FirstOrDefault() ?? string.Empty)
            .ToReadOnlyReactivePropertySlim<string>()
            .AddTo(_compositeDisposable);

            ConfigDirectoryPath = new ReactiveProperty<string>(_model.DefaultLogDirectoryPath)
            .SetValidateNotifyError(val => !System.IO.Directory.Exists(val) ? "指定されたフォルダが見つかりません" : null)
            .AddTo(_compositeDisposable);

            ConfigDirectoryPathError = ConfigDirectoryPath.ObserveErrorChanged
            .Select(error => error?.Cast<string>().FirstOrDefault() ?? string.Empty)
            .ToReadOnlyReactivePropertySlim<string>()
            .AddTo(_compositeDisposable);

            var canSave = Observable.Merge(ConfigIPAdress.ObserveHasErrors.ToUnit(), ConfigPort.ObserveHasErrors.ToUnit(), ConfigDirectoryPath.ObserveHasErrors.ToUnit())
            .Select(_ => ConfigIPAdress.HasErrors || ConfigPort.HasErrors || ConfigDirectoryPath.HasErrors)
            .Inverse();

            SaveAndLoadCommand = canSave
            .ToReactiveCommand()
            .WithSubscribe(() => 
            {
                _model.SaveConfig(ConfigIPAdress.Value, ConfigPort.Value, ConfigDirectoryPath.Value);
                var config = _model.LoadConfig();
                (ConfigIPAdress.Value, ConfigPort.Value, ConfigDirectoryPath.Value) = (config.IPAddress, config.Port, config.LogFileDirectory);
            }).AddTo(_compositeDisposable);

            FolderBrowseCommand = new ReactiveCommand().WithSubscribe(() =>
            {
                using (var folderBrowserDialog = new FolderBrowserDialog())
                {
                    folderBrowserDialog.InitialDirectory = _model.DefaultLogDirectoryPath;
                    folderBrowserDialog.Description = "VRChat outpulog directory";
                    var result = folderBrowserDialog.ShowDialog();
                    if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                    ConfigDirectoryPath.Value = folderBrowserDialog.SelectedPath;
                }
            }).AddTo(_compositeDisposable);

            var config = _model.LoadConfig();
            (ConfigIPAdress.Value, ConfigPort.Value, ConfigDirectoryPath.Value) = (config.IPAddress, config.Port, config.LogFileDirectory);

        }

    }
}
