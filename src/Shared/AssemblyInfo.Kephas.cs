// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.Kephas.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   AssemblyInfo.Kephas.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyCompany("Quartz Software SRL")]
[assembly: AssemblyProduct("Kephas Framework")]
[assembly: AssemblyCopyright("Copyright © Quartz Software SRL 2010-2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

[assembly: AssemblyVersion("4.2.0.0")]
[assembly: AssemblyFileVersion("4.2.0.0")]
[assembly: AssemblyInformationalVersion("4.2.0.0")]

// Stylecop global rule suppresions.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1028:Code must not contain trailing whitespace", Justification = "It is OK to leave the trailing spaces.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single class", Justification = "Generics with same name can stay in the same file.")]