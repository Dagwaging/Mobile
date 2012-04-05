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
            process.Account = ServiceAccount.NetworkService;
            service = new ServiceInstaller();
            service.ServiceName = WindowsService.SERVICE_NAME;
            service.StartType = ServiceStartMode.Automatic;
            Installers.Add(process);
            Installers.Add(service);

            AfterInstall += new InstallEventHandler(AfterInstall_StartService);
            BeforeUninstall += new InstallEventHandler(BeforeUninstall_StopService);

            //POST BUILD NOTES (from an Administrative VS2010 command prompt!):

            //Ensure that everyone has full control of the output directory (bin\Debug)

            //Stop the service
            //$ net stop RHITMobileService

            //Uninstall the old app
            //$installutil /u RHITMobileService.exe

            //Install the app:
            //$installutil RHITMobileService.exe

            //Start the service
            //$ net start RHITMobileService
        }

        public void BeforeUninstall_StopService(object sender, InstallEventArgs e)
        {
            using (var sc = new ServiceController(WindowsService.SERVICE_NAME))
            {
                if (sc.Status != ServiceControllerStatus.Stopped)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMinutes(1));
                }
            }
        }

        public void AfterInstall_StartService(object sender, InstallEventArgs e)
        {
            using (var sc = new ServiceController(WindowsService.SERVICE_NAME))
            {
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                if (sc.Status != ServiceControllerStatus.Running && sc.Status != ServiceControllerStatus.StartPending)
                {
                    throw new InvalidOperationException("Service failed to start");
                }

                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(1));
                if (sc.Status != ServiceControllerStatus.Running)
                {
                    throw new InvalidOperationException("Service failed to start in a reasonable amount of time");
                }
            }
        }
    }
}
