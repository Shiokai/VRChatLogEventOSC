using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;


namespace VRChatLogEventOSC.Control
{
    internal sealed class ControlWindowViewModel : INotifyPropertyChanged, IDisposable, IClosing
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly ControlWindowModel _model;
        private readonly ReactivePropertySlim<bool> _isPaused = new(false);
        private bool _isDirty = false;

        private readonly CompositeDisposable _compositeDisposable = new();

        public ReactiveCommand PauseCommand { get; init; }
        public ReactiveCommand RestartCommand { get; init; }
        public ReactiveCommand RescanCommand { get; init; }
        public ReactiveCommand RestartWithScanCommand { get; init; }
        public ReactiveCommand QuitApplicationCommand { get; init; }
        public ReactiveCommand FolderBrowseCommand { get; init; }
        public ReactiveCommand SaveAndLoadCommand { get; init; }

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

        public void Closing(CancelEventArgs cancelEventArgs)
        {
            if (!_isDirty)
            {
                return;
            }

            var result = System.Windows.MessageBox.Show("Configを保存しますか?", "Closing", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel)
            {
                cancelEventArgs.Cancel = true;
                return;
            }
            else if (result == MessageBoxResult.Yes)
            {
                SaveAndLoad();
                System.Windows.MessageBox.Show($"設定が適用されました\n\nLog directory: {ConfigDirectoryPath.Value}\nIP Address: {ConfigIPAdress.Value}\nPort: {ConfigPort.Value}", "Apply config", MessageBoxButton.OK);
                return;
            }
        }

        private void SaveAndLoad()
        {
            _model.SaveConfig(ConfigIPAdress.Value, ConfigPort.Value, ConfigDirectoryPath.Value);
            var config = _model.LoadConfig();
            (ConfigIPAdress.Value, ConfigPort.Value, ConfigDirectoryPath.Value) = (config.IPAddress, config.Port, config.LogFileDirectory);
            _isDirty = false;
        }

        public ControlWindowViewModel()
        {
            _model = ControlWindowModel.Instance;

            _model.IsRunning.Subscribe(running => _isPaused.Value = !running).AddTo(_compositeDisposable);

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


            ConfigIPAdress = new ReactiveProperty<string>(IPAddress.Loopback.ToString(), ReactivePropertyMode.DistinctUntilChanged)
            .SetValidateAttribute(() => ConfigIPAdress)
            .AddTo(_compositeDisposable);

            ConfigIPAdressError = ConfigIPAdress.ObserveErrorChanged
            .Select(error => error?.Cast<string>().FirstOrDefault() ?? string.Empty)
            .ToReadOnlyReactivePropertySlim<string>()
            .AddTo(_compositeDisposable);

            ConfigPort = new ReactiveProperty<int>(9000, ReactivePropertyMode.DistinctUntilChanged)
            .SetValidateAttribute(() => ConfigPort)
            .AddTo(_compositeDisposable);

            ConfigPortError = ConfigPort.ObserveErrorChanged
            .Select(error => error?.Cast<string>().FirstOrDefault() ?? string.Empty)
            .ToReadOnlyReactivePropertySlim<string>()
            .AddTo(_compositeDisposable);

            ConfigDirectoryPath = new ReactiveProperty<string>(Path.GetFullPath(ControlWindowModel.DefaultLogDirectoryPath), ReactivePropertyMode.DistinctUntilChanged)
            .SetValidateNotifyError(val => !Directory.Exists(val) ? "指定されたフォルダが見つかりません" : null)
            .AddTo(_compositeDisposable);

            ConfigDirectoryPathError = ConfigDirectoryPath.ObserveErrorChanged
            .Select(error => error?.Cast<string>().FirstOrDefault() ?? string.Empty)
            .ToReadOnlyReactivePropertySlim<string>()
            .AddTo(_compositeDisposable);

            // Config系ReactivePropertyの初期化後に記述
            var canSave = Observable.Merge(ConfigIPAdress.ObserveHasErrors.ToUnit(), ConfigPort.ObserveHasErrors.ToUnit(), ConfigDirectoryPath.ObserveHasErrors.ToUnit())
            .Select(_ => ConfigIPAdress.HasErrors || ConfigPort.HasErrors || ConfigDirectoryPath.HasErrors)
            .Inverse();

            SaveAndLoadCommand = canSave
            .ToReactiveCommand()
            .WithSubscribe(() => 
            {
                SaveAndLoad();
                System.Windows.MessageBox.Show($"設定が適用されました\n\nLog directory: {ConfigDirectoryPath.Value}\nIP Address: {ConfigIPAdress.Value}\nPort: {ConfigPort.Value}", "Apply config", MessageBoxButton.OK);
            }).AddTo(_compositeDisposable);

            // ConfigDirectoryPathの初期化より後に記述
            FolderBrowseCommand = new ReactiveCommand().WithSubscribe(() =>
            {
                using (var folderBrowserDialog = new FolderBrowserDialog())
                {
                    folderBrowserDialog.InitialDirectory = ConfigDirectoryPath.Value;
                    // ここの文字がダイアログの表示上で折り返してしまうの何とかしたい
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

            // 最初のLoadCinfigより後に行う
            // そうでなければ、読み込んだコンフィグがデフォルト値と異なる場合編集していなくてもDirtyになる
            Observable.Merge(ConfigIPAdress.ToUnit(), ConfigPort.ToUnit(), ConfigDirectoryPath.ToUnit())
            .Subscribe(_ => _isDirty = true).AddTo(_compositeDisposable);

        }

    }
}
