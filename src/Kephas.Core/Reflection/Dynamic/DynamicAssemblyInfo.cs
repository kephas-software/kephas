// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicAssemblyInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic assembly information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Resources;

    /// <summary>
    /// Information about the dynamic assembly.
    /// </summary>
    public class DynamicAssemblyInfo : DynamicElementInfo, IAssemblyInfo
    {
        /// <summary>
        /// The types.
        /// </summary>
        private readonly IList<ITypeInfo> types = new List<ITypeInfo>();

        /// <summary>
        /// Gets the types declared in this assembly.
        /// </summary>
        /// <value>
        /// The declared types.
        /// </value>
        public IEnumerable<ITypeInfo> Types => this.types;

        /// <summary>
        /// Adds a type to the assembly.
        /// </summary>
        /// <param name="type">The type to add.</param>
        protected internal virtual void AddType(ITypeInfo type)
        {
            Requires.NotNull(type, nameof(type));

            if (this.types.Any(m => m.FullName == type.FullName))
            {
                throw new InvalidOperationException(string.Format(Strings.DynamicAssemblyInfo_AddType_Duplicate_Exception, type.FullName, this));
            }

            this.types.Add(type);
        }
    }
}