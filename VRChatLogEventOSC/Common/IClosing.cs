using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChatLogEventOSC
{
    /// <summary>
    /// Window.Closingに+=する用
    /// </summary>
    internal interface IClosing
    {
        public void Closing(CancelEventArgs cancelEventArgs);
    }
}
