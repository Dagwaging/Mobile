set PATH=%WinDir%\Microsoft.NET\Framework\v4.0.30319;%PATH%

net stop RHITMobileService
installutil /u /LogToConsole=false RHITMobileService.exe
netsh http add urlacl url=http://+:8000/RHITSecure/service user="LOCAL SERVICE"
netsh http delete urlacl url=http://+:8000/RHITSecure/service
