using System;
using System.ComponentModel;
using Reactive.Bindings;
using System.Reactive.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Rug.Osc;

namespace VRChatLogEventOSC
{
    public sealed class OSCSender : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private static readonly IPAddress DefaultIPAddress = IPAddress.Loopback;
        private static readonly int DefaultPort = 9000;
        private OscSender _sender;
        private float _buttomInterval = 0.3f;
        private Task _buttomIntervalTask;
        private CancellationTokenSource _cancellationTokenSource = new();

        private IObservable<long> _buttonIntervalObservable;

        public float ButtomInterval
        {
            get => _buttomInterval;
            set
            {
                _buttomInterval = value;
                _buttomIntervalTask = Task.Delay(TimeSpan.FromSeconds(value), _cancellationTokenSource.Token);
            }
        }

        // private ReactiveProperty<OscMessage> oscMessage;
        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _sender.Close();
            _cancellationTokenSource.Dispose();
            ((IDisposable)_buttonIntervalObservable).Dispose();
            _disposed = true;
        }
        public OSCSender()
        {
            _sender = CreateNewClient(DefaultIPAddress.ToString(), DefaultPort);
            _buttomIntervalTask = Task.Delay(+TimeSpan.FromSeconds(_buttomInterval), _cancellationTokenSource.Token);
            _buttonIntervalObservable = Observable.Timer(TimeSpan.FromSeconds(ButtomInterval));
            _sender.Connect();
        }
        public OSCSender(string address)
        {
            _sender = CreateNewClient(address, DefaultPort);
            _buttomIntervalTask = Task.Delay(+TimeSpan.FromSeconds(_buttomInterval), _cancellationTokenSource.Token);
            _buttonIntervalObservable = Observable.Timer(TimeSpan.FromSeconds(ButtomInterval));
            _sender.Connect();
        }
        public OSCSender(int port)
        {
            _sender = CreateNewClient(DefaultIPAddress.ToString(), port);
            _buttomIntervalTask = Task.Delay(+TimeSpan.FromSeconds(_buttomInterval), _cancellationTokenSource.Token);
            _buttonIntervalObservable = Observable.Timer(TimeSpan.FromSeconds(ButtomInterval));
            _sender.Connect();
        }
        public OSCSender(string address, int port)
        {
            _sender = CreateNewClient(address, port);
            _buttomIntervalTask = Task.Delay(+TimeSpan.FromSeconds(_buttomInterval), _cancellationTokenSource.Token);
            _buttonIntervalObservable = Observable.Timer(TimeSpan.FromSeconds(ButtomInterval));
            _sender.Connect();
        }

        private OscSender CreateNewClient()
        {
            return CreateNewClient(DefaultIPAddress.ToString(), DefaultPort);
        }
        private OscSender CreateNewClient(string iPAddress, int port)
        {
            IPAddress address;
            try
            {
                address = IPAddress.Parse(iPAddress);
            }
            catch (FormatException)
            {
                address = DefaultIPAddress;
            }

            OscSender client;
            try
            {
                client = new OscSender(address, 0, port);
            }
            catch (ArgumentOutOfRangeException)
            {
                client = new OscSender(address, 0, DefaultPort);
            }

            return client;
        }

        public void ChangeClient(string iPAddress, int port)
        {
            _sender.Close();
            _sender = CreateNewClient(iPAddress, port);
            _sender.Connect();
        }

        public void SendMessage(string path, params object[] args)
        {
            if (args.Any(obj => obj == null))
            {
                return;
            }

            OscMessage message;
            if (string.IsNullOrWhiteSpace(path))
            {
                path = "/vrclogevent/invalid";
            }

            try
            {
                message = new OscMessage(path, args);
            }
            catch (ArgumentException)
            {
                message = new OscMessage("/vrclogevent/invalid", args);
            }
            _sender.Send(message);
        }

        public void ToggleMessage(string path, params object[] args)
        {
            SendMessage(path, args);
        }

        public void ButtomMessage(string path, object offValue, params object[] args)
        {
            SendMessage(path, args);
            Observable.Timer(TimeSpan.FromSeconds(ButtomInterval)).Subscribe(_ => SendMessage(path, offValue));
        }

        public void ButtomMessage(string path, params bool[] args)
        {
            ButtomMessage(path, false, args);
        }

        public void ButtomMessage(string path, params int[] args)
        {
            ButtomMessage(path, 0, args);
        }

        public void ButtomMessage(string path, params float[] args)
        {
            ButtomMessage(path, 0.0f, args);
        }

        public void ButtomMessage(string path, params string[] args)
        {
            ButtomMessage(path, string.Empty, args);
        }

        public async Task ButtomMessageAsync(string path, object offValue, params object[] args)
        {
            SendMessage(path, args);
            await _buttomIntervalTask;
            SendMessage(path, offValue);
        }

        public Task ButtomMessageAsync(string path, params bool[] args)
        {
            return ButtomMessageAsync(path, false, args);
        }

        public Task ButtomMessageAsync(string path, params int[] args)
        {
            return ButtomMessageAsync(path, 0, args);
        }

        public Task ButtomMessageAsync(string path, params float[] args)
        {
            return ButtomMessageAsync(path, 0.0f, args);
        }

        public Task ButtomMessageAsync(string path, params string[] args)
        {
            return ButtomMessageAsync(path, string.Empty, args);
        }

    }
}
