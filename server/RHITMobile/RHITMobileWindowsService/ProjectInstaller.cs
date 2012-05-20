using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using NetFwTypeLib;

namespace RHITMobileWindowsService {
    [RunInstallerAttribute(true)]
    public class ProjectInstaller : Installer {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller() {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.NetworkService;
            service = new ServiceInstaller();
            service.ServiceName = RHITMobileWindowsService.SERVICE_NAME;
            service.StartType = ServiceStartMode.Automatic;
            Installers.Add(process);
            Installers.Add(service);

            AfterInstall += new InstallEventHandler(AfterInstall_ConfigureFirewall);
            AfterInstall += new InstallEventHandler(AfterInstall_StartService);
            //BeforeUninstall += new InstallEventHandler(BeforeUninstall_StopService);
        }

        public void BeforeUninstall_StopService(object sender, InstallEventArgs e) {
            try {
                using (var sc = new ServiceController(RHITMobileWindowsService.SERVICE_NAME)) {
                    if (sc.Status != ServiceControllerStatus.Stopped) {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMinutes(1));
                    }
                }
            } catch (Exception) { }
        }

        public void AfterInstall_ConfigureFirewall(object sender, InstallEventArgs e) {
            string path = service.Context.Parameters["AssemblyPath"];
            if (path == null)
                return;

            FirewallManager firewall = new FirewallManager();

            firewall.AuthorizeProgram("RHIT Mobile Service", path,
                NET_FW_SCOPE_.NET_FW_SCOPE_ALL, NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY);
        }

        public void AfterInstall_StartService(object sender, InstallEventArgs e) {
            using (var sc = new ServiceController(RHITMobileWindowsService.SERVICE_NAME)) {
                sc.Start();
                try {
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                } catch (System.ServiceProcess.TimeoutException) { }
                //if (sc.Status != ServiceControllerStatus.Running && sc.Status != ServiceControllerStatus.StartPending)
                //{
                //   throw new InvalidOperationException("Service failed to start");
                //}

                //sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(1));
                //if (sc.Status != ServiceControllerStatus.Running)
                //{
                //    throw new InvalidOperationException("Service failed to start in a reasonable amount of time");
                //}
            }
        }

        private class FirewallManager {
            private static INetFwMgr WinFirewallManager() {
                Type type = Type.GetTypeFromCLSID(
                    new Guid("{304CE942-6E39-40D8-943A-B913C40C9CD4}"));
                return Activator.CreateInstance(type) as INetFwMgr;
            }

            public void AuthorizeService(string title, string path, NET_FW_SCOPE_ scope, NET_FW_IP_VERSION_ ipver) {
                INetFwMgr mgr = WinFirewallManager();

                Type type = Type.GetTypeFromProgID("HNetCfg.FwAuthorizedApplication");
                INetFwAuthorizedApplication authapp = Activator.CreateInstance(type)
                    as INetFwAuthorizedApplication;
                authapp.Name = title;
                authapp.ProcessImageFileName = path;
                authapp.Scope = scope;
                authapp.IpVersion = ipver;
                authapp.Enabled = true;

                mgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Add(authapp);
            }

            public void AuthorizeProgram(string title, string path, NET_FW_SCOPE_ scope, NET_FW_IP_VERSION_ ipver) {
                INetFwMgr mgr = WinFirewallManager();

                Type type = Type.GetTypeFromProgID("HNetCfg.FwAuthorizedApplication");
                INetFwAuthorizedApplication authapp = Activator.CreateInstance(type)
                    as INetFwAuthorizedApplication;
                authapp.Name = title;
                authapp.ProcessImageFileName = path;
                authapp.Scope = scope;
                authapp.IpVersion = ipver;
                authapp.Enabled = true;

                mgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Add(authapp);
            }
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e) {

        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e) {

        }
    }
}
