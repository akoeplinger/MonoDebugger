using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.VS2013.Debugger.VisualStudio
{
    class MonoPropertyInfosEnum : MonoEnumerator<DEBUG_PROPERTY_INFO, IEnumDebugPropertyInfo2>, IEnumDebugPropertyInfo2
    {
        public MonoPropertyInfosEnum(IEnumerable<DEBUG_PROPERTY_INFO> enumerator)
            : base(enumerator)
        {

        }
    }
}
