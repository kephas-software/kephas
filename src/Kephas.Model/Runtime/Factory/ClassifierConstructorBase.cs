// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierConstructorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base runtime provider for classifier information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using Kephas.Dynamic;
    using Kephas.Model.Elements;

    /// <summary>
    /// Base runtime provider for classifier information.
    /// </summary>
    /// <typeparam name="TModel">The type of the element information.</typeparam>
    public abstract class ClassifierConstructorBase<TModel> : ModelElementConstructorBase<TModel, IDynamicTypeInfo>
        where TModel : ClassifierBase<TModel>
    {
    }
}