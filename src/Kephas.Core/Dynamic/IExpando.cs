// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExpando.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for dynamic objects allowing getting or setting
//   properties by their name through an indexer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System.Collections.Generic;
    using System.Dynamic;

    /// <summary>
    /// Contract for dynamic objects allowing getting or setting
    /// properties by their name through an indexer.
    /// </summary>
    public interface IExpando : IDynamicMetaObjectProvider, IIndexable
    {
        /// <summary>
        /// Indicates whether the <paramref name="memberName"/> is defined in the expando.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// True if defined, false if not.
        /// </returns>
        bool IsDefined(string memberName);

        /// <summary>
        /// Converts the expando to a dictionary having as keys the property names and as values the respective properties' values.
        /// </summary>
        /// <returns>
        /// A dictionary of property values with their associated names.
        /// </returns>
        IDictionary<string, object> ToDictionary();
    }
}