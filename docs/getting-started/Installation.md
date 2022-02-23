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

## Application Installation

1. In Visual Studio, select Release mode configuration in the menu dropdown and rebuild the solution.
2. Look in the project's `bin` folder, where you will find `MappingEDU.1.0.0.0.nupkg`. Rename this
   file to `MappingEDU.zip` and copy to the web server.
3. Configure IIS:
   1. Unzip files into a directory such as `C:\inetpub\wwwroot\MappingEdu`.
   2. Recommended: delete the following files and directories, which are artifacts of the NuGet packaging:
      * `_rels/`
      * `package/`
      * `[Content_Types].xml`
      * `MappingEdu.nuspec`
   3. [Create a Web Site](https://docs.microsoft.com/en-us/iis/get-started/getting-started-with-iis/create-a-web-site)
      in IIS.
      * If you wish to instead use a virtual directory, then you'll need to adjust the `window['client_root']` setting
       in `index.cshtml` so that it has the virtual directory path. Ex: from `/client` to `/mappingedu/client`.
   4. The Web.config file will need updating for the server environment. Some important settings:
      * The `connectionStrings` entry for `MappingEdu` should point to the created database.
        * If IIS and SQL Server are on the same machine, you can easily use integrated security. Just provide the
          user `IIS APPPOOL\MAPPINGEDU` with `db_datareader` and `db_datawriter` permission, and you'll need to
          provide execute permissions on the stored procedures. You can add that user to the `MappingEdu_User` role.
        * Otherwise, integrated security is possible but out of scope of this documentation - recommend username
          and password. The database migration process auto-creates a `MappingEdu` user that you can use.
      * The `MappingEdu.Service.Email.*` entries should be configured for SMTP access.
      * Also review the [log4net configuration](https://logging.apache.org/log4net/release/manual/configuration.html);
        for example, you may wish to change the location of the rolling log file.
4. If using integrated security for the database connection, you need to add the application pool user
   to the MappingEDU database with read and write permissions.
   * When IIS and SQL Server are the same machine, the user will be `IIS APPPOOL\MappingEDU` (or substitute another
     name if you use an App Pool other than "MappingEDU").
   * When SQL Server is on another machine, the user will be

## Database Installation

Creating the initial MappingEdu database requires access to a SQL Server instance. You can run
the database installation from Visual Studio using the [Entity Framework command line
tool](https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/migrate-exe),
`migrate.exe`.

### Visual Studio

1. In Visual Studio, setup connection strings in the `Web.base.config` and then rebuild the project.
2. Check that the Web.UI project is the "default project" for the solution.
3. Select `Package Manager Console` from the `Tools > NuGet Package Manager` menu.
4. In the Package Manager Console window, change the Default project dropdown to `MappingEdu.Core.DataAccess`.
5. At the `PM>` prompt, run: `update-database`.

### Command Line

On the developer workstation, `migrate.exe` is in `packages\EntityFramework.6.2.0\tools`. You will want to
copy it to the bin directory so that it is side-by-side with the DataAccess assembly. In the NuGet
package described above, it is bundled into the `bin` directory. For detailed help, run `migrate.exe /?`.
Here is a sample command. Note that you _must not_ have a `.\` or `./` at the beginning of the assembly
name - doing so will cause an error.

```powershell
./migrate MappingEdu.Core.DataAccess.dll `
    /connectionString="server=(local);database=MappingEDU;integrated security=sspi" `
    /connectionProviderName=System.Data.SqlClient `
    /verbose
```

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
