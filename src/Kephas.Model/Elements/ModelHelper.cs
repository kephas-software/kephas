// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Construction;
    using Kephas.Reflection;
    using Kephas.Runtime;

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
        private static readonly IReadOnlyList<IRuntimeTypeInfo> EmptyProjection = new List<IRuntimeTypeInfo>();

        /// <summary>
        /// Gets the member name discriminator for the provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The member name discriminator.</returns>
        public static string GetMemberNameDiscriminator(this Type type)
        {
            Requires.NotNull(type, nameof(type));

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
        public static IReadOnlyList<IRuntimeTypeInfo> GetRuntimeProjection(this INamedElement element)
        {
            if (element == null)
            {
                return EmptyProjection;
            }

            return (IReadOnlyList<IRuntimeTypeInfo>)element["RuntimeProjection"];
        }

        /// <summary>
        /// An INamedElement extension method that sets runtime projection.
        /// </summary>
        /// <param name="element">The element to act on.</param>
        /// <param name="projection">The projection.</param>
        public static void SetRuntimeProjection(this INamedElement element, IReadOnlyList<IRuntimeTypeInfo> projection)
        {
            if (element == null)
            {
                return;
            }

            element["RuntimeProjection"] = projection;
        }

        /// <summary>
        /// Gets the model element classifier dependencies.
        /// </summary>
        /// <param name="classifier">The classifier to act on.</param>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <returns>
        /// An enumeration of dependencies.
        /// </returns>
        internal static IEnumerable<ITypeInfo> GetClassifierDependencies(this IClassifier classifier, IModelConstructionContext constructionContext)
        {
            var parts = classifier.Parts.OfType<ITypeInfo>();
            return classifier.GetClassifierDependencies(constructionContext, parts);
        }

        /// <summary>
        /// Gets the model element classifier dependencies.
        /// </summary>
        /// <param name="classifier">The classifier to act on.</param>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// An enumeration of dependencies.
        /// </returns>
        internal static IEnumerable<ITypeInfo> GetClassifierDependencies(this IClassifier classifier, IModelConstructionContext constructionContext, IEnumerable<ITypeInfo> parts)
        {
            var eligibleTypes = parts
                .SelectMany(classifier.GetRawDependencies)
                .Distinct()
                .Select(t =>
                    {
                        // if the found dependency is the classifier itself, ignore it.
                        var dependency = constructionContext.ModelSpace.TryGetClassifier(t, findContext: constructionContext);
                        if (dependency == classifier)
                        {
                            return null;
                        }

                        return dependency ?? t;
                    })
                .Where(t => t != null)
                .ToList();
            return eligibleTypes;
        }

        /// <summary>
        /// Gets the model element raw dependencies (unsorted, possible duplicated).
        /// </summary>
        /// <param name="classifier">The classifier to act on.</param>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <returns>
        /// An enumeration of dependencies.
        /// </returns>
        internal static IEnumerable<ITypeInfo> GetRawDependencies(this IClassifier classifier, IModelConstructionContext constructionContext)
        {
            var parts = classifier.Parts.OfType<ITypeInfo>();
            return parts.SelectMany(classifier.GetRawDependencies);
        }

        /// <summary>
        /// Gets the raw model element dependencies (unsorted, possible duplicated).
        /// </summary>
        /// <param name="classifier">The classifier to act on.</param>
        /// <param name="typeInfo">Information describing the type.</param>
        /// <returns>
        /// An enumeration of dependencies.
        /// </returns>
        internal static IEnumerable<ITypeInfo> GetRawDependencies(this IClassifier classifier, ITypeInfo typeInfo)
        {
            if (typeInfo is IClassifier aspect)
            {
                if (aspect.IsAspectOf(classifier))
                {
                    yield return aspect;
                    yield break;
                }
            }

            foreach (var baseType in typeInfo.BaseTypes)
            {
                yield return baseType;

                if (baseType.IsConstructedGenericType())
                {
                    yield return baseType.GenericTypeDefinition;
                }
            }
        }
    }
}