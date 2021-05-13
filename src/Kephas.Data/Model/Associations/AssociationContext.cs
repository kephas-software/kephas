// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssociationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Associations
{
    using System;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Context for association operations.
    /// </summary>
    public class AssociationContext : Context, IAssociationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssociationContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public AssociationContext(IAmbientServices ambientServices)
            : base(ambientServices)
        {
        }

        /// <summary>
        /// Gets or sets the type filter.
        /// </summary>
        public Func<ITypeInfo, IContext?, bool>? TypeFilter { get; set; }
    }
}