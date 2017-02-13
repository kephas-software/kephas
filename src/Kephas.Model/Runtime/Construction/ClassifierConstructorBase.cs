// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierConstructorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base runtime provider for classifier information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Base runtime provider for classifier information.
    /// </summary>
    /// <typeparam name="TModel">The type of the element information.</typeparam>
    /// <typeparam name="TModelContract">Type of the model contract.</typeparam>
    public abstract class ClassifierConstructorBase<TModel, TModelContract> : ModelElementConstructorBase<TModel, TModelContract, IRuntimeTypeInfo>
        where TModel : ClassifierBase<TModelContract>
        where TModelContract : class, IClassifier
    {
        /// <summary>
        /// Determines whether a model element can be created for the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// <c>true</c> if a model element can be created, <c>false</c> if not.
        /// </returns>
        protected override bool CanCreateModelElement(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            var classifierKind = runtimeElement.GetClassifierKind();
            if (classifierKind != null)
            {
                return classifierKind == typeof(TModelContract);
            }

            return false;
        }
    }
}