using Mono.Debugger.Soft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoDebugger.VS2013.Debugger.VisualStudio
{
    class MonoBreakpointLocation
    {
        public MethodMirror Method { get; set; }
        public long Offset { get; set; }
    }
}
