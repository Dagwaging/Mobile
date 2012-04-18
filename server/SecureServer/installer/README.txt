
# RHIT Mobile Secure Server

## Database Configuration

Included with this readme is a sql script (BannerDataCreationScript.sql) capable
of populating a database with the necessary tables and stored procedures for
running the mobile server.  Note that the script assumes it is operating on a
database called "BannerData".  Change the first line of the file as necessary to
reflect the actual database name being used.

## Installation Instructions

 - Included with this readme is an installer (SecureMobileServerInstaller.msi).
   Begin by installing the service using the installer.  The installer will
   attempt to start the service and will setup the necessary exceptions in the
   Windows firewall.

 - Configure the connection string for the database and the path to read the
   banner data from.  The configuration file is located in the application's
   installation directory: 
   C:\Program Files (x86)\Rose-Hulman\Secure Mobile Server\RHITMobileService.exe.config

   The path that the service will import data from is located in the appSettings
   section.  Replace the value specified in the "value" field with the
   appropriate path to the data files.  This path may include network shares.
   If a network share is used, the *machine account* must have read access to
   the remote directory.

   The connection string is located in the connectionStrings section.  A
   connection string is simply a semicolon separated list of parameters
   necessary to connect to the database.  Relevant fields include:

    - Data Source: The address of the database server.  Connecting via TCP is of
      the form `tcp:address,port`
    
    - Initial Catalog: The name of the database to use.

    - User Id: If the machine account cannot be used for authentication, a
      username must be specified here.
    
    - Password: If the machine account cannot be used for authentication, a
      password must be specified here.

 - In order for configuration changes to take effect, the service needs to be
   restarted, either through `services.msc` or via the command line:

       $ net stop RHITMobileSecureServer
       $ net start RHITMobileSecureServer

 - After configuring the service, it is recommended to set the service to
   automatically restart on failure.  This can be configured through
   `services.msc`

    - Open the properties for RHITMobileSecureServer
    - Click the **Recovery** tab
    - For each failure type, select "Restart the Service"
    - Click OK

## Monitoring

The status of the server can be viewed through the window event viewer
(`eventvwr.msc`) under the source name RHITMobileSecureServer.

