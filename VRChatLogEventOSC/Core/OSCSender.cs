using System;
using System.ComponentModel;
using Reactive.Bindings;
using System.Reactive.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Rug.Osc;

namespace VRChatLogEventOSC.Core
{
    internal sealed class OSCSender : IDisposable
    {
        private static readonly IPAddress DefaultIPAddress = IPAddress.Loopback;
        private static readonly int DefaultPort = 9000;
        private OscSender _sender;
        private float _buttomInterval = 0.3f;

        /// <summary>
        /// OSCTypeがButtonの際に、何秒後に値を戻すか
        /// </summary>
        public float ButtomInterval
        {
            get => _buttomInterval;
            set
            {
                _buttomInterval = value;
            }
        }

        public bool BoolButtonOffValue { get; set; } = false;
        public int IntButtonOffValue { get; set; } = 0;
        public float FloatButtonOffValue { get; set; } = 0.0f;
        public string StringButtonOffValue { get; set; } = string.Empty;

        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _sender.Close();
            _disposed = true;
        }
        public OSCSender()
        {
            _sender = CreateNewClient(DefaultIPAddress.ToString(), DefaultPort);
            _sender.Connect();
        }
        public OSCSender(string address)
        {
            _sender = CreateNewClient(address, DefaultPort);
            _sender.Connect();
        }
        public OSCSender(int port)
        {
            _sender = CreateNewClient(DefaultIPAddress.ToString(), port);
            _sender.Connect();
        }
        public OSCSender(string address, int port)
        {
            _sender = CreateNewClient(address, port);
            _sender.Connect();
        }

        /// <summary>
        /// ループバックIP Addressと9000番ポートでOSCクライアントを作製します
        /// </summary>
        /// <returns>作製したOSCクライアント</returns>
        private static OscSender CreateNewClient()
        {
            return CreateNewClient(DefaultIPAddress.ToString(), DefaultPort);
        }

        /// <summary>
        /// 指定したIP AddressとPort番号でOSCクライアントを作製します
        /// </summary>
        /// <param name="iPAddress">作製するクライアントの送信先IP Address</param>
        /// <param name="port">作製するクライアントの送信先Port番号</param>
        /// <returns>作製したOSCクライアント</returns>
        private static OscSender CreateNewClient(string iPAddress, int port)
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

        /// <summary>
        /// OSC送信先のIP AddressとPortを変更します
        /// </summary>
        /// <param name="iPAddress">変更先のIP Address</param>
        /// <param name="port">変更先のPort番号</param>
        public void ChangeClient(string iPAddress, int port)
        {
            _sender.Close();
            _sender = CreateNewClient(iPAddress, port);
            _sender.Connect();
        }

        /// <summary>
        /// OSC送信先のIP Addressを変更します
        /// </summary>
        /// <param name="iPAddress">変更先のIP Address</param>
        public void ChangeClient(string iPAddress)
        {
            _sender.Close();
            _sender = CreateNewClient(iPAddress, DefaultPort);
            _sender.Connect();
        }

        /// <summary>
        /// OSC送信先のPortを変更します
        /// </summary>
        /// <param name="port">変更先のPort番号</param>
        public void ChangeClient(int port)
        {
            _sender.Close();
            _sender = CreateNewClient(DefaultIPAddress.ToString(), port);
            _sender.Connect();
        }

        /// <summary>
        /// 指定したPathと値でOSCを送信します
        /// </summary>
        /// <param name="path">送信先</param>
        /// <param name="args">送信する値</param>
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

        /// <summary>
        /// 指定したPathと値でOSCを送信します
        /// </summary>
        /// <param name="path">送信先</param>
        /// <param name="args">送信する値</param>
        public void ToggleMessage(string path, params object[] args)
        {
            SendMessage(path, args);
        }

        /// <summary>
        /// 指定したPathと値でOSCを送信し、一定時間後に指定した値で再送します
        /// </summary>
        /// <param name="path">送信先</param>
        /// <param name="offValue">再送する値</param>
        /// <param name="args">送信する値</param>
        private void ButtomMessage(string path, object offValue, params object[] args)
        {
            SendMessage(path, args);
            Observable.Timer(TimeSpan.FromSeconds(ButtomInterval)).Subscribe(_ => SendMessage(path, offValue));
        }

        /// <summary>
        /// 指定したPathと値でOSCを送信し、一定時間後に規定の値で再送します
        /// </summary>
        /// <param name="path">送信先</param>
        /// <param name="value">送信する値</param>
        public void ButtomMessage(string path, object value)
        {
            switch (value)
            {
                case bool bval:
                    ButtomMessage(path, BoolButtonOffValue, bval);
                    break;
                case int ival:
                    ButtomMessage(path, IntButtonOffValue, ival);
                    break;
                case float fval:
                    ButtomMessage(path, FloatButtonOffValue, fval);
                    break;
                case string sval:
                    ButtomMessage(path, StringButtonOffValue, sval);
                    break;
                default:
                    break;
            }
        }
    }
}
