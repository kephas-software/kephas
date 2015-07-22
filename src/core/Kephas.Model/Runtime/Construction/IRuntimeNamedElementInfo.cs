// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeNamedElementInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract of named element infos for runtime elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Reflection;

    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Contract of named element infos for runtime elements.
    /// </summary>
    public interface IRuntimeNamedElementInfo : INamedElementInfo
    {
        /// <summary>
        /// Gets the runtime member information.
        /// </summary>
        /// <value>
        /// The runtime member information.
        /// </value>
        MemberInfo RuntimeElement { get; }
    }
}