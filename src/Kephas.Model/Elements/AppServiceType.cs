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

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Construction;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Classifier class for application services.
    /// </summary>
    public class AppServiceType : ClassifierBase<IAppServiceType>, IAppServiceType
    {
        /// <summary>
        /// The application service attribute.
        /// </summary>
        private readonly AppServiceContractAttribute appServiceAttribute;

        /// <summary>
        /// Context for the composition.
        /// </summary>
        private readonly ICompositionContext compositionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceType" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="appServiceAttribute">The application service attribute.</param>
        /// <param name="name">The name.</param>
        public AppServiceType(
            IModelConstructionContext constructionContext,
            AppServiceContractAttribute appServiceAttribute,
            string name)
            : base(constructionContext, name)
        {
            Requires.NotNull(appServiceAttribute, nameof(appServiceAttribute));

            this.appServiceAttribute = appServiceAttribute;
            this.compositionContext = constructionContext.CompositionContext;
            this.ContractType = this.appServiceAttribute.ContractType;
        }

        /// <summary>
        /// Gets a value indicating whether the service allows multiple service types.
        /// </summary>
        /// <value>
        /// True if allow multiple, false if not.
        /// </value>
        public bool AllowMultiple => this.appServiceAttribute.AllowMultiple;

        /// <summary>
        /// Gets a value indicating whether the service is exported as an open generic.
        /// </summary>
        /// <value>
        /// True if the service is exported as an open generic, false if not.
        /// </value>
        public bool AsOpenGeneric => this.appServiceAttribute.AsOpenGeneric;

        /// <summary>
        /// Gets the type of the contract.
        /// </summary>
        /// <value>
        /// The type of the contract.
        /// </value>
        public Type ContractType { get; private set; }

        /// <summary>
        /// Creates an instance with the provided arguments (if any).
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The new instance.</returns>
        public override object CreateInstance(IEnumerable<object> args = null)
        {
            if (this.AllowMultiple)
            {
                // TODO localization
                throw new AmbiguousMatchException($"The service {this.Name} allows multiple service implementation types.");
            }

            // TODO resolve or exception for generic services
            return this.compositionContext.GetExport(this.ContractType);
        }

        /// <summary>Adds a part to the aggregated element.</summary>
        /// <param name="part">The part to be added.</param>
        protected override void AddPart(object part)
        {
            base.AddPart(part);

            if (this.ContractType == null)
            {
                this.ContractType = (part as IRuntimeTypeInfo)?.Type;
            }
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
        protected override IClassifier ComputeBaseClassifier(IModelConstructionContext constructionContext, IEnumerable<ITypeInfo> baseTypes)
        {
            // no base classifier, only mixins.
            return null;
        }
    }
}