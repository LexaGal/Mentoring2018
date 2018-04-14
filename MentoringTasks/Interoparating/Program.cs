using System;
using Interoparating.Managers;

namespace Interoparating
{
    public class Program
    {
        static void Main()
        {
            var powerInfo = new PowerInfo.PowerInfo();

            var lastSleepTime = powerInfo.GetLastSleepTime();
            Console.WriteLine($"last sleep time: {lastSleepTime}");

            var lastWakeTime = powerInfo.GetLastWakeTime();
            Console.WriteLine($"last wake time: {lastWakeTime}");

            var systemBatteryState = powerInfo.GetSystemBatteryState();
            Console.WriteLine($"max cap.: {systemBatteryState.MaxCapacity}");
            Console.WriteLine($"remaining cap.: {systemBatteryState.RemainingCapacity}");

            var systemPowerInfo = powerInfo.GetSystemPowerInformation();
            foreach (var item in systemPowerInfo)
            {
                Console.WriteLine($"Curr. Mhz: {item.CurrentMhz}");
            }

            var hibernateFileManager = new HibernateFileManager();
            hibernateFileManager.ReserveFile();
            hibernateFileManager.RemoveFile();

            //var suspendManager = new SuspendStateManager();
            //to sleep
            //suspendManager.SetSuspendState(false, true, true);
            //to hibernate
            //suspendManager.SetSuspendState(false, false, false);

            Console.ReadKey();           
        }

    }
}
