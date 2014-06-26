using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugger.Soft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoDebugger.VS2013.Debugger.VisualStudio
{
    class MonoStackFrame : IDebugStackFrame2, IDebugExpressionContext2
    {
        private StackFrame _frame;
        private MonoDocumentContext _docContext;
        private MonoProperty _locals;
        private MonoThread _thread;

        public MonoStackFrame(MonoThread thread, StackFrame frame)
        {
            _thread = thread;
            _frame = frame;

            _docContext = new MonoDocumentContext(_frame.FileName,
                                                  _frame.LineNumber,
                                                  _frame.ColumnNumber);
            var locals = frame.GetVisibleVariables().ToList();
            _locals = new MonoProperty(frame, locals);
        }

        internal FRAMEINFO GetFrameInfo(enum_FRAMEINFO_FLAGS dwFieldSpec)
        {
            var frameInfo = new FRAMEINFO();
            frameInfo.m_bstrFuncName = _frame.Location.Method.Name;
            frameInfo.m_bstrModule = _frame.FileName;
            frameInfo.m_pFrame = this;
            frameInfo.m_fHasDebugInfo = 1;
            frameInfo.m_fStaleCode = 0;

            frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_STALECODE;
            frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_DEBUGINFO;
            frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_MODULE;
            frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_FUNCNAME;
            return frameInfo;
        }

        public int EnumProperties(enum_DEBUGPROP_INFO_FLAGS dwFields, uint nRadix, ref Guid guidFilter, uint dwTimeout, out uint pcelt, out IEnumDebugPropertyInfo2 ppEnum)
        {
            var res = _locals.EnumChildren(dwFields, nRadix, ref guidFilter, 0, null, dwTimeout, out ppEnum);
            if (ppEnum != null)
                ppEnum.GetCount(out pcelt);
            else
                pcelt = 0;
            return res;
        }

        public int GetCodeContext(out IDebugCodeContext2 ppCodeCxt)
        {
            ppCodeCxt = _docContext;
            return VSConstants.S_OK;
        }

        public int GetDebugProperty(out IDebugProperty2 ppProperty)
        {
            ppProperty = null;// _locals;
            return VSConstants.S_OK;
        }

        public int GetDocumentContext(out IDebugDocumentContext2 ppCxt)
        {
            ppCxt = _docContext;
            return VSConstants.S_OK;
        }

        public int GetExpressionContext(out IDebugExpressionContext2 ppExprCxt)
        {
            ppExprCxt = this;
            return VSConstants.S_OK;
        }

        public int GetInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, uint nRadix, FRAMEINFO[] pFrameInfo)
        {
            pFrameInfo[0] = GetFrameInfo(dwFieldSpec);
            return VSConstants.S_OK;
        }

        public int GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
        {
            pbstrLanguage = MonoGuids.LanguageName;
            pguidLanguage = MonoGuids.LanguageGuid;
            return VSConstants.S_OK;
        }

        public int GetName(out string pbstrName)
        {
            pbstrName = _frame.FileName;
            return VSConstants.S_OK;
        }

        public int GetPhysicalStackRange(out ulong paddrMin, out ulong paddrMax)
        {
            paddrMin = 0;
            paddrMax = 0;
            return VSConstants.S_OK;
        }

        public int GetThread(out IDebugThread2 ppThread)
        {
            ppThread = _thread;
            return VSConstants.S_OK;
        }

        public int ParseText(string pszCode, enum_PARSEFLAGS dwFlags, uint nRadix, out IDebugExpression2 ppExpr, out string pbstrError, out uint pichError)
        {
            pbstrError = "";
            pichError = 0;
            ppExpr = null;
            var lookup = pszCode;

            var result = _frame.GetVisibleVariableByName(lookup);
            if (result != null)
            {
                ppExpr = new TrivialMonoExpression(new MonoProperty(_frame, new[] { result }));
                return VSConstants.S_OK;
            }

            pbstrError = "Unsupported Expression";
            pichError = (uint)pbstrError.Length;
            return VSConstants.S_FALSE;
        }
    }
}
