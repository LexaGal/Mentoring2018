using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Interoparating.PowerInfo;
using Interoparating.UnmanagedUtil;

namespace Interoparating.Managers
{
    public class HibernateFileManager
    {
        public void RemoveFile()
        {
            ApplyHibernateAction(HibernateFileAction.Remove);
        }

        public void ReserveFile()
        {
            ApplyHibernateAction(HibernateFileAction.Reserve);
        }
        
        private void ApplyHibernateAction(HibernateFileAction action)
        {
            var byteSize = Marshal.SizeOf<byte>();
            var bytePtr = Marshal.AllocHGlobal(byteSize);
            Marshal.WriteByte(bytePtr, (byte) action);

            var isSuccess = PowerManagementUtil.CallNtPowerInformation(
                (int) PowerInformationLevel.SystemReserveHiberFile,
                bytePtr,
                byteSize,
                IntPtr.Zero,
                0);

            Marshal.FreeHGlobal(bytePtr);

            if (isSuccess != PowerManagementUtil.STATUS_SUCCESS)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());            
            }           
        }
    }   
}
