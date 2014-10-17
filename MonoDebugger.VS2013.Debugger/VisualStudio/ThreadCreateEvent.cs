using Microsoft.VisualStudio.Debugger.Interop;
using MonoDebugger.VS2013.Debugger.Events;

namespace MonoDebugger.VS2013.Debugger.VisualStudio
{
    internal class ThreadCreateEvent : AsynchronousEvent, IDebugThreadCreateEvent2
    {
        public const string IID = "2090CCFC-70C5-491D-A5E8-BAD2DD9EE3EA";
    }
}