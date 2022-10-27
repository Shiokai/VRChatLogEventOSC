using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChatLogEventOSC
{
    internal interface IClosing
    {
        public void Closing(CancelEventArgs cancelEventArgs);
    }
}
