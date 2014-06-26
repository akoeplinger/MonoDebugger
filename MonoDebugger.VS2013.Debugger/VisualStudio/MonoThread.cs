using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugger.Soft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoDebugger.VS2013.Debugger.VisualStudio;

namespace MonoDebugger.VS2013.Debugger
{
    class MonoThread : IDebugThread2
    {
        public ThreadMirror ThreadMirror { get; private set; }

        private string _threadName = "Mono Thread";
        private MonoEngine _engine;

        public MonoThread(MonoEngine engine, ThreadMirror threadMirror)
        {
            _engine = engine;
            ThreadMirror = threadMirror;
        }

        public int CanSetNextStatement(IDebugStackFrame2 pStackFrame, IDebugCodeContext2 pCodeContext)
        {
            return VSConstants.S_FALSE;
        }

        public int EnumFrameInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, uint nRadix, out IEnumDebugFrameInfo2 ppEnum)
        {
            var stackFrames = ThreadMirror.GetFrames();
            ppEnum = new MonoFrameInfoEnum(stackFrames.Select(x => new MonoStackFrame(this, x).GetFrameInfo(dwFieldSpec)));
            return VSConstants.S_OK;
        }

        public int GetLogicalThread(IDebugStackFrame2 pStackFrame, out IDebugLogicalThread2 ppLogicalThread)
        {
            throw new NotImplementedException();
        }

        public int GetName(out string pbstrName)
        {
            pbstrName = _threadName;
            return VSConstants.S_OK;
        }

        public int GetProgram(out IDebugProgram2 ppProgram)
        {
            ppProgram = _engine;
            return VSConstants.S_OK;
        }

        public int GetThreadId(out uint pdwThreadId)
        {
            pdwThreadId = 1234;
            return VSConstants.S_OK;
        }

        public int GetThreadProperties(enum_THREADPROPERTY_FIELDS dwFields, THREADPROPERTIES[] ptp)
        {
            return VSConstants.S_OK;
        }

        public int Resume(out uint pdwSuspendCount)
        {
            pdwSuspendCount = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int SetNextStatement(IDebugStackFrame2 pStackFrame, IDebugCodeContext2 pCodeContext)
        {
            return VSConstants.S_FALSE;
        }

        public int SetThreadName(string pszName)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Suspend(out uint pdwSuspendCount)
        {
            pdwSuspendCount = 0;
            return VSConstants.E_NOTIMPL;
        }
    }
}
