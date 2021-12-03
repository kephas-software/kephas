// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using Kephas.Injection;
    using Kephas.Model.Construction;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Classifier class for application services.
    /// </summary>
    public class AppServiceType : ClassifierBase<IAppServiceType>, IAppServiceType
    {
        private readonly IAppServiceInfo appServiceInfo;
        private readonly IInjector injector;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceType" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="appServiceInfo">The application service information.</param>
        /// <param name="name">The name.</param>
        public AppServiceType(
            IModelConstructionContext constructionContext,
            IAppServiceInfo appServiceInfo,
            string name)
            : base(constructionContext, name)
        {
            appServiceInfo = appServiceInfo ?? throw new System.ArgumentNullException(nameof(appServiceInfo));

            this.appServiceInfo = appServiceInfo;
            this.injector = constructionContext.Injector;
            this.ContractType = this.appServiceInfo.ContractType;
        }

        /// <summary>
        /// Gets a value indicating whether the service allows multiple service types.
        /// </summary>
        /// <value>
        /// True if allow multiple, false if not.
        /// </value>
        public bool AllowMultiple => this.appServiceInfo.AllowMultiple;

        /// <summary>
        /// Gets a value indicating whether the service is exported as an open generic.
        /// </summary>
        /// <value>
        /// True if the service is exported as an open generic, false if not.
        /// </value>
        public bool AsOpenGeneric => this.appServiceInfo.AsOpenGeneric;

        /// <summary>
        /// Gets the type of the contract.
        /// </summary>
        /// <value>
        /// The type of the contract.
        /// </value>
        public Type? ContractType { get; private set; }

        /// <summary>
        /// Gets the instancing strategy: factory, type, or instance.
        /// </summary>
        public object? InstancingStrategy => this.appServiceInfo.InstancingStrategy;

        /// <summary>
        /// Gets the supported metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public IDictionary<string, object?>? Metadata => this.appServiceInfo.Metadata;

        /// <summary>
        /// Gets the service lifetime.
        /// </summary>
        /// <value>
        /// The service lifetime.
        /// </value>
        public AppServiceLifetime Lifetime => this.appServiceInfo.Lifetime;

        /// <summary>
        /// Adds the metadata with the provided name and value.
        /// </summary>
        /// <param name="name">The metadata name.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>This <see cref="IAppServiceInfo"/>.</returns>
        IAppServiceInfo IAppServiceInfo.AddMetadata(string name, object? value) =>
            throw new NotSupportedException("Cannot add metadata to the underlying service, instead add metadata directly to that service.");

        /// <summary>
        /// Creates an instance with the provided arguments (if any).
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The new instance.</returns>
        public override object CreateInstance(IEnumerable<object?>? args = null)
        {
            if (this.AllowMultiple)
            {
                // TODO localization
                throw new AmbiguousMatchException($"The service {this.Name} allows multiple service implementation types.");
            }

            if (this.ContractType == null)
            {
                // TODO localization
                throw new InvalidOperationException($"The service {this.Name} does not have a contract type.");
            }

            // TODO resolve or exception for generic services
            return this.injector.Resolve(this.ContractType);
        }

        /// <summary>Adds a part to the aggregated element.</summary>
        /// <param name="part">The part to be added.</param>
        protected override void AddPart(object part)
        {
            base.AddPart(part);

            this.ContractType ??= (part as IRuntimeTypeInfo)?.Type;
        }

        /// <summary>Calculates the base mixins.</summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="baseTypes">List of base types.</param>
        /// <returns>The calculated base mixins.</returns>
        protected override IEnumerable<IClassifier> ComputeBaseMixins(IModelConstructionContext constructionContext, IEnumerable<ITypeInfo> baseTypes)
        {
            // consider all base types mix-ins.
            return new ReadOnlyCollection<IClassifier>(baseTypes.OfType<IClassifier>().ToList());
        }

        /// <summary>Calculates the base classifier.</summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="baseTypes">List of base types.</param>
        /// <returns>The calculated base classifier.</returns>
        protected override IClassifier? ComputeBaseClassifier(IModelConstructionContext constructionContext, IEnumerable<ITypeInfo> baseTypes)
        {
            // no base classifier, only mixins.
            return null;
        }
    }
}