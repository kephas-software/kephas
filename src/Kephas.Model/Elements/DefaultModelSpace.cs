// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultModelSpace.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        /// Tries to get the classifier associated to the provided <see cref="ITypeInfo"/>.
        /// </summary>
        /// <param name="typeInfo">The <see cref="ITypeInfo"/>.</param>
        /// <param name="findContext">Context to control the finding of classifiers.</param>
        /// <returns>
        /// The classifier, or <c>null</c> if the classifier was not found.
        /// </returns>
        public IClassifier TryGetClassifier(ITypeInfo typeInfo, IContext findContext = null)
        {
            var classifier = typeInfo as IClassifier;
            if (classifier != null)
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
            classifier = this.TryComputeClassifier(typeInfo, classifiers, findContext);
            if (classifier != null)
            {
                // set the cached value.
                typeInfo[cacheKey] = classifier;
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
            // first, get the classifiers and resolve their aspects
            var unsortedClassifiers = constructionContext.ElementInfos.OfType<IClassifier>().ToList();
            this.ResolveAspects(unsortedClassifiers);

            // then sort them, to be able to have all the dependencies constructed completely
            // before moving on.
            constructionContext.ConstructedClassifiers = unsortedClassifiers;
            var orderGraphNodes = unsortedClassifiers.Select(
                c => new KeyValuePair<IClassifier, IEnumerable<IElementInfo>>(
                    c,
                    ((IConstructableElement)c).GetDependencies(constructionContext)));
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
            // collect the model dimensions and dimension elements, completing their construction
            this.Dimensions = this.ComputeDimensions(constructionContext);

            // build the model projections
            this.Projections = this.ComputeProjections(constructionContext, this.Dimensions);

            // complete the construction of the other model elements, assigning them to the right projection
            this.Classifiers = this.ComputeClassifiers(constructionContext);

            // aggregate the model elements, adding them to the right aggregated projection

            // TODO...;
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
        /// <param name="constructionContext">The construction context (optional).</param>
        /// <returns>
        /// An IClassifier.
        /// </returns>
        private IClassifier TryComputeClassifier(ITypeInfo typeInfo, IEnumerable<IClassifier> classifiers, IContext constructionContext)
        {
            // TODO 
            // return only aggregated classifiers, not partial ones.
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
                        var constructedType = resolvedGenericDefinition.MakeGenericType(typeInfo.GenericTypeArguments, constructionContext);
                        return constructedType as IClassifier;
                    }
                }
            }

            return resolvedClassifier;
        }

        /// <summary>
        /// Resolves the aspects by adding them as parts to the targeted classifiers.
        /// </summary>
        /// <param name="classifiers">The classifiers.</param>
        private void ResolveAspects(IReadOnlyCollection<IClassifier> classifiers)
        {
            var aspects = classifiers.Where(c => c.IsAspect).ToList();
            foreach (var aspect in aspects)
            {
                foreach (var classifier in classifiers)
                {
                    if (aspect != classifier && aspect.IsAspectOf(classifier))
                    {
                        ((IConstructableElement)classifier).AddPart(aspect);
                    }
                }
            }
        }
    }
}