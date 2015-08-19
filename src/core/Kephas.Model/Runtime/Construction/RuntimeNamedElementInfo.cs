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
    using Kephas.Model.Runtime.Factory;

    /// <summary>
    /// Runtime based constructor information for named elements.
    /// </summary>
    /// <typeparam name="TRuntimeElement">The type of the runtime information.</typeparam>
    public abstract class RuntimeNamedElementInfo<TRuntimeElement> : Expando, IRuntimeNamedElementInfo
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
        object IRuntimeNamedElementInfo.RuntimeElement => this.RuntimeElement;

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
        protected virtual string ElementNameDiscriminator => null;

        /// <summary>
        /// Constructs the information.
        /// </summary>
        /// <param name="runtimeModelInfoFactory">The runtime model information provider.</param>
        internal protected virtual void ConstructInfo(IRuntimeModelInfoFactory runtimeModelInfoFactory)
        {
        }

        /// <summary>
        /// Computes the model element name based on the runtime element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>The element name, or <c>null</c> if the name could not be computed.</returns>
        protected virtual string ComputeName(object runtimeElement)
        {
            var memberInfo = runtimeElement as MemberInfo;
            if (memberInfo == null)
            {
                return null;
            }

            var nameBuilder = new StringBuilder(memberInfo.Name);

            var typeInfo = runtimeElement as TypeInfo;
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

        /// <summary>
        /// Convert this object into a string representation.
        /// </summary>
        /// <returns>
        /// A string that represents this object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Name} ({this.RuntimeElement})";
        }
    }
}