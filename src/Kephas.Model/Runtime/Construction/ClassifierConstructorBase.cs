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
    using Kephas.Dynamic;
    using Kephas.Model.Elements;
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
    }
}