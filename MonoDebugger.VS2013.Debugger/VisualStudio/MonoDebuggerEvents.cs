using Microsoft.VisualStudio.Debugger.Interop;
using MonoDebugger.VS2013.Debugger.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MonoDebugger.VS2013.Debugger.VisualStudio;

namespace MonoDebugger.VS2013.Debugger
{
    public class MonoDebuggerEvents
    {
        private readonly MonoEngine _engine;
        private readonly IDebugEventCallback2 _callback;

        public MonoDebuggerEvents(MonoEngine monoEngine, IDebugEventCallback2 pCallback)
        {
            _engine = monoEngine;
            _callback = pCallback;
        }

        public void EngineCreated()
        {
            var iid = new Guid(EngineCreateEvent.IID);
            _callback.Event(_engine, _engine.RemoteProcess, _engine, null, new EngineCreateEvent(_engine), ref iid, EngineCreateEvent.Attributes);
        }

        public void ProgramCreated()
        {
            var iid = new Guid(ProgramCreateEvent.IID);
            _callback.Event(_engine, null, _engine, null, new ProgramCreateEvent(), ref iid, ProgramCreateEvent.Attributes);
        }

        public void EngineLoaded()
        {
            var iid = new Guid(LoadCompleteEvent.IID);
            _callback.Event(_engine, _engine.RemoteProcess, _engine, null, new LoadCompleteEvent(), ref iid, LoadCompleteEvent.Attributes);
        }

        internal void DebugEntryPoint()
        {
            Guid iid = new Guid(DebugEntryPointEvent.IID);
            _callback.Event(_engine, _engine.RemoteProcess, _engine, null, new DebugEntryPointEvent(_engine), ref iid, DebugEntryPointEvent.Attributes);
        }

        internal void ProgramDestroyed(IDebugProgram2 program)
        {
            Guid iid = new Guid(ProgramDestroyedEvent.IID);
            _callback.Event(_engine, null, program, null, new ProgramDestroyedEvent(), ref iid, ProgramDestroyedEvent.Attributes);
        }

        internal void BoundBreakpoint(MonoPendingBreakpoint breakpoint)
        {
            Guid iid = new Guid(BreakPointEvent.IID);
            _callback.Event(_engine, _engine.RemoteProcess, _engine, null, new BreakPointEvent(breakpoint), ref iid, BreakPointEvent.Attributes);
        }

        internal void BreakpointHit(MonoPendingBreakpoint breakpoint, MonoThread thread)
        {
            Guid iid = new Guid(BreakPointHitEvent.IID);
            _callback.Event(_engine, _engine.RemoteProcess, _engine, thread, new BreakPointHitEvent(breakpoint), ref iid, BreakPointHitEvent.Attributes);
        }

        internal void ThreadStarted(MonoThread thread)
        {
            Guid iid = new Guid(ThreadCreateEvent.IID);
            _callback.Event(_engine, _engine.RemoteProcess, _engine, thread, new ThreadCreateEvent(), ref iid, BreakPointHitEvent.Attributes);
        }

        internal void StepCompleted(MonoThread thread)
        {
            Guid iid = new Guid(StepCompleteEvent.IID);
            _callback.Event(_engine, _engine.RemoteProcess, _engine, thread, new StepCompleteEvent(), ref iid, StepCompleteEvent.Attributes);
        }
    }
}
