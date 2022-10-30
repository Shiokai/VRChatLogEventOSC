using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Reactive.Bindings.Extensions;

using VRChatLogEventOSC.Common;

using static VRChatLogEventOSC.Common.RegexPattern;

namespace VRChatLogEventOSC.Core
{
    internal sealed class EventToOSCConverter : IDisposable
    {
        private readonly LineClassifier _lineClassifier;
        private readonly OSCSender _oSCSender;
        private readonly CompositeDisposable _eventsDisposables = new();
        public WholeSetting CurrentSetting {get; set;} = new();

        private bool _disposed = false;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _lineClassifier.Dispose();
            _eventsDisposables.Dispose();
            _oSCSender.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// ReqestInviteの設定がイベントとマッチしているか判定します
        /// </summary>
        /// <param name="settingCapture">判定する設定のReqInv</param>
        /// <param name="matchCapture">判定するイベントのReqInv</param>
        /// <returns>設定とイベントがマッチする場合trueを、そうでなければfalseを返します</returns>
        private static bool IsMatchReqInvSetting(string settingCapture, string matchCapture)
        {
            // ReqInveはInvete+以外取れないので特殊判定
            if (settingCapture == "NotSpecified")
            {
                return true;
            }
            else if (settingCapture == "None" && string.IsNullOrEmpty(matchCapture))
            {
                return true;
            }
            else if (settingCapture == "CanRequestInvite" && "~canRequestInvite" == matchCapture)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// イベントと設定がマッチするか判定します
        /// </summary>
        /// <param name="match">判定するイベントの正規表現マッチ</param>
        /// <param name="setting">判定する設定</param>
        /// <param name="captures">判定するイベントの正規表現の名前付きグループの名前一覧</param>
        /// <returns></returns>
        private static bool IsEventMatchSetting(Match match, SingleSetting setting, IEnumerable<string> captures)
        {
            // "continue" is resolved as "matched" or "pass".
            bool matchAll = true;
            foreach (var capture in captures)
            {
                string settingCapture = setting.CaptureProperty(capture);
                string matchCapture = match.Groups[capture].Value;

                // ReqInvは特殊判定
                if (capture == "ReqInv" && IsMatchReqInvSetting(settingCapture, matchCapture))
                {
                    continue;
                }

                // 空欄はフィルタリング無し扱い(ReqInv除く)
                if (string.IsNullOrWhiteSpace(settingCapture))
                {
                    continue;
                }

                // インスタンスがpublicのとき、InstanceTypeが取れないので特殊判定
                if (capture == "InstanceType" && string.IsNullOrEmpty(matchCapture) && settingCapture == "public")
                {
                    continue;
                }

                // Regionがusのとき、Regionが取れないので特殊判定
                if (capture == "Region" && string.IsNullOrEmpty(matchCapture) && settingCapture == "us")
                {
                    continue;
                }

                if (settingCapture == matchCapture)
                {
                    continue;
                }

                matchAll = false;

                if (matchAll == false)
                {
                    return false;
                }
            }

            return matchAll;
        }

        /// <summary>
        /// 設定とイベントがマッチする場合に、設定に従いOSCを送信します
        /// </summary>
        /// <param name="match">判定するイベントの正規表現マッチ</param>
        /// <param name="setting">判定する設定</param>
        /// <param name="captures">判定するイベントの正規表現の名前付きグループの一覧</param>
        private void SendIfValid(Match match, SingleSetting setting, IEnumerable<string> captures)
        {
            // 送信すべき値が無いなら送信しない
            if (setting.OSCValue == null)
            {
                return;
            }

            // 設定でフィルタリング
            if (!IsEventMatchSetting(match, setting, captures))
            {
                return;
            }

            if (setting.OSCType == SingleSetting.OSCTypeEnum.Button)
            {
                _oSCSender.ButtomMessage(setting.OSCAddress, setting.OSCValue);
            }
            else if (setting.OSCType == SingleSetting.OSCTypeEnum.Toggle)
            {
                // VRChatのchatboxに送信するときは
                // /chatbox/input s b
                // の形で送る必要がある
                // 自動送信でentry UIを表示させる需要は少ないと思われるため、直接chatboxに送信(true)する
                if (setting.OSCAddress == "/chatbox/input")
                {
                    _oSCSender.ToggleMessage(setting.OSCAddress, setting.OSCValue, true);
                    return;
                }

                _oSCSender.ToggleMessage(setting.OSCAddress, setting.OSCValue);
            }
        }

        public EventToOSCConverter(LineClassifier lineClassifier, OSCSender oSCSender)
        {
            _lineClassifier = lineClassifier;
            _oSCSender = oSCSender;

            // 全イベントをSubscribe
            foreach (var type in Enum.GetValues<EventTypeEnum>())
            {
                if (type == EventTypeEnum.None || !Regexes.ContainsKey(type))
                {
                    continue;
                }

                var diposable = _lineClassifier.EventReactiveProperties[type]
                .Where(e => e != null)
                .Subscribe( e =>
                {
                    Match match = Regexes[type].Match(e);
                    IEnumerable<string> captures = CaptureNames(type);

                    foreach (var setting in CurrentSetting.Settings[type])
                    {
                        SendIfValid(match, setting, captures);
                    }
                }).AddTo(_eventsDisposables);
            }
        }
    }
}
