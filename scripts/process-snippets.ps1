#to stop processes, this script may need to be run as administrator, but the ProcessID can be found throught TaskManager Details


#PowerShell to get process IDs of all Excel instances (including the those that might only show up under "Details" of Task Manager

#to find/kill all Excel processes
Get-Process Excel
#Get-Process Excel | Stop-Process

#to find a specific excel instance with a particular command line
Get-WmiObject Win32_Process -Filter "name like '%excel%' and commandline like '%JniPMML%' " | select commandline

$localp=Get-WmiObject -Class Win32_Process -Filter "name like '%excel%' and commandline like '%JniPMML%' "
$localpid=$localp.ProcessID
Write-Host $localpid
#Stop-Process -Id $localpid

