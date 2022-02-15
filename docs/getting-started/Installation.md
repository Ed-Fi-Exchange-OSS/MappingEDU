# Server Installation

This documentation describes how to install the MappingEDU system on a test or production server.

## Audience

This documentation is for technical professionals with DevOps experience in a Windows environment. They should have
some knowledge of SQL Server, Visual Studio, and IIS.

## Prerequisites

A Windows 2019 Server environment with the following preinstalled:

* SQL Server 2016+
* .NET Framework 4.8

A Windows workstation environment with the following preinstalled:

* SQL Server 2016+
* Visual Studio 2019+
* .NET Framework 4.8

Access to a SMTP server is required to handle account creation and password reset emails.

## Setup

1. From the workstation, download and extract a zip file of the `main` branch of this repository.
1. Run Visual Studio and open the solution file `src/MappingEdu.sln`.
1. Build the solution in Visual Studio.
   * NuGet packages may need to be restored first. Right-click on the solution in Solution Explorer and choose
     `Restore NuGet Packages`.

## Database Installation

Creating the initial MappingEdu database requires access to a SQL Server instance. If the remote server is not
accessible from the workstation or the developer does not have access rights, then follow the process below on
the workstation, make a backup of the database, and restore that backup on the remote server.

1. Create a new SQL Server database. `MappingEdu` is a recommended name.
1. In Visual Studio, find the `MappingEdu.Core.DataAccess` project and modify the `connectionStrings` section of
   the `App.config` file so that entries point to the created database.
1. Select `Package Manager Console` from the `Tools > NuGet Package Manager` menu.
1. In the Package Manager Console window, change the Default project dropdown to `MappingEdu.Core.DataAccess`.
1. At the `PM>` prompt, run this command: `update-database`.

## Application Installation

1. In Visual Studio, select Release mode configuration in the menu dropdown and rebuild the solution.
1. Right-click on the `MappingEdu.Web.UI` project, choose `Publish`. The application can be published directly to
   an existing IIS site or published to a folder for manual deployment.
   * For manual deployment, place the folder of "published" files into a remote directory, e.g.
     `C:\inetpub\wwwroot\MappingEdu` on the web server, and
     [create an application](https://docs.microsoft.com/en-us/iis/configuration/system.applicationhost/sites/site/application/)
     in IIS.
   * The Web.config file will need updating for the server environment. Some important settings:
     * The `connectionStrings` entry for `MappingEdu` should point to the created database.
     * The `MappingEdu.Service.Email.*` entries should be configured for SMTP access.

## Default Logins

There are two built-in users available with a fresh database installation:
   * Administrator
     * Email: `admin@example.com`
     * Password: `password`
   * User
     * Email: `user@example.com`
     * Password: `password`

❗ After an initial login in with the Administrator account, it is strongly recommended that a new admin account be
created and the built-in accounts deleted.
