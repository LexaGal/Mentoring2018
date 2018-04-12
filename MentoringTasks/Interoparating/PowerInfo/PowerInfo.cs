using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using Interoparating.Structures;
using Interoparating.UnmanagedUtil;

namespace Interoparating.PowerInfo
{
    public class PowerInfo
    {
        public DateTime GetLastSleepTime()
        {
            var lastSleepTimeInTicks = GetStructure<long>(PowerInformationLevel.LastSleepTime);
            var bootUpTime = GetLastBootUpTime();
            var lastSleepTime = bootUpTime.AddTicks(lastSleepTimeInTicks);
            return lastSleepTime;
        }

        public DateTime GetLastWakeTime()
        {
            var lastWakeTimeInTicks = GetStructure<long>(PowerInformationLevel.LastWakeTime);
            var bootUpTime = GetLastBootUpTime();
            var lastWakeTime = bootUpTime.AddTicks(lastWakeTimeInTicks);
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

        private DateTime GetLastBootUpTime()
        {
            var system = new ManagementClass("Win32_OperatingSystem");
            var properies = new List<PropertyData>();

            foreach (var obj in system.GetInstances())
            {
                properies.AddRange(obj.Properties.Cast<PropertyData>());
            }
            var lasBootUp = properies.First(x => x.Name == "LastBootUpTime");
            var bootUpTime = ManagementDateTimeConverter.ToDateTime(lasBootUp.Value.ToString());
            return bootUpTime;
        }

        private T GetStructure<T>(PowerInformationLevel informationLevel)
        {
            var outputPtr = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
            var isSuccess = PowerManagementUtil.CallNtPowerInformation(
                (int) informationLevel,
                IntPtr.Zero,
                0,
                outputPtr,
                Marshal.SizeOf<T>());

            var obj = Marshal.PtrToStructure<T>(outputPtr);
            Marshal.FreeHGlobal(outputPtr);

            if (isSuccess == PowerManagementUtil.STATUS_SUCCESS)
            {
                return obj;
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
