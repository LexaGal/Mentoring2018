$pip = New-Object -com InteroparatingCOM.PowerInfoProvider
$pip.GetLastSleepTime()
$pip.GetLastWakeTime()
$pip.GetBatteryEstimatedTime()
$pip.GetSystemPowerProcsCurrMhz()
$pip.ReserveHiberFile()
$pip.RemoveHiberFile()
#$pip.SetSuspendState(0, 1, 1)