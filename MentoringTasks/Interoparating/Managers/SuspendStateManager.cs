using System.ComponentModel;
using System.Runtime.InteropServices;
using Interoparating.UnmanagedUtil;

namespace Interoparating.Managers
{
    public class SuspendStateManager
    {
        public void SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent)
        {
            var isSuccess = PowerManagementUtil.SetSuspendState(hibernate, forceCritical, disableWakeEvent);
            if (isSuccess == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }
}
