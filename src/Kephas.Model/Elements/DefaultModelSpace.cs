// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultModelSpace.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The default implementation of the model space.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Sets;

    /// <summary>
    /// The default implementation of the model space.
    /// </summary>
    public class DefaultModelSpace : ModelElementBase<IModelSpace>, IModelSpace
    {
        /// <summary>
        /// Compares two classifiers to get a priority in handling them.
        /// A classifier is "greater" than another classifier if the other one is contained in its <see cref="T:INamedElementBase{T}.Parts"/>.
        /// Otherwise they are not comparable.
        /// </summary>
        /// <returns>
        /// A value used to compare the two classifiers.
        /// </returns>
        public static readonly Func<KeyValuePair<IClassifier, IEnumerable<IElementInfo>>, KeyValuePair<IClassifier, IEnumerable<IElementInfo>>, int?> ClassifierDependencyComparer = (c1, c2) =>
            {
                if (c1.Value.Contains(c2.Key))
                {
                    return 1;
                }

                if (c2.Value.Contains(c1.Key))
                {
                    return -1;
                }

                return null;
            };

    /// <summary>
    /// Unique identifier.
    /// </summary>
    private readonly Guid guid = Guid.NewGuid();

        /// <summary>
        /// The classifier cache key.
        /// </summary>
        private readonly string classifierCacheKey;

        /// <summary>
        /// The classifier cache key for construction.
        /// </summary>
        private readonly string constructionClassifierCacheKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultModelSpace"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        public DefaultModelSpace(IModelConstructionContext constructionContext)
            : base(constructionContext, string.Empty)
        {
            this.classifierCacheKey = $"CLS_{this.guid}";
            this.constructionClassifierCacheKey = $"CLS_CONSTRUCT_{this.guid}";
        }

        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        public override IModelSpace ModelSpace => this;

        /// <summary>
        /// Gets the dimensions.
        /// </summary>
        /// <value>
        /// The dimensions.
        /// </value>
        public IReadOnlyList<IModelDimension> Dimensions { get; private set; }

        /// <summary>
        /// Gets the projections.
        /// </summary>
        /// <value>
        /// The projections.
        /// </value>
        public IEnumerable<IModelProjection> Projections { get; private set; }

        /// <summary>
        /// Gets the classifiers.
        /// </summary>
        /// <value>
        /// The classifiers.
        /// </value>
        public IEnumerable<IClassifier> Classifiers { get; private set; }

        /// <summary>
        /// Gets the context for construction.
        /// </summary>
        /// <value>
        /// The construction context.
        /// </value>
        public IModelConstructionContext ConstructionContext { get; private set; }

        /// <summary>
        /// Tries to get the classifier associated to the provided <see cref="ITypeInfo"/>.
        /// </summary>
        /// <param name="typeInfo">The <see cref="ITypeInfo"/>.</param>
        /// <param name="findContext">Context to control the finding of classifiers.</param>
        /// <returns>
        /// The classifier, or <c>null</c> if the classifier was not found.
        /// </returns>
        public IClassifier TryGetClassifier(ITypeInfo typeInfo, IContext findContext = null)
        {
            if (typeInfo is IClassifier classifier)
            {
                return classifier;
            }

            var constructionContext = findContext as IModelConstructionContext;
            var cacheKey = constructionContext != null
                               ? this.constructionClassifierCacheKey
                               : this.classifierCacheKey;

            // try to get from the cached value.
            classifier = typeInfo[cacheKey] as IClassifier;
            if (classifier != null)
            {
                return classifier;
            }

            var classifiers = constructionContext?.ConstructedClassifiers ?? this.Classifiers;
            var isNew = false;
            (classifier, isNew) = this.TryComputeClassifier(typeInfo, classifiers, constructionContext);
            if (classifier != null)
            {
                // set the cached value.
                typeInfo[cacheKey] = classifier;

                if (isNew)
                {
                    // TODO lock the list to avoid problems in multi threaded environments
                    var listClassifiers = classifiers as IList<IClassifier>;
                    listClassifiers?.Add(classifier);
                }
            }

            return classifier;
        }

        /// <summary>
        /// Calculates the dimensions.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <returns>
        /// The calculated dimensions.
        /// </returns>
        protected internal virtual IModelDimension[] ComputeDimensions(
            IModelConstructionContext constructionContext)
        {
            var dimensions = constructionContext.ElementInfos.OfType<IModelDimension>().OrderBy(d => d.Index).ToArray();
            dimensions.ForEach(dimension => (dimension as IConstructableElement)?.CompleteConstruction(constructionContext));

            return dimensions;
        }

        /// <summary>
        /// Calculates the projections.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="dimensions">The dimensions.</param>
        /// <returns>
        /// The calculated projections.
        /// </returns>
        protected internal virtual IList<IModelProjection> ComputeProjections(
            IModelConstructionContext constructionContext,
            IReadOnlyList<IModelDimension> dimensions)
        {
            var projections = new List<IModelProjection>();
            this.BuildProjections(constructionContext, dimensions.ToArray(), 0, new List<IModelDimensionElement>(), projections);

            var nonAggregatableDimensions = dimensions.Where(d => !d.IsAggregatable).ToArray();
            if (dimensions.Count > nonAggregatableDimensions.Length)
            {
                var nonAggregatableProjections = new List<IModelProjection>();
                this.BuildProjections(constructionContext, nonAggregatableDimensions, 0, new List<IModelDimensionElement>(), nonAggregatableProjections);
                var nonAggregatableDictionary = nonAggregatableProjections.ToDictionary(p => p.Name, p => p);
                projections.ForEach(p => ((IConstructableElement)nonAggregatableDictionary[p.AggregatedProjectionName]).AddPart(p));

                projections.AddRange(nonAggregatableProjections);
            }

            projections.ForEach(c => (c as IConstructableElement)?.CompleteConstruction(constructionContext));

            return projections;
        }

        /// <summary>
        /// Enumerates compute classifiers in this collection.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process compute classifiers in this
        /// collection.
        /// </returns>
        protected internal virtual IEnumerable<IClassifier> ComputeClassifiers(IModelConstructionContext constructionContext)
        {
            // 1. get the classifiers and collect from them all other classifier references/dependencies
            // to be able to get a proper sorting upon construction completion
            var unsortedClassifiers = constructionContext.ElementInfos.OfType<IClassifier>().ToList();

            // 2. identify the constructed generics dependencies (from the base types)
            // and add them to the unsorted classifiers.
            // Note: there is no need to include Property/Field/Method parameter types,
            // because they can be solved at a later time. Now it is important to resolve the
            // type inheritance graph.
            var constructedGenericsDependencies = this.ComputeConstructedGenericDependencies(constructionContext, unsortedClassifiers);
            unsortedClassifiers.AddRange(
                constructedGenericsDependencies
                    .Select(d => constructionContext.TryGetElementInfo?.Invoke(d) as IClassifier)
                    .Where(c => c != null));

            // When constructing generics, must change the signature to include (param, arg) mappings
            
            // 3. resolve the aspects.
            // TODO aspects can be applied only to non-generic or open-generics
            this.ResolveAspects(unsortedClassifiers, this.GetAspects(unsortedClassifiers));

            // 4. sort them, to be able to have all the dependencies completely constructed
            // before moving on.
            constructionContext.ConstructedClassifiers = unsortedClassifiers;
            var orderGraphNodes = unsortedClassifiers.Select(
                c => new KeyValuePair<IClassifier, IEnumerable<IElementInfo>>(
                    c,
                    c.GetClassifierDependencies(constructionContext)));
            var orderedSet = new PartialOrderedSet<KeyValuePair<IClassifier, IEnumerable<IElementInfo>>>(orderGraphNodes, ClassifierDependencyComparer);
            var classifiers = orderedSet.Select(cd => cd.Key).ToList();

            // having ordered classifiers, go complete their construction
            // the constructed classifiers are now in the proper order
            constructionContext.ConstructedClassifiers = classifiers;
            classifiers.ForEach(c => (c as IConstructableElement)?.CompleteConstruction(constructionContext));

            var constructionExceptions = classifiers.OfType<IConstructableElement>()
                .Where(c => c.ConstructionState.IsFaulted)
                .Select(c => c.ConstructionState.Exception)
                .Where(e => e != null)
                .ToList();

            if (constructionExceptions.Count > 0)
            {
                throw new AggregateException(constructionExceptions);
            }

            return classifiers;
        }

        /// <summary>
        /// Called when the construction is complete.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        protected override void OnCompleteConstruction(IModelConstructionContext constructionContext)
        {
            this.ConstructionContext = constructionContext;

            // collect the model dimensions and dimension elements, completing their construction
            this.Dimensions = this.ComputeDimensions(constructionContext);

            // build the model projections
            this.Projections = this.ComputeProjections(constructionContext, this.Dimensions);

            // complete the construction of the other model elements, assigning them to the right projection
            this.Classifiers = this.ComputeClassifiers(constructionContext);

            // TODO aggregate the model elements, adding them to the right aggregated projection
            // ...
        }

        /// <summary>
        /// Builds the projections.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="dimensions">The dimensions.</param>
        /// <param name="index">Zero-based index of the dimensions array.</param>
        /// <param name="elements">The elements of the projection.</param>
        /// <param name="projections">The projections.</param>
        private void BuildProjections(IModelConstructionContext constructionContext, IModelDimension[] dimensions, int index, IList<IModelDimensionElement> elements, IList<IModelProjection> projections)
        {
            if (index >= dimensions.Length)
            {
                var aggregatedElements = elements.Where(e => !e.DeclaringDimension.IsAggregatable).ToArray();
                var projection = new ModelProjection(
                    constructionContext,
                    this.ComputeProjectionName(elements),
                    this.ComputeProjectionName(aggregatedElements)) { DimensionElements = elements.ToArray() };
                projections.Add(projection);
                return;
            }

            var dimension = dimensions[index];
            foreach (var element in dimension.Elements)
            {
                var projectionElements = new List<IModelDimensionElement>(elements);
                projectionElements.Add(element);

                this.BuildProjections(constructionContext, dimensions, index + 1, projectionElements, projections);
            }
        }

        /// <summary>
        /// Calculates the projection name.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns>
        /// The calculated projection name.
        /// </returns>
        private string ComputeProjectionName(IEnumerable<IModelDimensionElement> elements)
        {
            return string.Concat(elements.Select(e => e.QualifiedFullName));
        }

        /// <summary>
        /// Tries to compute the classifier of the provided <see cref="ITypeInfo"/>.
        /// </summary>
        /// <param name="typeInfo">The <see cref="ITypeInfo"/>.</param>
        /// <param name="classifiers">The classifiers.</param>
        /// <param name="constructionContext">The construction context.</param>
        /// <returns>
        /// An IClassifier.
        /// </returns>
        private (IClassifier Classifier, bool IsNew) TryComputeClassifier(ITypeInfo typeInfo, IEnumerable<IClassifier> classifiers, IModelConstructionContext constructionContext)
        {
            // TODO return only aggregated classifiers, not partial ones.
            // try to find in all classifiers, in all parts, the provided type info
            // if one is found, the containing classifier is the searched one.

            var resolvedClassifier = classifiers.FirstOrDefault(c => c == typeInfo || c.Aggregates(typeInfo));
            if (resolvedClassifier == null)
            {
                if (typeInfo.IsConstructedGenericType())
                {
                    var genericTypeDefinition = typeInfo.GenericTypeDefinition;
                    var resolvedGenericDefinition = classifiers.FirstOrDefault(c => c == genericTypeDefinition || c.Aggregates(genericTypeDefinition));
                    if (resolvedGenericDefinition != null)
                    {
                        constructionContext = constructionContext ?? this.ConstructionContext;
                        var constructedType = (IClassifier)constructionContext.TryGetElementInfo(typeInfo);
                        this.ResolveAspects(new[] { constructedType }, this.GetAspects(classifiers));

                        // TODO: maybe do not complete construction yet, if the model space is still constructing
                        ((IConstructableElement)constructedType).CompleteConstruction(constructionContext);

                        return (constructedType, true);
                    }
                }
            }

            return (resolvedClassifier, false);
        }

        /// <summary>
        /// Resolves the aspects by adding them as parts to the targeted classifiers.
        /// </summary>
        /// <param name="classifiers">The classifiers.</param>
        /// <param name="aspects">The classifier aspects.</param>
        private void ResolveAspects(IReadOnlyCollection<IClassifier> classifiers, IEnumerable<IClassifier> aspects)
        {
            foreach (var aspect in aspects)
            {
                foreach (var classifier in classifiers)
                {
                    // TODO apply open generic aspects to open generic classifiers
                    // and constructed generic aspects to constructed generic classifiers
                    if (aspect != classifier && aspect.IsAspectOf(classifier))
                    {
                        ((IConstructableElement)classifier).AddPart(aspect);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the aspects from the classifiers.
        /// </summary>
        /// <param name="classifiers">The classifiers.</param>
        /// <returns>
        /// The aspects.
        /// </returns>
        private IEnumerable<IClassifier> GetAspects(IEnumerable<IClassifier> classifiers)
        {
            return classifiers.Where(c => c.IsAspect);
        }

        /// <summary>
        /// Calculates the constructed generic dependencies.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="unsortedClassifiers">The unsorted classifiers.</param>
        /// <returns>
        /// The calculated constructed generic dependencies.
        /// </returns>
        private HashSet<ITypeInfo> ComputeConstructedGenericDependencies(
            IModelConstructionContext constructionContext,
            IList<IClassifier> unsortedClassifiers)
        {
            var genericDefinitions = unsortedClassifiers
                .SelectMany(c => c.Parts)
                .OfType<ITypeInfo>()
                .Where(p => p.IsGenericTypeDefinition())
                .ToList();
            var constructedGenericsDependencies = new HashSet<ITypeInfo>(
                unsortedClassifiers
                    .SelectMany(c => c.GetRawDependencies(constructionContext))
                    .Where(t => t.IsConstructedGenericType() && genericDefinitions.Contains(t.GenericTypeDefinition)));
            while (true)
            {
                var newDependencies = constructedGenericsDependencies
                    .SelectMany(d => d.GenericTypeArguments)
                    .Where(t => !constructedGenericsDependencies.Contains(t) && 
                                t.IsConstructedGenericType() && genericDefinitions.Contains(t.GenericTypeDefinition))
                    .ToList();
                if (!newDependencies.Any())
                {
                    break;
                }

                constructedGenericsDependencies.AddRange(newDependencies);
            }

            return constructedGenericsDependencies;
        }
    }
}