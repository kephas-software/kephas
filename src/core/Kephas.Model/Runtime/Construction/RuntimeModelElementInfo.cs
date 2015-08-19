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
    using System.Linq;
    using System.Reflection;

    using Kephas.Model.AttributedModel;
    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Runtime.Factory;

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
        }

        /// <summary>
        /// Gets the members' constructor information.
        /// </summary>
        /// <value>
        /// The members' constructor information.
        /// </value>
        public IEnumerable<INamedElementInfo> Members { get; private set; }

        /// <summary>
        /// Constructs the information.
        /// </summary>
        /// <param name="runtimeModelInfoFactory">The runtime model information provider.</param>
        internal protected override void ConstructInfo(IRuntimeModelInfoFactory runtimeModelInfoFactory)
        {
            this.Members = this.ComputeMembers(runtimeModelInfoFactory, this.RuntimeElement);
        }

        /// <summary>
        /// Computes the members from the runtime element.
        /// </summary>
        /// <param name="runtimeModelInfoFactory">The runtime model information provider.</param>
        /// <param name="runtimeElement">The runtime member information.</param>
        /// <returns>
        /// An enumeration of <see cref="INamedElementInfo"/>.
        /// </returns>
        protected virtual IEnumerable<INamedElementInfo> ComputeMembers(IRuntimeModelInfoFactory runtimeModelInfoFactory, TRuntimeElement runtimeElement)
        {
            var members = new List<INamedElementInfo>();

            var annotations = this.ComputeMemberAnnotations(runtimeModelInfoFactory, runtimeElement);
            if (annotations != null)
            {
                members.AddRange(annotations);
            }

            var properties = this.ComputeMemberProperties(runtimeModelInfoFactory, runtimeElement);
            if (properties != null)
            {
                members.AddRange(properties);
            }

            return members;
        }

        /// <summary>
        /// Computes the member annotations from the runtime element.
        /// </summary>
        /// <param name="runtimeModelInfoFactory">The runtime model information provider.</param>
        /// <param name="runtimeElement">The runtime member information.</param>
        /// <returns>
        /// An enumeration of <see cref="INamedElementInfo"/>.
        /// </returns>
        protected virtual IEnumerable<INamedElementInfo> ComputeMemberAnnotations(IRuntimeModelInfoFactory runtimeModelInfoFactory, TRuntimeElement runtimeElement)
        {
            var attributes = runtimeElement.GetCustomAttributes(inherit: false);
            return attributes.Select(runtimeModelInfoFactory.TryGetModelElementInfo)
                             .Where(annotationInfo => annotationInfo != null);
        }

        /// <summary>
        /// Computes the member properties from the runtime element.
        /// </summary>
        /// <param name="runtimeModelInfoFactory">The runtime model information provider.</param>
        /// <param name="runtimeElement">The runtime member information.</param>
        /// <returns>
        /// An enumeration of <see cref="INamedElementInfo"/>.
        /// </returns>
        protected virtual IEnumerable<INamedElementInfo> ComputeMemberProperties(IRuntimeModelInfoFactory runtimeModelInfoFactory, TRuntimeElement runtimeElement)
        {
            var properties = (runtimeElement as TypeInfo)?.DeclaredProperties.Where(pi => pi.GetCustomAttribute<ExcludeFromModelAttribute>() == null);
            return properties?.Select(runtimeModelInfoFactory.TryGetModelElementInfo)
                              .Where(propertyInfo => propertyInfo != null);
        }
    }
}