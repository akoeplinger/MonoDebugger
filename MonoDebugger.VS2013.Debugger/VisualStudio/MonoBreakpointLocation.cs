using Mono.Debugger.Soft;

namespace MonoDebugger.VS2013.Debugger.VisualStudio
{
    internal class MonoBreakpointLocation
    {
        public MethodMirror Method { get; set; }
        public long Offset { get; set; }
    }
}