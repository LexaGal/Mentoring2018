using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Interoparating.Structures;
using Interoparating.UnmanagedUtil;

namespace Interoparating.PowerInfo
{
    public class PowerInfo
    {
        public string GetLastSleepTime()
        {
            var lastSleepTimeInTicks = GetStructure<long>(PowerInformationLevel.LastSleepTime);
            var lastSleepTime = new DateTime(lastSleepTimeInTicks).ToLongTimeString();
            return lastSleepTime;
        }

        public string GetLastWakeTime()
        {
            var lastWakeTimeInTicks = GetStructure<long>(PowerInformationLevel.LastWakeTime);
            var lastWakeTime = new DateTime(lastWakeTimeInTicks).ToLongTimeString();
            return lastWakeTime;
        }
        
        public SystemBatteryState GetSystemBatteryState()
        {
            var batteryState = GetStructure<SystemBatteryState>(PowerInformationLevel.SystemBatteryState);
            return batteryState;        
        }

        public ProcessorPowerInformation[] GetSystemPowerInformation()
        {
            var nProcessors = Environment.ProcessorCount;
            var procPowerInfo = new ProcessorPowerInformation[nProcessors];
            var isSuccess = PowerManagementUtil.CallNtPowerInformation(
                (int) PowerInformationLevel.ProcessorInformation,
                IntPtr.Zero,
                0,
                procPowerInfo,
                procPowerInfo.Length*Marshal.SizeOf(typeof(ProcessorPowerInformation)));

            if (isSuccess == PowerManagementUtil.STATUS_SUCCESS)
            {
                return procPowerInfo;
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        
        private T GetStructure<T>(PowerInformationLevel informationLevel)
        {
            T obj;
            var outputPtr = IntPtr.Zero;
            uint isSuccess;
            try
            {
               outputPtr = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
               isSuccess = PowerManagementUtil.CallNtPowerInformation(
                    (int) informationLevel,
                    IntPtr.Zero,
                    0,
                    outputPtr,
                    Marshal.SizeOf<T>());

                obj = Marshal.PtrToStructure<T>(outputPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(outputPtr);
            }
            if (isSuccess == PowerManagementUtil.STATUS_SUCCESS)
            {
                return obj;
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
