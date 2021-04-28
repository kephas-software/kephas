// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RefDynamicPropertyInfo.cs" company="Kephas Software SRL">
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

    /// <summary>
    /// Dynamic property information for entity references.
    /// </summary>
    public class RefDynamicPropertyInfo : DynamicPropertyInfo, IRefPropertyInfo
    {
        private ITypeInfo? refType;
        private string? refTypeName;

        /// <summary>
        /// Gets or sets the reference type.
        /// </summary>
        public virtual ITypeInfo RefType
        {
            get => this.refType ??= this.TryGetType(this.refTypeName);
            set => this.refType = value;
        }

        /// <summary>
        /// Gets or sets the name of the reference type.
        /// </summary>
        public virtual string? RefTypeName
        {
            get => this.refTypeName ?? this.refType?.FullName;
            set
            {
                this.refTypeName = value;
                this.refType = null;
            }
        }

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
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
            get => typeof(IRef).FullName;
            set { }
        }

        /// <summary>
        /// Gets the reference type asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding the reference type.</returns>
        public virtual async Task<ITypeInfo> GetRefTypeAsync(CancellationToken cancellationToken = default)
        {
            return this.refType ??= await this.TryGetTypeAsync(this.refTypeName, cancellationToken);
        }
    }
}