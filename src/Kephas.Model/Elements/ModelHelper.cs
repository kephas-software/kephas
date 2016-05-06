// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelHelper.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Helper class for model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Helper class for model.
    /// </summary>
    internal static class ModelHelper
    {
        /// <summary>
        /// The empty classifier enumeration.
        /// </summary>
        public static readonly IReadOnlyList<IClassifier> EmptyClassifiers = new ReadOnlyCollection<IClassifier>(new List<IClassifier>());

        /// <summary>
        /// The empty model element enumeration.
        /// </summary>
        public static readonly IReadOnlyList<IModelElement> EmptyModelElements = new ReadOnlyCollection<IModelElement>(new List<IModelElement>());

        /// <summary>
        /// The empty annotations.
        /// </summary>
        public static readonly IReadOnlyList<IAnnotation> EmptyAnnotations = new ReadOnlyCollection<IAnnotation>(new List<IAnnotation>());

        /// <summary>
        /// The name discriminators.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, string> NameDiscriminators = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// The empty projection.
        /// </summary>
        private static readonly IReadOnlyList<IDynamicTypeInfo> EmptyProjection = new List<IDynamicTypeInfo>();

        /// <summary>
        /// Gets the member name discriminator for the provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The member name discriminator.</returns>
        public static string GetMemberNameDiscriminator(this Type type)
        {
            Contract.Requires(type != null);

            return NameDiscriminators.GetOrAdd(
                type,
                t =>
                {
                    var attr = t.GetTypeInfo().GetCustomAttribute<MemberNameDiscriminatorAttribute>();
                    if (attr == null)
                    {
                        return string.Empty;
                    }

                    return attr.NameDiscriminator;
                });
        }

        /// <summary>
        /// An INamedElement extension method that gets runtime projection.
        /// </summary>
        /// <param name="element">The element to act on.</param>
        /// <returns>
        /// The runtime projection.
        /// </returns>
        public static IReadOnlyList<IDynamicTypeInfo> GetRuntimeProjection(this INamedElement element)
        {
            Contract.Ensures(Contract.Result<IReadOnlyList<IDynamicTypeInfo>>() != null);

            if (element == null)
            {
                return EmptyProjection;
            }

            return (IReadOnlyList<IDynamicTypeInfo>)element["RuntimeProjection"];
        }

        /// <summary>
        /// An INamedElement extension method that sets runtime projection.
        /// </summary>
        /// <param name="element">The element to act on.</param>
        /// <param name="projection">The projection.</param>
        public static void SetRuntimeProjection(this INamedElement element, IReadOnlyList<IDynamicTypeInfo> projection)
        {
            if (element == null)
            {
                return;
            }

            element["RuntimeProjection"] = projection;
        }
    }
}