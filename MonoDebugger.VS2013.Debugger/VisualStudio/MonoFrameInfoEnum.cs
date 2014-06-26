using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoDebugger.VS2013.Debugger.VisualStudio
{
    class MonoFrameInfoEnum : MonoEnumerator<FRAMEINFO, IEnumDebugFrameInfo2>, IEnumDebugFrameInfo2
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
