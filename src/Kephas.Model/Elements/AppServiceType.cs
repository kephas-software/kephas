// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Model.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using Kephas.Diagnostics.Contracts;
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
            Requires.NotNull(appServiceInfo, nameof(appServiceInfo));

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
        /// Gets the supported metadata attributes.
        /// </summary>
        /// <value>
        /// The metadata attributes.
        /// </value>
        /// <remarks>The metadata attributes are used to register the conventions for application services.</remarks>
        public Type[]? MetadataAttributes => this.appServiceInfo.MetadataAttributes;

        /// <summary>
        /// Gets the type of the contract.
        /// </summary>
        /// <value>
        /// The type of the contract.
        /// </value>
        public Type? ContractType { get; private set; }

        /// <summary>
        /// Gets the service instance.
        /// </summary>
        /// <value>
        /// The service instance.
        /// </value>
        public object? Instance => this.appServiceInfo.Instance;

        /// <summary>
        /// Gets the type of the service instance.
        /// </summary>
        /// <value>
        /// The type of the service instance.
        /// </value>
        public Type InstanceType => this.appServiceInfo.InstanceType;

        /// <summary>
        /// Gets the service instance factory.
        /// </summary>
        /// <value>
        /// The service instance factory.
        /// </value>
        public Func<IInjector, object>? InstanceFactory => this.appServiceInfo.InstanceFactory;

        /// <summary>
        /// Gets the service lifetime.
        /// </summary>
        /// <value>
        /// The service lifetime.
        /// </value>
        public AppServiceLifetime Lifetime => this.appServiceInfo.Lifetime;

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