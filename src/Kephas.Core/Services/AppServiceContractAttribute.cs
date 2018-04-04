// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceContractAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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

    /// <summary>
    /// Marks an interface to be an application service contract.
    /// Application services are automatically identified by the composition
    /// and added to the container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppServiceContractAttribute : Attribute
    {
        /// <summary>
        /// The default metadata attribute types.
        /// </summary>
        public static readonly IReadOnlyCollection<Type> DefaultMetadataAttributeTypes;

        /// <summary>
        /// The default metadata attribute types.
        /// </summary>
        private static readonly IList<Type> WritableDefaultMetadataAttributeTypes
            = new List<Type>
                  {
                      typeof(ProcessingPriorityAttribute),
                      typeof(OverridePriorityAttribute),
                      typeof(OptionalServiceAttribute),
                      typeof(ServiceNameAttribute)
                  };

        /// <summary>
        /// Initializes static members of the <see cref="AppServiceContractAttribute"/> class.
        /// </summary>
        static AppServiceContractAttribute()
        {
            DefaultMetadataAttributeTypes = new ReadOnlyCollection<Type>(WritableDefaultMetadataAttributeTypes);
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
        /// Gets a value indicating whether the application service is shared.
        /// </summary>
        /// <value>
        ///   <c>true</c> if shared; otherwise, <c>false</c>.
        /// </value>
        public bool IsShared => this.Lifetime == AppServiceLifetime.Shared;

        /// <summary>
        /// Gets a value indicating whether the application service is instanced per request.
        /// </summary>
        /// <value>
        ///   <c>true</c> if instanced per request; otherwise, <c>false</c>.
        /// </value>
        public bool IsInstance => this.Lifetime == AppServiceLifetime.Instance;

        /// <summary>
        /// Gets a value indicating whether the application service is shared within a scope.
        /// </summary>
        /// <value>
        ///   <c>true</c> if shared within a scope; otherwise, <c>false</c>.
        /// </value>
        public bool IsScopeShared => this.Lifetime == AppServiceLifetime.ScopeShared;

        /// <summary>
        /// Gets or sets the contract type of the export.
        /// </summary>
        /// <value>
        /// The contract type of the export.
        /// </value>
        public Type ContractType { get; set; }

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