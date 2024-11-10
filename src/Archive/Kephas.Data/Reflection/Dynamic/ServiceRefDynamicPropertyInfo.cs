// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRefDynamicPropertyInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Reflection.Dynamic
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Reflection;
    using Kephas.Reflection.Dynamic;
    using Kephas.Serialization;

    /// <summary>
    /// Dynamic property information for service references.
    /// </summary>
    public class ServiceRefDynamicPropertyInfo : DynamicPropertyInfo, IServiceRefPropertyInfo
    {
        private ITypeInfo? serviceRefType;
        private string? serviceRefTypeName;

        /// <summary>
        /// Gets or sets the service reference type.
        /// </summary>
        [ExcludeFromSerialization]
        public virtual ITypeInfo ServiceRefType
        {
            get => this.serviceRefType ??= this.TryGetType(this.serviceRefTypeName);
            set => this.serviceRefType = value;
        }

        /// <summary>
        /// Gets or sets the name of the service reference type.
        /// </summary>
        public virtual string? ServiceRefTypeName
        {
            get => this.serviceRefTypeName ?? this.serviceRefType?.FullName;
            set
            {
                this.serviceRefTypeName = value;
                this.serviceRefType = null;
            }
        }

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        [ExcludeFromSerialization]
        public override ITypeInfo ValueType
        {
            get => base.ValueType;
            set { }
        }

        /// <summary>
        /// Gets or sets the type name of the property.
        /// </summary>
        public override string? ValueTypeName
        {
            get => typeof(IServiceRef).FullName;
            set { }
        }

        /// <summary>
        /// Gets the reference type asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding the reference type.</returns>
        public virtual async Task<ITypeInfo> GetServiceRefTypeAsync(CancellationToken cancellationToken = default)
        {
            return this.serviceRefType ??= await this.TryGetTypeAsync(this.serviceRefTypeName, cancellationToken);
        }
    }
}