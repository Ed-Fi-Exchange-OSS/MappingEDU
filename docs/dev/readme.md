# Developer On-boarding

- [Developer On-boarding](#developer-on-boarding)
  - [Development Environment](#development-environment)
  - [Web Development Resources](#web-development-resources)
  - [Database Installation](#database-installation)
  - [Running the Application on Localhost](#running-the-application-on-localhost)
  - [Running the Tests in MappingEdu.Tests.DataAccess](#running-the-tests-in-mappingedutestsdataaccess)

## Development Environment

* Visual Studio 2019
  * The VS2019 TypeScript support might need some configuration help; as of
    2021-04-02 it is flagging a lot of ESLint errors that may be due to linting
    on a newer version of TypeScript than was used for MappingEDU development.
  * Visual Studio 2022 cannot run database installation from the Package Manager Console,
    due to [this known issue](https://github.com/dotnet/ef6/issues/1916).
* SQL Server 2014 or newer
* .NET Framework 4.5 sdk
* Node.js
* (optional) [AngularJS Batarang
  extension](https://chrome.google.com/webstore/detail/angularjs-batarang/ighdmehidhipcmcojjgiloacoafjmpfk)
  for Chrome

## Web Development Resources

* Layout is using [Bootstrap
  3.3.4](https://bootstrapdocs.com/v3.3.4/docs/getting-started/)
* SPA library is [AngularJS 1.8](https://angular.io/docs)
* Other libraries and components currently loaded in the application:
  * [Angular Bootstrap Scrolling
    Tabs](https://github.com/mikejacobson/angular-bootstrap-scrolling-tabs)
    0.0.30
  * [Angular UI](http://github.com/angular-ui) (UI components)
    * [angular-spinner](https://github.com/urish/angular-spinner) 0.8.0
    * [ui-select](http://github.com/angular-ui/ui-select) 0.19.3
    * [uiSwitch](https://github.com/xpepermint/angular-ui-switch) unknown
      version (iOS-style toggle control)
  * [angular-vs-repeater](http://github.com/kamilkp) 1.0.0-rc12
  * [Angulartics](http://luisfarzati.github.io/angulartics) 0.17.0 (website
    analytics with [Application Insights
    integration](https://stevenfollis.com/2015/03/10/using-application-insights-with-angularjs-applications/))
  * Application Insights
  * [Chart.js](http://chartjs.org/) 1.1.1
  * [DataTables for Bootstrap3](http://datatables.net/manual/styling/bootstrap)
  * [FileAPI](https://github.com/mailru/FileAPI) 2.0.7 (tools for working with
    files)
  * [fixed-table-header](https://github.com/daniel-nagy/fixed-table-header)
    unknown version
  * [jQuery](http://jquery.com/) 2.1.4
  * [ng-file-upload](https://github.com/danialfarid/ng-file-upload) 12.2.13 or
    5.1.0 * there are two files with different version numbers
  * [ngprogress](https://github.com/VictorBjelkholm/ngProgress) 1.1.2 (slim,
    site-wide progressbar for AngularJS)
  * perfect-scrollbar 0.6.11
  * [RestAngular](https://github.com/mgonto/restangular) 1.4.0 (REST client)
  * [spin.js](https://spin.js.org/) unknown version
    (content-is-loading-please-wait spinner; used by angular-spinner)
  * [textAngular](http://textangular.com/) 1.5.1 (WYSIWYG Text Editor)
  * Toastr unknown version (user notifications)
  * [Underscore](https://underscorejs.org/) 1.8.2 (various functional utilities)

## Database Installation

1. Build the solution in Visual Studio
   * Might need to right-click on the solution in Solution Explorer and choose
     "Restore NuGet Packages" first.
2. Open the Package Manager Console from the `Tools > NuGet Package Manager`
   menu.
3. In the Package Manager Console window, change the default project to
   `MappingEdu.Core.DataAccess`.
4. Run this command: `update-database`.

## Running the Application on Localhost

1. In Visual Studio, right click on `MappingEdu.Web.UI` in Solution Explorer and
   choose "Set as Startup Project".
2. F5 to start the application.
3. This will launch the application using IIS Express without TLS/SSL
   certificate. If you see an SSL protocol error in the browser, it probably
   means that you previously ran a web application with HSTS installed and using
   a developer certificate. Now the browser is throwing an error if you try to
   access HTTP instead of HTTPS. Clearing the browser cache will probably fix
   that. Alternately, just leave the error window open (if you close it, Visual
   Studio will detect that and stop the debug session), and paste the URL (as
   "http" instead of "https") into a browser private-mode window.
4. Built-in users (available both with the fresh database or the staging backup):
   * Administrator
     * Email: `admin@example.com`
     * Password: `password`
   * User
     * Email: `user@example.com`
     * Password: `password`

## Running the Tests in MappingEdu.Tests.DataAccess

These tests are integrated with the database. The default config file points to
a database called `MappingEdu-Tests` so that there is no impact on the machine's
main `MappingEdu` database.

1. Create the new database manually in your localhost
1. Temporarily change `web.base.config` in the Web.UI project, modifying the
   connection string to use the database named `MappingEdu-Tests`.
1. Follow the [Fresh Database](#fresh-database) instructions above, substituting
   in the the correct database name.
