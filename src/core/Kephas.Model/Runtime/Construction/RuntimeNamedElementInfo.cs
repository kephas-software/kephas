// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeNamedElementInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Runtime based constructor information for named elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Text;

    using Kephas.Dynamic;

    /// <summary>
    /// Runtime based constructor information for named elements.
    /// </summary>
    /// <typeparam name="TRuntimeElement">The type of the runtime information.</typeparam>
    public abstract class RuntimeNamedElementInfo<TRuntimeElement> : Expando, IRuntimeNamedElementInfo
        where TRuntimeElement : MemberInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeNamedElementInfo{TRuntimeElement}"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime member information.</param>
        protected RuntimeNamedElementInfo(TRuntimeElement runtimeElement)
        {
            Contract.Requires(runtimeElement != null);

            this.RuntimeElement = runtimeElement;
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            this.Name = this.ComputeName(runtimeElement);
        }

        /// <summary>
        /// Gets the runtime member information.
        /// </summary>
        /// <value>
        /// The runtime member information.
        /// </value>
        public TRuntimeElement RuntimeElement { get; }

        /// <summary>
        /// Gets the runtime member information.
        /// </summary>
        /// <value>
        /// The runtime member information.
        /// </value>
        MemberInfo IRuntimeNamedElementInfo.RuntimeElement => this.RuntimeElement;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the function used to select the container.
        /// </summary>
        /// <value>
        /// The function used to select the container.
        /// </value>
        /// <remarks>
        /// This function returns <c>true</c> if the current element is member of the provided container.
        /// </remarks>
        public Func<IModelElement, bool> IsMemberOf { get; protected set; }

        /// <summary>
        /// Gets the element name discriminator.
        /// </summary>
        /// <value>
        /// The element name discriminator.
        /// </value>
        /// <remarks>
        /// This dicriminator can be used as a suffix in the name to identify the element type.
        /// </remarks>
        protected virtual string ElementNameDiscriminator
        {
            get { return null; }
        }

        /// <summary>
        /// Computes the model element name based on the runtime element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>The element name, or <c>null</c> if the name could not be computed.</returns>
        protected virtual string ComputeName(TRuntimeElement runtimeElement)
        {
            var memberInfo = this.RuntimeElement as MemberInfo;
            if (memberInfo == null)
            {
                return null;
            }

            var nameBuilder = new StringBuilder(memberInfo.Name);

            var typeInfo = this.RuntimeElement as TypeInfo;
            if (typeInfo != null && typeInfo.IsInterface && nameBuilder[0] == 'I')
            {
                nameBuilder.Remove(0, 1);
            }

            var discriminator = this.ElementNameDiscriminator;
            if (!string.IsNullOrEmpty(discriminator))
            {
                if (memberInfo.Name.EndsWith(discriminator))
                {
                    nameBuilder.Remove(nameBuilder.Length - discriminator.Length, discriminator.Length);
                }
            }

            return nameBuilder.ToString();
        }
    }
}