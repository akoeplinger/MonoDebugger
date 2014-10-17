using System.Collections.Generic;
using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoDebugger.VS2013.Debugger.VisualStudio
{
    internal class MonoFrameInfoEnum : MonoEnumerator<FRAMEINFO, IEnumDebugFrameInfo2>, IEnumDebugFrameInfo2
    {
        public MonoFrameInfoEnum(IEnumerable<FRAMEINFO> enumerable) : base(enumerable)
        {
        }

        public int Next(uint celt, FRAMEINFO[] rgelt, ref uint celtFetched)
        {
            return Next(celt, rgelt, out celtFetched);
        }
    }
}