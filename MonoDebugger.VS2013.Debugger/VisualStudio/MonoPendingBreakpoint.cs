using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugger.Soft;

namespace MonoDebugger.VS2013.Debugger.VisualStudio
{
    class MonoPendingBreakpoint : IDebugPendingBreakpoint2
    {
        public bool IsBound { get; set; }
        public bool IsEnabled { get; private set; }
        public bool IsDeleted { get; private set; }
        public int StartLine { get; private set; }
        public int StartColumn { get; private set; }
        public int EndLine { get; private set; }
        public int EndColumn { get; private set; }
        public string DocumentName { get; set; }
        public MonoThread CurrentThread { get; set; }
        public EventRequest LastRequest { get; set; }

        private BP_REQUEST_INFO _bpRequestInfo;
        private IDebugBreakpointRequest2 _pBPRequest;
        private MonoBoundBreakpoint _boundBreakpoint;
        private MonoEngine _engine;

        public MonoPendingBreakpoint(MonoEngine engine, IDebugBreakpointRequest2 pBPRequest)
        {
            var requestInfo = new BP_REQUEST_INFO[1];
            pBPRequest.GetRequestInfo(enum_BPREQI_FIELDS.BPREQI_BPLOCATION, requestInfo);
            _bpRequestInfo = requestInfo[0];
            _pBPRequest = pBPRequest;
            _engine = engine;

            IsEnabled = true;

            var docPosition = (IDebugDocumentPosition2)Marshal.GetObjectForIUnknown(_bpRequestInfo.bpLocation.unionmember2);

            string documentName;
            docPosition.GetFileName(out documentName);
            var startPosition = new TEXT_POSITION[1];
            var endPosition = new TEXT_POSITION[1];
            docPosition.GetRange(startPosition, endPosition);

            DocumentName = documentName;
            StartLine = (int)startPosition[0].dwLine;
            StartColumn = (int)startPosition[0].dwColumn;

            EndLine = (int)endPosition[0].dwLine;
            EndColumn = (int)endPosition[0].dwColumn;

        }

        public int Bind()
        {
            try
            {
                _boundBreakpoint = new MonoBoundBreakpoint(_engine, this);
                return VSConstants.S_OK;
            }
            catch
            {
                return VSConstants.S_FALSE;
            }
        }

        public int CanBind(out IEnumDebugErrorBreakpoints2 ppErrorEnum)
        {
            ppErrorEnum = null;
            if (_bpRequestInfo.bpLocation.bpLocationType == (uint)enum_BP_LOCATION_TYPE.BPLT_CODE_FILE_LINE)
                return VSConstants.S_OK;

            return VSConstants.S_FALSE;
        }

        public int Delete()
        {
            return VSConstants.S_OK;
        }

        public int Enable(int fEnable)
        {
            IsEnabled = fEnable != 0;
            return VSConstants.S_OK;
        }

        public int EnumBoundBreakpoints(out IEnumDebugBoundBreakpoints2 ppEnum)
        {
            ppEnum = new MonoBoundBreakpointEnumerator(new[] { _boundBreakpoint });
            return VSConstants.S_OK;
        }

        public int EnumErrorBreakpoints(enum_BP_ERROR_TYPE bpErrorType, out IEnumDebugErrorBreakpoints2 ppEnum)
        {
            ppEnum = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetBreakpointRequest(out IDebugBreakpointRequest2 ppBPRequest)
        {
            ppBPRequest = _pBPRequest;
            return VSConstants.S_OK;
        }

        public int GetState(PENDING_BP_STATE_INFO[] pState)
        {
            if (IsDeleted)
            {
                pState[0].state = enum_PENDING_BP_STATE.PBPS_DELETED;
            }
            else if (IsEnabled)
            {
                pState[0].state = enum_PENDING_BP_STATE.PBPS_ENABLED;
            }
            else if (!IsEnabled)
            {
                pState[0].state = enum_PENDING_BP_STATE.PBPS_DISABLED;
            }
            return VSConstants.S_OK;
        }

        public int SetCondition(BP_CONDITION bpCondition)
        {
            throw new NotImplementedException();
        }

        public int SetPassCount(BP_PASSCOUNT bpPassCount)
        {
            throw new NotImplementedException();
        }

        public int Virtualize(int fVirtualize)
        {
            return VSConstants.S_OK;
        }

        internal bool TryBind(Dictionary<string, TypeSummary> types, out MonoBreakpointLocation breakpointLocation)
        {
            try
            {
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseFile(DocumentName);
                TextLine textLine = syntaxTree.GetText().Lines[StartLine];
                Microsoft.CodeAnalysis.Location location = syntaxTree.GetLocation(textLine.Span);
                SyntaxTree sourceTree = location.SourceTree;
                var node = location.SourceTree.GetRoot().FindNode(location.SourceSpan, true, true);

                var method = GetParentMethod<MethodDeclarationSyntax>(node.Parent);
                var methodName = method.Identifier.Text;

                var cl = GetParentMethod<ClassDeclarationSyntax>(method);
                var className = cl.Identifier.Text;

                var ns = GetParentMethod<NamespaceDeclarationSyntax>(method);
                var nsname = ns.Name.ToString();

                var name = string.Format("{0}.{1}", nsname, className);
                TypeSummary summary;
                if (types.TryGetValue(name, out summary))
                {
                    var methodMirror = summary.Methods.FirstOrDefault(x => x.Name == methodName);

                    if (methodMirror != null)
                    {
                        breakpointLocation = new MonoBreakpointLocation
                        {
                            Method = methodMirror,
                            Offset = 0,
                        };
                        return true;
                    }
                }
            }
            catch
            {
            }

            breakpointLocation = null;
            return false;
        }

        private T GetParentMethod<T>(SyntaxNode node) where T : SyntaxNode
        {
            if (node == null)
                return null;

            if (node is T)
                return node as T;
            else
                return GetParentMethod<T>(node.Parent);
        }
    }
}
