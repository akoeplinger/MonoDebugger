using Mono.Debugger.Soft;

namespace MonoDebugger.VS2013.Debugger
{
    public class TypeSummary
    {
        private MethodMirror[] _methods;

        public TypeMirror TypeMirror { get; set; }

        public MethodMirror[] Methods
        {
            get
            {
                lock (this)
                {
                    if (_methods == null && TypeMirror != null)
                        _methods = TypeMirror.GetMethods();
                }

                return _methods;
            }
        }
    }
}