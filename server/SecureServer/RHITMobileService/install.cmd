set PATH=%WinDir%\Microsoft.NET\Framework\v4.0.30319;%PATH%

netsh http add urlacl url=http://+:8000/RHITSecure/service user="LOCAL SERVICE"
installutil RHITMobileService.exe
net start RHITMobileService