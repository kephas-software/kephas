// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceContractAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Marks an interface to be an application service.
//   Application services are automatically identified by the composition
//   and added to the container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Marks an interface to be an application service contract.
    /// Application services are automatically identified by the composition
    /// and added to the container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppServiceContractAttribute : Attribute, IAppServiceInfo
    {
        /// <summary>
        /// The default metadata attribute types.
        /// </summary>
        public static readonly IReadOnlyCollection<Type> DefaultMetadataAttributeTypes;

        /// <summary>
        /// The empty metadata attribute types.
        /// </summary>
        public static readonly IReadOnlyCollection<Type> EmptyMetadataAttributeTypes;

        /// <summary>
        /// The default metadata attribute types.
        /// </summary>
        private static readonly IList<Type> WritableDefaultMetadataAttributeTypes
            = new List<Type>
                  {
                      typeof(ProcessingPriorityAttribute),
                      typeof(OverridePriorityAttribute),
                      typeof(OptionalServiceAttribute),
                      typeof(ServiceNameAttribute),
                  };

        /// <summary>
        /// Name of the scope.
        /// </summary>
        private readonly string scopeName;

        /// <summary>
        /// Initializes static members of the <see cref="AppServiceContractAttribute"/> class.
        /// </summary>
        static AppServiceContractAttribute()
        {
            DefaultMetadataAttributeTypes = new ReadOnlyCollection<Type>(WritableDefaultMetadataAttributeTypes);
            EmptyMetadataAttributeTypes = new ReadOnlyCollection<Type>(new List<Type>());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceContractAttribute"/> class.
        /// </summary>
        public AppServiceContractAttribute()
            : this(AppServiceLifetime.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceContractAttribute"/> class.
        /// </summary>
        /// <param name="lifetime">The lifetime.</param>
        protected AppServiceContractAttribute(AppServiceLifetime lifetime)
        {
            this.Lifetime = lifetime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceContractAttribute"/> class.
        /// </summary>
        /// <param name="scopeName">Name of the scope.</param>
        protected AppServiceContractAttribute(string scopeName)
            : this(AppServiceLifetime.ScopeShared)
        {
            Requires.NotNullOrEmpty(scopeName, nameof(scopeName));

            this.scopeName = scopeName;
        }

        /// <summary>
        /// Gets the application service lifetime.
        /// </summary>
        /// <value>
        /// The application service lifetime.
        /// </value>
        public AppServiceLifetime Lifetime { get; }

        /// <summary>
        /// Gets or sets a value indicating whether multiple services for this contract are allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multiple services are allowed; otherwise, <c>false</c>.
        /// </value>
        public bool AllowMultiple { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the contract should be exported as an open generic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the contract should be exported as an open generic; otherwise, <c>false</c>.
        /// </value>
        public bool AsOpenGeneric { get; set; }

        /// <summary>
        /// Gets or sets the supported metadata attributes.
        /// </summary>
        /// <value>
        /// The metadata attributes.
        /// </value>
        /// <remarks>The metadata attributes are used to register the conventions for application services.</remarks>
        public Type[] MetadataAttributes { get; set; }

        /// <summary>
        /// Gets or sets the contract type of the export.
        /// </summary>
        /// <value>
        /// The contract type of the export.
        /// </value>
        public Type ContractType { get; set; }

        /// <summary>
        /// Gets the service instance.
        /// </summary>
        /// <value>
        /// The service instance.
        /// </value>
        object IAppServiceInfo.Instance { get; }

        /// <summary>
        /// Gets the type of the service instance.
        /// </summary>
        /// <value>
        /// The type of the service instance.
        /// </value>
        Type IAppServiceInfo.InstanceType { get; }

        /// <summary>
        /// Gets the service instance factory.
        /// </summary>
        /// <value>
        /// The service instance factory.
        /// </value>
        Func<ICompositionContext, object> IAppServiceInfo.InstanceFactory { get; }

        /// <summary>
        /// Gets the name of the scope for scoped shared services.
        /// </summary>
        /// <value>
        /// The name of the scope for scoped shared services.
        /// </value>
        string IAppServiceInfo.ScopeName => this.scopeName;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var multiple = this.AllowMultiple ? ", multi" : string.Empty;
            var openGeneric = this.AsOpenGeneric ? ", open generic" : string.Empty;
            var scope = this.scopeName != null ? $"/scope:{this.scopeName}" : string.Empty;

            return $"{this.ContractType}{multiple}{openGeneric}, {this.Lifetime}{scope}";
        }

        /// <summary>
        /// Registers the provided metadata attribute types as default attributes.
        /// </summary>
        /// <param name="attributeTypes">A variable-length parameters list containing attribute types.</param>
        public static void RegisterDefaultMetadataAttributeTypes(params Type[] attributeTypes)
        {
            foreach (var attributeType in attributeTypes)
            {
                if (!WritableDefaultMetadataAttributeTypes.Contains(attributeType))
                {
                    WritableDefaultMetadataAttributeTypes.Add(attributeType);
                }
            }
        }
    }
}