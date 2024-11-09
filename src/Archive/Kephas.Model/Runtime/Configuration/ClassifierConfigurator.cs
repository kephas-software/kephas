// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the classifier configurator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Configuration
{
    using Kephas.Runtime;

    /// <summary>
    /// A classifier configurator.
    /// </summary>
    /// <typeparam name="TRuntimeElement">Type of the runtime element.</typeparam>
    public abstract class ClassifierConfigurator<TRuntimeElement> : ClassifierConfiguratorBase<IClassifier, TRuntimeElement, ClassifierConfigurator<TRuntimeElement>>
    {
    }
}