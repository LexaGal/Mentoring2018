using System;
using System.Linq;
using System.Runtime.InteropServices;
using Interoparating.Managers;
using Interoparating.PowerInfo;
using Interoparating.Structures;

namespace InteroparatingCOM
{
    [ComVisible(true)]
    [Guid("A8C431FA-DC49-403B-B30D-CD0E08995FD3")]
    [ClassInterface(ClassInterfaceType.None)]
    public class PowerInfoProvider : IPowerInfoProvider
    {
        private PowerInfo _powerInfo;
        private SuspendStateManager _suspendStateManager;
        private HibernateFileManager _hibernateFileManager;

        public PowerInfoProvider()
        {
            _powerInfo = new PowerInfo();
            _suspendStateManager = new SuspendStateManager();
            _hibernateFileManager = new HibernateFileManager();
        }

        public string GetLastSleepTime()
        {
            return $"Sleep: {_powerInfo.GetLastSleepTime()}";
        }

        public string GetLastWakeTime()
        {
            return $"Wake: {_powerInfo.GetLastWakeTime()}";
        }

        public string GetBatteryEstimatedTime()
        {
            return $"Estimated batt. min. time: {new TimeSpan(_powerInfo.GetSystemBatteryState().EstimatedTime).Minutes}";
        }

        public string GetSystemPowerProcsCurrMhz()
        {
            return $"Curr. Mhz:\n{string.Join("\n", _powerInfo.GetSystemPowerInformation().Select(i => i.CurrentMhz))}";
        }

        public string ReserveHiberFile()
        {
            _hibernateFileManager.ReserveFile();
            return "File reserved";
        }

        public string RemoveHiberFile()
        {
            _hibernateFileManager.RemoveFile();
            return "File removed";
        }

        public string SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent)
        {
            _suspendStateManager.SetSuspendState(hibernate, forceCritical, disableWakeEvent);
            return "Went to sleep";
        }
    }
}
