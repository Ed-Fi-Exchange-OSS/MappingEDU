// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("MappingEdu Data Access Library")]
[assembly: AssemblyDescription("Data access library for the application")]
[assembly: AssemblyCompany("Ed-Fi Alliance")]
[assembly: AssemblyProduct("MappingEdu")]
[assembly: AssemblyCopyright("Copyright © 2016 Ed-Fi Alliance")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#if (Debug || DEBUG)

[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("7C3215CD-58B4-4344-95C6-5A1F1BD2EE27")]

// CLS Compliant

[assembly: CLSCompliant(false)]