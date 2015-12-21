// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberNameDiscriminatorAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Attribute for indicating a name discriminator used by the qualified name.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Attribute for indicating a name discriminator used by the qualified name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class MemberNameDiscriminatorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberNameDiscriminatorAttribute"/> class.
        /// </summary>
        /// <param name="nameDiscriminator">The name discriminator.</param>
        public MemberNameDiscriminatorAttribute(string nameDiscriminator)
        {
            Contract.Requires(!string.IsNullOrEmpty(nameDiscriminator));

            this.NameDiscriminator = nameDiscriminator;
        }

        /// <summary>
        /// Gets or sets the name discriminator.
        /// </summary>
        /// <value>
        /// The name discriminator.
        /// </value>
        /// <remarks>
        /// The name discriminator will be used in the qualified name as a prefix for the name, 
        /// to prevent naming conflicts within the members collection.
        /// </remarks>
        public string NameDiscriminator { get; set; }
    }
}