using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.VS2013.Debugger
{
    public static class DebugHelper
    {
        internal static void TraceEnteringMethod([CallerMemberName] string callerMember = "")
        {
            var mth = new StackTrace().GetFrame(1).GetMethod();
            var className = mth.ReflectedType.Name;
            Trace.WriteLine(className + " (entering) :  " + callerMember);
        }
    }
}
