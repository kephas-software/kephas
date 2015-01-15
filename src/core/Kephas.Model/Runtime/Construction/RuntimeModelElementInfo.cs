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
    /// <typeparam name="TRuntimeElement">The type of the runtime information.</typeparam>
    public abstract class RuntimeModelElementInfo<TRuntimeElement> : RuntimeNamedElementInfo<TRuntimeElement>, IModelElementInfo
        where TRuntimeElement : MemberInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelElementInfo{TRuntimeElement}"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime member information.</param>
        protected RuntimeModelElementInfo(TRuntimeElement runtimeElement)
            : base(runtimeElement)
        {
            this.Members = new List<INamedElementInfo>();
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