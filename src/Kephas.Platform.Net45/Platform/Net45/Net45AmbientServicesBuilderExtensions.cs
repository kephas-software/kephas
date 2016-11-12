// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Net45AmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Platform.Net45
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using Kephas.Application;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class Net45AmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the .NET 4.5 application runtime to the ambient services.
        /// </summary>
        /// <exception cref="ContractException">Thrown when a method Contract has been broken.</exception>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="assemblyFilter">(Optional) a filter specifying the assembly.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithNet45AppRuntime(this AmbientServicesBuilder ambientServicesBuilder, Func<AssemblyName, bool> assemblyFilter = null)
        {
            Contract.Requires(ambientServicesBuilder != null);

            return ambientServicesBuilder.WithAppRuntime(new Net45AppRuntime(assemblyFilter: assemblyFilter));
        }
    }
}