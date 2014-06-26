using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonoDebugger.VS2013.Debugger.VisualStudio
{
    class AsyncDispatcher
    {
        private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();
        private AutoResetEvent _wait = new AutoResetEvent(false);
        private volatile bool _running = true;

        public AsyncDispatcher()
        {
            Task.Factory.StartNew(Run);
        }

        private void Run()
        {
            while (_running)
            {
                _wait.WaitOne();

                lock (_wait)
                {
                    Action action;
                    if (_actions.TryDequeue(out action))
                    {
                        action();
                    }
                    else
                        _wait.Reset();
                }
            }
        }

        public void Queue(Action action)
        {
            lock (_wait)
            {
                _actions.Enqueue(action);
                _wait.Set();
            }
        }

        internal void Stop()
        {
            _running = false;
            _wait.Set();
        }
    }
}
