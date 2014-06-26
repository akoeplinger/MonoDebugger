using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MonoDebugger.VS2013.Debugger.VisualStudio
{
    class MonoDocument : IDebugDocument2
    {
        private MonoPendingBreakpoint _pendingBreakpoint;

        public MonoDocument(MonoPendingBreakpoint pendingBreakpoint)
        {
            _pendingBreakpoint = pendingBreakpoint;
        }

        public int GetDocumentClassId(out Guid pclsid)
        {
            throw new NotImplementedException();
        }

        public int GetName(enum_GETNAME_TYPE gnType, out string pbstrFileName)
        {
            gnType = enum_GETNAME_TYPE.GN_FILENAME;
            pbstrFileName = Path.GetFileName(_pendingBreakpoint.DocumentName);
            return VSConstants.S_OK;
        }
    }
}
