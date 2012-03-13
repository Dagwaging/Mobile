using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace RHITMobile.Secure
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalService;
            service = new ServiceInstaller();
            service.ServiceName = WindowsService.SERVICE_NAME;
            service.StartType = ServiceStartMode.Automatic;
            Installers.Add(process);
            Installers.Add(service);

            //POST BUILD NOTES (from an Administrative VS2010 command prompt!):

            //Ensure that everyone has full control of the output directory (bin\Debug)

            //Reserve our URL:
            //$ netsh http add urlacl url=http://+:8000/RHITSecure/service user="LOCAL SERVICE"

            //Stop the service
            //$ net stop RHITMobileService

            //Uninstall the old app
            //$installutil /u RHITMobileService.exe

            //Install the app:
            //$installutil RHITMobileService.exe

            //Start the service
            //$ net start RHITMobileService
        }
    }
}
