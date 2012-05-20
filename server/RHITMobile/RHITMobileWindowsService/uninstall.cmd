set PATH=%WinDir%\Microsoft.NET\Framework\v4.0.30319;%PATH%

net stop RHITMobileWindowsService
installutil /u /LogToConsole=false RHITMobile.exe
netsh http delete urlacl url=http://+:5600
netsh http delete urlacl url=https://+:5601
