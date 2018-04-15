using System;
using System.Runtime.InteropServices;
using Interoparating.Structures;

namespace InteroparatingCOM
{
    [ComVisible(true)]
    [ComImport, Guid("61281354-1EC8-44C6-B686-389E47E13BED")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IPowerInfoProvider
    {
        string GetLastSleepTime();
        string GetLastWakeTime();
        string GetBatteryEstimatedTime();
        string GetSystemPowerProcsCurrMhz();
        
        string ReserveHiberFile();
        string RemoveHiberFile();

        string SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
    }
}
