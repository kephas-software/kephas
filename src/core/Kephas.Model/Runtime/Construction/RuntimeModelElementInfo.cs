// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelElementInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Runtime based constructor information for model elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Runtime based constructor information for model elements.
    /// </summary>
    /// <typeparam name="TRuntimeInfo">The type of the runtime information.</typeparam>
    public abstract class RuntimeModelElementInfo<TRuntimeInfo> : RuntimeNamedElementInfo<TRuntimeInfo>, IModelElementInfo
        where TRuntimeInfo : MemberInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelElementInfo{TRuntimeInfo}"/> class.
        /// </summary>
        /// <param name="runtimeMemberInfo">The runtime member information.</param>
        protected RuntimeModelElementInfo(TRuntimeInfo runtimeMemberInfo)
            : base(runtimeMemberInfo)
        {
        }

        /// <summary>
        /// Gets the members' constructor information.
        /// </summary>
        /// <value>
        /// The members' constructor information.
        /// </value>
        public IEnumerable<INamedElementInfo> Members { get; private set; }
    }
}