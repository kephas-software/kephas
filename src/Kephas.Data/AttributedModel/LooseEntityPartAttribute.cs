// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LooseEntityPartAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the loose entity part class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.AttributedModel
{
    using System;

    /// <summary>
    /// Marks an entity part as loose, meaning that it can have an autonomous life, however with an important relationship to the container entity.
    /// Such kinds of parts may be, for example, additional documents related to the original document.
    /// </summary>
    /// <remarks>
    /// Entity parts can be both self contained entities or collections.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class LooseEntityPartAttribute : EntityPartAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LooseEntityPartAttribute"/> class.
        /// </summary>
        public LooseEntityPartAttribute()
            : base(EntityPartKind.Loose)
        {
        }
    }
}