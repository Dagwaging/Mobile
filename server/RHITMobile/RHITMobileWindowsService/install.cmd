set PATH=%WinDir%\Microsoft.NET\Framework\v4.0.30319;%PATH%

netsh http add urlacl url=http://+:5600 user="network service" listen=yes
netsh http add urlacl url=https://+:5601 user="network service" listen=yes
installutil RHITMobile.exe
net start RHITMobileWindowsService