// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelConstructionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the model construction context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Construction
{
    using System;
    using System.Collections.Generic;
    using Kephas.Services;
    using Kephas.Logging;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A model construction context.
    /// </summary>
    public class ModelConstructionContext : Context, IModelConstructionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelConstructionContext"/> class.
        /// </summary>
        /// <param name="parentContext">Context for the parent.</param>
        public ModelConstructionContext(IContext parentContext)
            : base(parentContext)
        {
            parentContext = parentContext ?? throw new ArgumentNullException(nameof(parentContext));

            this.Logger = parentContext.ServiceProvider.GetLogger(this.GetType());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelConstructionContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        public ModelConstructionContext(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            this.Logger = serviceProvider.GetLogger(this.GetType());
        }

        /// <summary>
        /// Gets or sets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        public IModelSpace ModelSpace { get; set; }

        /// <summary>
        /// Gets or sets the model element factory.
        /// </summary>
        /// <value>
        /// The model element factory.
        /// </value>
        public IRuntimeModelElementFactory RuntimeModelElementFactory { get; set; }

        /// <summary>
        /// Gets or sets the element infos.
        /// </summary>
        /// <value>
        /// The element infos.
        /// </value>
        public IEnumerable<IElementInfo> ElementInfos { get; set; }

        /// <summary>
        /// Gets or sets the constructed classifiers.
        /// </summary>
        /// <value>
        /// The constructed classifiers.
        /// </value>
        public IEnumerable<IClassifier> ConstructedClassifiers { get; set; }

        /// <summary>
        /// Gets or sets a function to try to get a model element based on a native element information.
        /// </summary>
        /// <value>
        /// A function for getting a model element.
        /// </value>
        public Func<IElementInfo?, IElementInfo> TryGetModelElementInfo { get; set; }
    }
}