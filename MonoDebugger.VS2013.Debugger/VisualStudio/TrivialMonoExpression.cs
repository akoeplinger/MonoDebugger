using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoDebugger.VS2013.Debugger.VisualStudio
{
    class TrivialMonoExpression : IDebugExpression2
    {
        private MonoProperty _monoProperty;

        public TrivialMonoExpression(MonoProperty monoProperty)
        {
            _monoProperty = monoProperty;
        }

        public int Abort()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int EvaluateAsync(enum_EVALFLAGS dwFlags, IDebugEventCallback2 pExprCallback)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int EvaluateSync(enum_EVALFLAGS dwFlags, uint dwTimeout, IDebugEventCallback2 pExprCallback, out IDebugProperty2 ppResult)
        {
            ppResult = _monoProperty;
            return VSConstants.S_OK;
        }
    }
}
