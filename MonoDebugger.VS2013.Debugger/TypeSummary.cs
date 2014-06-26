using Mono.Debugger.Soft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoDebugger.VS2013.Debugger
{
    class TypeSummary
    {
        private MethodMirror[] _methods;

        public TypeMirror TypeMirror { get; set; }

        public MethodMirror[] Methods
        {
            get
            {
                lock(this)
                {
                    if (_methods == null && TypeMirror != null)
                        _methods = TypeMirror.GetMethods();
                }

                return _methods;
            }
        }
    }
}
