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
        public static IEnumerable<ITypeInfo> GetClassifierDependencies(this IClassifier classifier, IModelConstructionContext constructionContext)
        {
            var parts = classifier.Parts.OfType<ITypeInfo>().ToList();
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
        public static IEnumerable<ITypeInfo> GetClassifierDependencies(this IClassifier classifier, IModelConstructionContext constructionContext, IList<ITypeInfo> parts)
        {
            var eligibleTypes = parts.SelectMany(classifier.GetDependencies)
                .Select(t => constructionContext.ModelSpace.TryGetClassifier(t, findContext: constructionContext) ?? t)
                .ToList();
            return eligibleTypes;
        }

        /// <summary>
        /// Gets the model element dependencies.
        /// </summary>
        /// <param name="classifier">The classifier to act on.</param>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <returns>
        /// An enumeration of dependencies.
        /// </returns>
        public static IEnumerable<ITypeInfo> GetDependencies(this IClassifier classifier, IModelConstructionContext constructionContext)
        {
            var parts = classifier.Parts.OfType<ITypeInfo>().ToList();
            return parts.SelectMany(classifier.GetDependencies);
        }

        /// <summary>
        /// Gets the model element dependencies.
        /// </summary>
        /// <param name="classifier">The classifier to act on.</param>
        /// <param name="typeInfo">Information describing the type.</param>
        /// <returns>
        /// An enumeration of dependencies.
        /// </returns>
        public static IEnumerable<ITypeInfo> GetDependencies(this IClassifier classifier, ITypeInfo typeInfo)
        {
            var aspect = typeInfo as IClassifier;
            if (aspect != null)
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