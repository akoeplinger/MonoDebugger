using System.Collections.Generic;
using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoDebugger.VS2013.Debugger.VisualStudio
{
    internal class MonoBoundBreakpointEnumerator : MonoEnumerator<IDebugBoundBreakpoint2, IEnumDebugBoundBreakpoints2>,
        IEnumDebugBoundBreakpoints2
    {
        public MonoBoundBreakpointEnumerator(IEnumerable<MonoBoundBreakpoint> data)
            : base(data)
        {
        }

        public int Next(uint celt, IDebugBoundBreakpoint2[] rgelt, ref uint celtFetched)
        {
            return Next(celt, rgelt, out celtFetched);
        }
    }
}