﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Mono.Debugger.Soft;
using MonoDebugger.VS2013.Debugger.VisualStudio;
using NLog;
using Location = Microsoft.CodeAnalysis.Location;

namespace MonoDebugger.VS2013.Debugger
{
    internal class RoslynHelper
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        internal static StatementRange GetStatementRange(string fileName, int startLine, int startColumn)
        {
            try
            {
                logger.Trace("Line: {0} Column: {1} Source: {2}", startLine, startColumn, fileName);


                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(fileName));
                SourceText text = syntaxTree.GetText();
                var root = (CompilationUnitSyntax) syntaxTree.GetRoot();

                var span = new TextSpan(text.Lines[startLine - 1].Start + startColumn, 1);
                SyntaxNode node = root.FindNode(span, false, false);

                if (node is BlockSyntax)
                    return MapBlockSyntax(span, node);
                while (node is TypeSyntax || node is MemberAccessExpressionSyntax)
                    node = node.Parent;

                Location location = node.GetLocation();
                FileLinePositionSpan mapped = location.GetMappedLineSpan();

                return new StatementRange
                {
                    StartLine = mapped.StartLinePosition.Line,
                    StartColumn = mapped.StartLinePosition.Character,
                    EndLine = mapped.EndLinePosition.Line,
                    EndColumn = mapped.EndLinePosition.Character,
                };
            }
            catch
            {
                return null;
            }
        }

        private static StatementRange MapBlockSyntax(TextSpan span, SyntaxNode node)
        {
            var block = (BlockSyntax) node;
            bool start = Math.Abs(block.SpanStart - span.Start) < Math.Abs(block.Span.End - span.Start);

            Location location = block.GetLocation();
            FileLinePositionSpan mapped = location.GetMappedLineSpan();

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
            return new StatementRange
            {
                StartLine = mapped.EndLinePosition.Line,
                StartColumn = mapped.EndLinePosition.Character - 1,
                EndLine = mapped.EndLinePosition.Line,
                EndColumn = mapped.EndLinePosition.Character,
            };
        }

        internal static StatementRange GetILOffset(MonoPendingBreakpoint bp, MethodMirror methodMirror, out int ilOffset)
        {
            List<Mono.Debugger.Soft.Location> locations = methodMirror.Locations.ToList();

            foreach (Mono.Debugger.Soft.Location location in locations)
            {
                int line = location.LineNumber;
                int column = location.ColumnNumber;

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