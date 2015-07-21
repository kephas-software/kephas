// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspectForAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Attribute to mark aspects to be applied to classifiers indicated by the provided types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.AttributedModel
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    using Kephas.Model.Runtime.Construction;

    /// <summary>
    /// Attribute to mark aspects to be applied to classifiers indicated by the provided types.
    /// </summary>
    public class AspectForAttribute : AspectAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AspectForAttribute"/> class.
        /// </summary>
        /// <param name="runtimeTypes">The runtime types.</param>
        public AspectForAttribute(params Type[] runtimeTypes)
            : base(GetRuntimeTypesFilter(runtimeTypes))
        {
            Contract.Requires(runtimeTypes != null && runtimeTypes.Length > 0);
        }

        /// <summary>
        /// Gets the filter for the classifiers based on the runtime types.
        /// </summary>
        /// <param name="runtimeTypes">The runtime types.</param>
        /// <returns>A predicate to filter the classifiers based on the provided runtime types.</returns>
        private static Func<IClassifier, bool> GetRuntimeTypesFilter(Type[] runtimeTypes)
        {
            var runtimeTypeInfos = runtimeTypes.Select(t => t.GetTypeInfo()).ToList();
            return
                c =>
                c.UnderlyingElementInfos.OfType<IRuntimeNamedElementInfo>()
                    .Any(info => runtimeTypeInfos.Contains(info.RuntimeElement));
        } 
    }
}