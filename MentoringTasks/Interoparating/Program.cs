using System;

namespace Interoparating
{
    public class Program
    {
        static void Main()
        {
            var powerInfo = new PowerInfo();
            
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

            Console.ReadKey();           
        }

    }
}
