# Octopus Deploy

These are quick notes on the Ed-Fi Octopus Deploy project. The Alliance is using
an older version without good support for import/export, so these instructions
are the best we can provide.

1. Create a new Project called `MappingEdu-OSS`
2. Setup the following Process:
   1. Multi-step deployment named "Deploy Web Application"
   2. Add child step: "Deploy Web Application":
      * Package: MappingEdu.Web.UI from Team City
      * Configuration Transforms: default
      * Configuration Variables: replace entries in config files
      * IIS Deployment Type: IIS Web Site
      * Web site:
        * YES: create or update an IIS Web Site and Application Pool
        * Web site name: `MappingEDU_#{Octopus.Environment.Name}`
        * Physical Path: Package installation directory
        * YES: start IIS Web site
      * Application Pool:
        * Pool name: `MappingEDU_#{Octopus.Environment.Name}`
        * .NET CLR version: v4.0
        * Identity: Application Pool Identity
        * YES: Start IIS Application Pool
      * Bindings:
        * Protocol: https
        * Port: 443
        * IP Address: *
        * Host: `#{MappingEdu_HostName}`
        * SSL certificate thumbprint: `{SslCertThumbPrint}`
      * IIS Authentication: anonymous
      * Environments: all applicable Lifecycle environments
   3. Add Powershell step named "Deploy Database":
      * Script source: defined below
      * Script content:

        ```pwsh
Write-Host "Migrating database using connection string: <"$DBAdmin">"

\# Get the exe name based on the directory
$contentPath  = (Join-Path $OctopusParameters['Octopus.Action[Deploy Web Application].Output.Package.InstallationDirectoryPath'] "bin")
$fullPath = (Join-Path $contentPath "migrate.exe")
Write-Host "Using migrate.exe path:" $fullPath

cd $contentPath
write-host "Using working dir: "$(get-location)

\# Run the migration utility
& ".\migrate.exe" MappingEdu.Core.DataAccess.dll /connectionString=$DBAdmin /connectionProviderName="System.Data.SqlClient" /verbose | Write-Host

\# Run the migration utility again to force seeding
& ".\migrate.exe" MappingEdu.Core.DataAccess.dll /connectionString=$DBAdmin /connectionProviderName="System.Data.SqlClient" /verbose | Write-Host
          ```

      * Environments: all applicable Lifecycle environments
3. Variables (fill in values as appropriate)
   *  `DatabaseName`
   *  `DatabaseServer`
   *  `DBAdmin` = `#{MappingEdu}`
   *  `MappingEdu` = `data source=#{DatabaseServer};initial catalog=#{DatabaseName};persist security info=True;user id=#{MappingEduUser};password=#{MappingEduPwd};multipleactiveresultsets=True;`
   *  `MappingEdu_HostName` (for example, `mappingedu.ed-fi.org`)
   *  `MappingEdu.DataAccess.Cache.AbsoluteMinutes` = `10`
   *  `MappingEdu.DataAccess.Cache.Enabled` = `False`
   *  `MappingEdu.DataAccess.Cache.SlidingExpiration` = `0`
   *  `MappingEdu.Service.Email.DisableSSLCertificateCheck` = `True` or `False`
   *  `MappingEdu.Service.Email.EnableSSL` = `True` or `False`
   *  `MappingEdu.Service.Email.From`
   *  `MappingEdu.Service.Email.Password`
   *  `MappingEdu.Service.Email.Port`
   *  `MappingEdu.Service.Email.ServerAddress`
   *  `MappingEdu.Service.Email.Username`
   *  `MappingEdu.Telemetry.Instrumentation.Enabled` = `True` or `False`
   *  `MappingEdu.Version` = `#{Octopus.Release.Number}`
   *  `MappingEduPwd`
   *  `MappingEduUser`
   *  `MigrateDatabase` = `True`
   *  `SslCertThumbPrint` = the string thumbprint from the certificate that is available to IIS
   *  `SslEnabled` = `True` or `False`
