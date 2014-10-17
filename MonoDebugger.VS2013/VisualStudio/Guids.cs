// Guids.cs
// MUST match guids.h

using System;

namespace MonoDebugger.VS2013
{
    internal static class GuidList
    {
        public const string guidMonoDebugger_VS2013PkgString = "15538f63-0557-4a8c-8ed4-a842d6f6f4db";
        public const string guidMonoDebugger_VS2013CmdSetString = "7951fdf3-b04d-41d6-9917-3fa9d554cdcd";

        public static readonly Guid guidMonoDebugger_VS2013CmdSet = new Guid(guidMonoDebugger_VS2013CmdSetString);
    };
}