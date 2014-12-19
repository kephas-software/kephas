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
    using System.Diagnostics.Contracts;
    using System.Reflection;

    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Runtime based constructor information for named elements.
    /// </summary>
    /// <typeparam name="TRuntimeInfo">The type of the runtime information.</typeparam>
    public abstract class RuntimeNamedElementInfo<TRuntimeInfo> : INamedElementInfo
        where TRuntimeInfo : MemberInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeNamedElementInfo{TRuntimeInfo}"/> class.
        /// </summary>
        /// <param name="runtimeMemberInfo">The runtime member information.</param>
        protected RuntimeNamedElementInfo(TRuntimeInfo runtimeMemberInfo)
        {
            Contract.Requires(runtimeMemberInfo != null);

            this.RuntimeMemberInfo = runtimeMemberInfo;
        }

        /// <summary>
        /// Gets the runtime member information.
        /// </summary>
        /// <value>
        /// The runtime member information.
        /// </value>
        public TRuntimeInfo RuntimeMemberInfo { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }
    }
}