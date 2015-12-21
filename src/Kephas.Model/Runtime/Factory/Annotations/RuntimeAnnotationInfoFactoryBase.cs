// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeAnnotationInfoFactoryBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for runtime annotation information factories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory.Annotations
{
    using System;

    using Kephas.Model.Runtime.Construction.Annotations;

    /// <summary>
    /// Base class for runtime annotation information factories.
    /// </summary>
    /// <typeparam name="TAnnotationInfo">Type of the annotation information.</typeparam>
    /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
    public abstract class RuntimeAnnotationInfoFactoryBase<TAnnotationInfo, TAttribute> : RuntimeNamedElementInfoFactoryBase<TAnnotationInfo, TAttribute>
        where TAnnotationInfo : RuntimeAnnotationInfoBase<TAttribute>
        where TAttribute : Attribute
    {
    }
}