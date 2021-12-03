// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspectForAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Attribute to mark aspects to be applied to classifiers indicated by the provided types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel
{
    using System;
    using System.Linq;

    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Attribute to mark aspects to be applied to classifiers indicated by the provided types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class AspectForAttribute : AspectAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AspectForAttribute"/> class.
        /// </summary>
        /// <param name="runtimeTypes">The runtime types.</param>
        public AspectForAttribute(params Type[] runtimeTypes)
            : base(GetRuntimeTypesFilter(runtimeTypes))
        {
            if (runtimeTypes == null || runtimeTypes.Length == 0) throw new System.ArgumentException("Value must not be null or empty.", nameof(runtimeTypes));
        }

        /// <summary>
        /// Gets the filter for the classifiers based on the runtime types.
        /// </summary>
        /// <param name="runtimeTypes">The runtime types.</param>
        /// <returns>A predicate to filter the classifiers based on the provided runtime types.</returns>
        private static Func<IClassifier, bool> GetRuntimeTypesFilter(Type[] runtimeTypes)
        {
            return
                c =>
                c.Parts.OfType<IRuntimeTypeInfo>()
                    .Any(info => runtimeTypes.Contains(info.Type));
        }
    }
}