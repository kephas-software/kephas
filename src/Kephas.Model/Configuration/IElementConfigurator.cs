﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IElementConfigurator.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for model element configurators.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Configuration
{
    using Kephas.Model.Construction;

    /// <summary>
    /// Contract for model element configurators.
    /// </summary>
    public interface IElementConfigurator
    {
        /// <summary>
        /// Configures the model element provided.
        /// </summary>
        /// <param name="constructionContext">The construction context.</param>
        /// <param name="element">The model element to be configured.</param>
        void Configure(IModelConstructionContext constructionContext, INamedElement element);
    }
}