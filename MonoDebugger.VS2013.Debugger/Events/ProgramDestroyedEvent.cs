using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.VS2013.Debugger.Events
{
    class ProgramDestroyedEvent : AsynchronousEvent, IDebugProgramDestroyEvent2
    {
        public const string IID = "E147E9E3-6440-4073-A7B7-A65592C714B5";

        #region IDebugProgramDestroyEvent2 Members

        int IDebugProgramDestroyEvent2.GetExitCode(out uint pdwExit)
        {
            pdwExit = 0;
            return VSConstants.S_OK;
        }

        #endregion
    }
}
