using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Mono.Debugger.Soft;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.VS2013.Debugger
{
    class RoslynHelper
    {
        internal static StatementRange GetStatementRange(string fileName, int startLine, int startColumn)
        {
            try
            {
                Trace.WriteLine(string.Format("Line: {0} Column: {1} Source: {2}", startLine, startColumn, fileName));


                var syntaxTree = CSharpSyntaxTree.ParseFile(fileName);
                SourceText text = syntaxTree.GetText();
                var root = (CompilationUnitSyntax)syntaxTree.GetRoot();

                var span = new TextSpan(text.Lines[startLine - 1].Start + startColumn, 1);
                var node = root.FindNode(span, false, false);

                if (node is BlockSyntax)
                    return MapBlockSyntax(span, node);
                else
                {
                    while (node is TypeSyntax || node is MemberAccessExpressionSyntax)
                        node = node.Parent;

                    var location = node.GetLocation();
                    var mapped = location.GetMappedLineSpan();

                    return new StatementRange
                    {
                        StartLine = mapped.StartLinePosition.Line,
                        StartColumn = mapped.StartLinePosition.Character,
                        EndLine = mapped.EndLinePosition.Line,
                        EndColumn = mapped.EndLinePosition.Character,
                    };
                }
            }
            catch 
            {
                return null;
            }
        }

        private static StatementRange MapBlockSyntax(TextSpan span, SyntaxNode node)
        {
            var block = (BlockSyntax)node;
            bool start = Math.Abs(block.SpanStart - span.Start) < Math.Abs(block.Span.End - span.Start);

            var location = block.GetLocation();
            var mapped = location.GetMappedLineSpan();

            if (start)
            {
                return new StatementRange
                {
                    StartLine = mapped.StartLinePosition.Line,
                    StartColumn = mapped.StartLinePosition.Character,
                    EndLine = mapped.StartLinePosition.Line,
                    EndColumn = mapped.StartLinePosition.Character + 1,
                };
            }
            else
            {
                return new StatementRange
                {
                    StartLine = mapped.EndLinePosition.Line,
                    StartColumn = mapped.EndLinePosition.Character - 1,
                    EndLine = mapped.EndLinePosition.Line,
                    EndColumn = mapped.EndLinePosition.Character,
                };
            }
        }

        internal static StatementRange GetILOffset(MonoPendingBreakpoint bp, MethodMirror methodMirror, out int ilOffset)
        {
            var locations = methodMirror.Locations.ToList();

            foreach (var location in locations)
            {
                var line = location.LineNumber;
                var column = location.ColumnNumber;

                if (line != bp.StartLine + 1)
                    continue;
                //if (column != bp.StartColumn)
                //    continue;

                ilOffset = location.ILOffset;

                Console.WriteLine(location.ColumnNumber);
                return null;
            }

            throw new Exception("Cant bind breakpoint");
        }
    }
}
