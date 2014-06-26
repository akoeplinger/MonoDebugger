using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.VS2013.Debugger.Events
{
    class DebugEntryPointEvent : AsynchronousEvent, IDebugEntryPointEvent2
    {
        public const string IID = "E8414A3E-1642-48EC-829E-5F4040E16DA9";
        public object Engine { get; set; }

        public DebugEntryPointEvent(IDebugEngine2 mEngine)
        {
            Engine = mEngine;
        }
    }
}
