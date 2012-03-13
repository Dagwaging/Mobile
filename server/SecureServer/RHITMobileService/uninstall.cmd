set PATH=%WinDir%\Microsoft.NET\Framework\v4.0.30319;%PATH%

net stop RHITMobileService
installutil /u RHITMobileService.exe
netsh http delete urlacl url=http://+:8000/RHITSecure/service