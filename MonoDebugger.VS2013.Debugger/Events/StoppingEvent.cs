using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoDebugger.VS2013.Debugger.Events
{
    class StoppingEvent : IDebugEvent2
    {
        public const uint Attributes = (uint)enum_EVENTATTRIBUTES.EVENT_SYNC_STOP;

        int IDebugEvent2.GetAttributes(out uint eventAttributes)
        {
            eventAttributes = Attributes;
            return VSConstants.S_OK;
        }
    }
}
