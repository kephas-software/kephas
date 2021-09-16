// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefPartConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Conventions builder for a specific part.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition.Mef.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Injection.Conventions;

    /// <summary>
    /// Conventions builder for a specific part.
    /// </summary>
    public class MefPartConventionsBuilder : IPartConventionsBuilder
    {
        /// <summary>
        /// The inner convention builder.
        /// </summary>
        private readonly PartConventionBuilder innerConventionBuilder;

        private Type contractType;
        private Type serviceType;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefPartConventionsBuilder"/> class.
        /// </summary>
        /// <param name="innerConventionBuilder">The inner convention builder.</param>
        internal MefPartConventionsBuilder(PartConventionBuilder innerConventionBuilder)
        {
            Requires.NotNull(innerConventionBuilder, nameof(innerConventionBuilder));

            this.innerConventionBuilder = innerConventionBuilder;
        }

        /// <summary>
        /// Indicates the declared service type. Typically this is the same as the contract type, but
        /// this may get overwritten, for example when declaring generic type services for collecting
        /// metadata.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartConventionsBuilder AsServiceType(Type serviceType)
        {
            Requires.NotNull(serviceType, nameof(serviceType));

            this.serviceType = serviceType;
            return this;
        }

        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public IPartConventionsBuilder Singleton()
        {
            this.innerConventionBuilder.Shared();
            return this;
        }

        /// <summary>
        /// Mark the part as being shared within the scope.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartConventionsBuilder Scoped()
        {
            this.innerConventionBuilder.Shared(InjectionScopeNames.Default);
            return this;
        }

        /// <summary>
        /// Exports the part using the specified conventions builder.
        /// </summary>
        /// <param name="conventionsBuilder">The conventions builder.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public IPartConventionsBuilder Export(Action<IExportConventionsBuilder>? conventionsBuilder = null)
        {
            if (conventionsBuilder == null)
            {
                this.innerConventionBuilder.Export();
            }
            else
            {
                this.innerConventionBuilder.Export(b => conventionsBuilder(new MefExportConventionsBuilder(b)));
            }

            return this;
        }

        /// <summary>
        /// Select the interface on the part type that will be exported.
        /// </summary>
        /// <param name="exportInterface">The interface to export.</param>
        /// <param name="exportConfiguration">The export configuration.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartConventionsBuilder ExportInterface(
            Type exportInterface,
            Action<Type, IExportConventionsBuilder>? exportConfiguration = null)
        {
            Requires.NotNull(exportInterface, nameof(exportInterface));

            this.contractType = exportInterface;

            var interfaceFilter = exportInterface.IsGenericTypeDefinition
                                      ? (Predicate<Type>)(t => this.IsClosedGenericOf(exportInterface, t))
                                      : t => ReferenceEquals(exportInterface, t);

            if (exportConfiguration == null)
            {
                this.innerConventionBuilder.ExportInterfaces(interfaceFilter);
            }
            else
            {
                this.innerConventionBuilder.ExportInterfaces(
                    interfaceFilter,
                    (t, builder) => exportConfiguration(t, new MefExportConventionsBuilder(builder)));
            }

            return this;
        }

        /// <summary>
        /// Select which of the available constructors will be used to instantiate the part.
        /// </summary>
        /// <param name="constructorSelector">Filter that selects a single constructor.</param><param name="importConfiguration">Action configuring the parameters of the selected constructor.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartConventionsBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector, Action<ParameterInfo, IImportConventionsBuilder>? importConfiguration = null)
        {
            Requires.NotNull(constructorSelector, nameof(constructorSelector));

            ConstructorInfo NewConstructorSelector(IEnumerable<ConstructorInfo> ctorInfos)
            {
                var ctor = constructorSelector(ctorInfos);

                if (ctor == null)
                {
                    var constructorsList = ctorInfos.Where(c => !c.IsStatic && c.IsPublic).ToList();

                    if (constructorsList.Count == 0)
                    {
                        throw new InjectionException(string.Format(Strings.MefPartConventionsBuilder_MissingCompositionConstructor, ctorInfos.FirstOrDefault()?.DeclaringType?.ToString() ?? "<unknown>", this.contractType));
                    }

                    if (constructorsList.Count == 1)
                    {
                        return constructorsList[0];
                    }

                    var sortedConstructors = constructorsList.ToDictionary(c => c, c => c.GetParameters().Length).OrderByDescending(kv => kv.Value).ToList();
                    if (sortedConstructors[0].Value == sortedConstructors[1].Value)
                    {
                        throw new InjectionException(string.Format(Strings.MefPartConventionsBuilder_AmbiguousCompositionConstructor, constructorsList.First().DeclaringType, typeof(InjectionConstructorAttribute)));
                    }

                    return sortedConstructors[0].Key;
                }

                return ctor;
            }

            if (importConfiguration == null)
            {
                this.innerConventionBuilder.SelectConstructor(NewConstructorSelector);
            }
            else
            {
                this.innerConventionBuilder.SelectConstructor(
                    NewConstructorSelector,
                    (pi, config) => importConfiguration(pi, new MefImportConventionsBuilder(config)));
            }

            return this;
        }

        /// <summary>
        /// Indicates that this service allows multiple registrations.
        /// </summary>
        /// <param name="value">True if multiple service registrations are allowed, false otherwise.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartConventionsBuilder IPartConventionsBuilder.AllowMultiple(bool value)
        {
            // not used, by default all services allow multiple instances.
            return this;
        }

        /// <summary>
        /// Determines whether the provided interface is a closed generic of the specified open generic contract.
        /// </summary>
        /// <param name="openGenericContract">The open generic contract.</param>
        /// <param name="exportInterface">The export interface.</param>
        /// <returns><c>true</c> if the provided interface is a closed generic of the specified open generic contract, otherwise <c>false</c>.</returns>
        private bool IsClosedGenericOf(Type openGenericContract, Type exportInterface)
        {
            return exportInterface.IsGenericType && exportInterface.GetGenericTypeDefinition() == openGenericContract;
        }
    }
}