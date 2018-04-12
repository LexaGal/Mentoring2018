﻿using System.Runtime.InteropServices;

namespace Interoparating.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessorPowerInformation
    {
        public uint Number;
        public uint MaxMhz;
        public uint CurrentMhz;
        public uint MhzLimit;
        public uint MaxIdleState;
        public uint CurrentIdleState;
    }
} 
