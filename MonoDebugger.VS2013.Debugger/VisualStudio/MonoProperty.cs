using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugger.Soft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoDebugger.VS2013.Debugger.VisualStudio
{
    class MonoProperty : IDebugProperty2
    {
        private LocalVariable[] _values;
        private StackFrame _frame;

        public MonoProperty(StackFrame frame, IEnumerable<LocalVariable> values)
        {
            _frame = frame;
            _values = values.ToArray();
        }

        public int EnumChildren(enum_DEBUGPROP_INFO_FLAGS dwFields, uint dwRadix, ref Guid guidFilter, enum_DBG_ATTRIB_FLAGS dwAttribFilter, string pszNameFilter, uint dwTimeout, out IEnumDebugPropertyInfo2 ppEnum)
        {
            var properties = _values.Where(x => !string.IsNullOrEmpty(x.Name)).Select(x => GetDebugPropertyInfo(x, dwFields));
            ppEnum = new MonoPropertyInfosEnum(properties);
            return VSConstants.S_OK;
        }

        internal DEBUG_PROPERTY_INFO GetDebugPropertyInfo(LocalVariable variable, enum_DEBUGPROP_INFO_FLAGS dwFields)
        {
            DEBUG_PROPERTY_INFO propertyInfo = new DEBUG_PROPERTY_INFO();

            if ((dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_FULLNAME) != 0)
            {
                propertyInfo.bstrFullName = variable.Name;
                propertyInfo.dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_FULLNAME;
            }

            if ((dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_NAME) != 0)
            {
                propertyInfo.bstrName = variable.Name;
                propertyInfo.dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_NAME;
            }

            if ((dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_TYPE) != 0)
            {
                propertyInfo.bstrType = variable.Type.Namespace + "." + variable.Type.Name;
                propertyInfo.dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_TYPE;
            }

            if ((dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE) != 0)
            {
                var value = _frame.GetValue(variable);
                if (value is ObjectMirror)
                {
                    var obj = ((ObjectMirror)value);
                    var toStringMethod = obj.Type.GetMethod("ToString");
                    value = obj.InvokeMethod(_frame.Thread, toStringMethod, Enumerable.Empty<Value>().ToList(), InvokeOptions.DisableBreakpoints);
                    propertyInfo.bstrValue = ((StringMirror)value).Value;
                }
                else if (value is PrimitiveValue)
                {
                    var obj = ((PrimitiveValue)value);
                    if (obj.Value != null)
                        propertyInfo.bstrValue = obj.Value.ToString();
                }

                propertyInfo.dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE;
            }

            if ((dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ATTRIB) != 0)
            {
                propertyInfo.dwAttrib = enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_READONLY;

                if (IsExpandable(variable))
                {
                    propertyInfo.dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_OBJ_IS_EXPANDABLE;
                }
                propertyInfo.dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ATTRIB;

            }

            if (((dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_PROP) != 0) || IsExpandable(variable))
            {
                propertyInfo.pProperty = (IDebugProperty2)this;
                propertyInfo.dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_PROP;
            }

            return propertyInfo;
        }

        private bool IsExpandable(LocalVariable variable)
        {
            return true;
        }

        public int GetDerivedMostProperty(out IDebugProperty2 ppDerivedMost)
        {
            throw new NotImplementedException();
        }

        public int GetExtendedInfo(ref Guid guidExtendedInfo, out object pExtendedInfo)
        {
            throw new NotImplementedException();
        }

        public int GetMemoryBytes(out IDebugMemoryBytes2 ppMemoryBytes)
        {
            throw new NotImplementedException();
        }

        public int GetMemoryContext(out IDebugMemoryContext2 ppMemory)
        {
            throw new NotImplementedException();
        }

        public int GetParent(out IDebugProperty2 ppParent)
        {
            throw new NotImplementedException();
        }

        public int GetPropertyInfo(enum_DEBUGPROP_INFO_FLAGS dwFields, uint dwRadix, uint dwTimeout, IDebugReference2[] rgpArgs, uint dwArgCount, DEBUG_PROPERTY_INFO[] pPropertyInfo)
        {
            rgpArgs = null;
            if (_values.Length != 1)
                return VSConstants.E_NOTIMPL;

            pPropertyInfo[0] = GetDebugPropertyInfo(_values[0], dwFields);
            return VSConstants.S_OK;
        }

        public int GetReference(out IDebugReference2 ppReference)
        {
            throw new NotImplementedException();
        }

        public int GetSize(out uint pdwSize)
        {
            throw new NotImplementedException();
        }

        public int SetValueAsReference(IDebugReference2[] rgpArgs, uint dwArgCount, IDebugReference2 pValue, uint dwTimeout)
        {
            throw new NotImplementedException();
        }

        public int SetValueAsString(string pszValue, uint dwRadix, uint dwTimeout)
        {
            throw new NotImplementedException();
        }
    }
}
