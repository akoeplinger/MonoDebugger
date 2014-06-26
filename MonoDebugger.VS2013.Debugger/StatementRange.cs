using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoDebugger.VS2013.Debugger
{
    class StatementRange
    {
        public int StartLine { get; set; }

        public int StartColumn { get; set; }

        public int EndLine { get; set; }

        public int EndColumn { get; set; }
    }
}
