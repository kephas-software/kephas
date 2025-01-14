// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Serilog.Exceptions.Destructurers;

[assembly: DeclareTransientServiceFamily(typeof(IExceptionDestructurer))]
