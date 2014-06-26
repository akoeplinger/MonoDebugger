using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.VS2013.Debugger.Events
{
    sealed class LoadCompleteEvent : StoppingEvent, IDebugLoadCompleteEvent2
    {
        public const string IID = "B1844850-1349-45D4-9F12-495212F5EB0B";
    }
}
