using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Reactive.Bindings.Extensions;
using VRChatLogEventOSC.Common;
using static VRChatLogEventOSC.Common.RegexPattern;

using System.Diagnostics;

namespace VRChatLogEventOSC
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
        }

        private static bool IsEventMatchSetting(Match match, SingleSetting setting, IEnumerable<string> captures)
        {
            bool matchAll = true;
            foreach (var capture in captures)
            {
                string settingCapture = setting.CaptureProperty(capture);
                string matchCapture = match.Groups[capture].Value;

                if (capture == "ReqInv")
                {
                    if (string.IsNullOrEmpty(settingCapture))
                    {
                        continue;
                    }
                    else if (settingCapture == "None" && string.IsNullOrEmpty(matchCapture))
                    {
                        continue;
                    }
                    else if (settingCapture == "~canRequestInvite" && "~canRequestInvite" == matchCapture)
                    {
                        continue;
                    }
                }

                if (string.IsNullOrWhiteSpace(settingCapture))
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

        public EventToOSCConverter(LineClassifier lineClassifier, OSCSender oSCSender)
        {
            _lineClassifier = lineClassifier;
            _oSCSender = oSCSender;

            
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
                        if (setting.OSCValue == null)
                        {
                            continue;
                        }

                        if (!IsEventMatchSetting(match, setting, captures))
                        {
                            continue;
                        }

                        if (setting.OSCType == SingleSetting.OSCTypeEnum.Button)
                        {
                            _oSCSender.ButtomMessage(setting.OSCAddress, setting.OSCValue);
                        }
                        else if (setting.OSCType == SingleSetting.OSCTypeEnum.Toggle)
                        {
                            _oSCSender.ToggleMessage(setting.OSCAddress, setting.OSCValue);
                        }
                        
                    }
                }).AddTo(_eventsDisposables);
            }
        }
    }
}
