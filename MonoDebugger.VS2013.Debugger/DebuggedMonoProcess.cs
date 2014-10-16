using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugger.Soft;
using MonoDebugger.VS2013.Debugger.VisualStudio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace MonoDebugger.VS2013.Debugger
{
    public class DebuggedMonoProcess
    {
        public event EventHandler ApplicationClosed;

        private readonly Dictionary<string, TypeSummary> _types = new Dictionary<string, TypeSummary>();
        private readonly IPAddress _ipAddress;
        private List<MonoPendingBreakpoint> _pendingBreakpoints = new List<MonoPendingBreakpoint>();
        private volatile bool _isRunning = true;
        private VirtualMachine _vm;
        private MonoEngine _engine;
        private MonoThread _mainThread;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        public DebuggedMonoProcess(MonoEngine engine, IPAddress ipAddress)
        {
            _engine = engine;
            _ipAddress = ipAddress;
        }

        internal void StartDebugging()
        {
            if (_vm != null)
                return;

            _vm = VirtualMachineManager.Connect(new IPEndPoint(_ipAddress, 11000));
            _vm.EnableEvents(EventType.AssemblyLoad,
                             EventType.ThreadStart,
                             EventType.ThreadDeath,
                             EventType.AssemblyUnload,
                             EventType.UserBreak,
                             EventType.Exception,
                             EventType.UserLog,
                             EventType.KeepAlive,
                             EventType.TypeLoad);

            var set = _vm.GetNextEventSet();
            if (set.Events.OfType<VMStartEvent>().Any())
            {
                _mainThread = new MonoThread(_engine, ((VMStartEvent)set.Events[0]).Thread);
                _engine.Events.ThreadStarted(_mainThread);
                
                Task.Factory.StartNew(ReceiveThread, TaskCreationOptions.LongRunning);
            }
            else
                throw new Exception("Didnt get VMStart-Event!");
        }

        internal void Attach()
        {
        }

        private void ReceiveThread()
        {
            Thread.Sleep(3000);
            _vm.Resume();

            while (_isRunning)
            {
                var set = _vm.GetNextEventSet();

                bool resume = false;
                foreach (var ev in set.Events)
                {
                    resume = resume || HandleEventSet(ev);
                }

                if (resume && _vm != null)
                    _vm.Resume();
            }
        }

        private bool HandleEventSet(Event ev)
        {
            if (ev.EventType == EventType.Breakpoint)
            {
                HandleBreakPoint((BreakpointEvent)ev);
                return false;
            }
            else if (ev.EventType == EventType.Step)
            {
                HandleStep((StepEvent)ev);
                return false;
            }
            else
            {
                switch (ev.EventType)
                {
                    case EventType.TypeLoad:
                        var typeEvent = (TypeLoadEvent)ev;
                        RegisterType(typeEvent.Type);
                        if (TryBindBreakpoints() != 0)
                            return false;

                        break;
                    case EventType.VMDeath:
                    case EventType.VMDisconnect:
                        Disconnect();
                        return false;
                    default:
                        logger.Trace(ev);
                        break;
                }

                return true;
            }
        }

        private void HandleStep(StepEvent stepEvent)
        {
            stepEvent.Request.Disable();
            _engine.Events.StepCompleted(_mainThread);
            logger.Trace(string.Format("Stepping: {0}:{1}", stepEvent.Method.Name, stepEvent.Location));
        }

        private void HandleBreakPoint(BreakpointEvent bpEvent)
        {
            var bp = _pendingBreakpoints.FirstOrDefault(x => x.LastRequest == bpEvent.Request);
            var frames = bpEvent.Thread.GetFrames();
            _engine.Events.BreakpointHit(bp, _mainThread);
        }

        private int TryBindBreakpoints()
        {
            int countBounded = 0;

            try
            {
                foreach (var bp in _pendingBreakpoints.Where(x => !x.IsBound))
                {
                    MonoBreakpointLocation location;
                    if (bp.TryBind(_types, out location))
                    {
                        try
                        {
                            int ilOffset;
                            var position = RoslynHelper.GetILOffset(bp, location.Method, out ilOffset);

                            var request = _vm.SetBreakpoint(location.Method, ilOffset);
                            request.Enable();
                            bp.IsBound = true;
                            bp.LastRequest = request;
                            _engine.Events.BoundBreakpoint(bp);
                            _vm.Resume();
                            bp.CurrentThread = _mainThread;
                            countBounded++;
                        }
                        catch (Exception ex)
                        {
                            logger.Trace("Cant bind breakpoint: " + ex);
                        }
                    }
                }
            }
            catch
            {
            }


            return countBounded;
        }

        private void Disconnect()
        {
            _isRunning = false;
            Terminate();
            if (ApplicationClosed != null)
                ApplicationClosed(this, EventArgs.Empty);
        }

        private void RegisterType(TypeMirror typeMirror)
        {
            if (!_types.ContainsKey(typeMirror.FullName))
            {
                _types.Add(typeMirror.FullName, new TypeSummary
                {
                    TypeMirror = typeMirror,
                });
                
                string typeName = typeMirror.Name;
                if (!string.IsNullOrEmpty(typeMirror.Namespace))
                    typeName = typeMirror.Namespace + "." + typeMirror.Name;
                logger.Trace("Loaded and registered Type: " + typeName);
            }
        }

        internal void WaitForAttach()
        {
        }

        internal void Break()
        {

        }

        internal void Continue()
        {
        }

        internal void Resume()
        {
            _vm.Resume();
        }

        internal void Execute(MonoThread debuggedMonoThread)
        {
            _vm.Resume();
        }

        internal void Terminate()
        {
            try
            {
                if (_vm != null)
                {
                    _vm.Detach();
                    _vm = null;
                }
            }
            catch
            {
            }
        }

        internal MonoPendingBreakpoint AddPendingBreakpoint(IDebugBreakpointRequest2 pBPRequest)
        {
            var bp = new MonoPendingBreakpoint(_engine, pBPRequest);
            _pendingBreakpoints.Add(bp);
            TryBindBreakpoints();
            return bp;
        }

        internal void Step(MonoThread thread, enum_STEPKIND sk)
        {
            var request = _vm.CreateStepRequest(thread.ThreadMirror);
            switch (sk)
            {
                case enum_STEPKIND.STEP_INTO:
                    request.Depth = StepDepth.Into;
                    break;
                case enum_STEPKIND.STEP_OUT:
                    request.Depth = StepDepth.Out;
                    break;
                case enum_STEPKIND.STEP_OVER:
                    request.Depth = StepDepth.Over;
                    break;
                default:
                    return;
            }
            request.Size = StepSize.Line;
            request.Enable();
            _vm.Resume();
        }
    }
}
