using System;
using System.Runtime.InteropServices;
using Interoparating.Structures;

namespace Interoparating
{
    public class PowerManagementUtil
    {
        public const uint STATUS_SUCCESS = 0;

        [DllImport("PowrProf.dll", SetLastError = true)]
        public static extern uint CallNtPowerInformation(
            int informaitonLevel,
            IntPtr inputBuffer,
            int inputBufSize,
            IntPtr outputBuffer,
            int outputBufferSize);
        
        [DllImport("PowrProf.dll", SetLastError = true)]
        public static extern uint CallNtPowerInformation(
            int informationLevel,
            IntPtr lpInputBuffer,
            int inputBufSize,
            [Out] ProcessorPowerInformation[] lpOutputBuffer,
            int nOutputBufferSize);

        [DllImport("PowrProf.dll", SetLastError = true)]
        public static extern uint SetSuspendState(
            bool hibernate,
            bool forceCritical,
            bool disableWakeEvent);
    }
}
