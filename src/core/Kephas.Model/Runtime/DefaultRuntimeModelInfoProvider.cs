// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRuntimeModelInfoProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Model provider based on the .NET runtime and the type system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Logging;
    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Resources;
    using Kephas.Model.Runtime.Factory;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Model provider based on the .NET runtime and the type system.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultRuntimeModelInfoProvider : RuntimeModelInfoProviderBase
    {
        /// <summary>
        /// The model registrars.
        /// </summary>
        private readonly ICollection<IRuntimeModelRegistry> modelRegistries;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRuntimeModelInfoProvider" /> class.
        /// </summary>
        /// <param name="runtimeElementInfoFactoryDispatcher">The runtime model info factory.</param>
        /// <param name="modelRegistries">The model registries.</param>
        public DefaultRuntimeModelInfoProvider(
            IRuntimeElementInfoFactoryDispatcher runtimeElementInfoFactoryDispatcher,
            ICollection<IRuntimeModelRegistry> modelRegistries)
            : base(runtimeElementInfoFactoryDispatcher)
        {
            Contract.Requires(runtimeElementInfoFactoryDispatcher != null);
            Contract.Requires(modelRegistries != null);

            this.modelRegistries = modelRegistries;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<DefaultRuntimeModelInfoProvider> Logger { get; set; }

        /// <summary>
        /// Gets the element infos used for building the model space.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task promising an enumeration of element information.
        /// </returns>
        public override async Task<IEnumerable<INamedElementInfo>> GetElementInfosAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var runtimeElements = new HashSet<object>();
            foreach (var modelRegistry in this.modelRegistries)
            {
                var registryElements = await modelRegistry.GetRuntimeElementsAsync(cancellationToken).WithServerContext();
                runtimeElements.AddRange(registryElements.Select(this.NormalizeRuntimeElement));

                cancellationToken.ThrowIfCancellationRequested();
            }

            var elementInfos = new List<INamedElementInfo>();

            foreach (var runtimeElement in runtimeElements)
            {
                if (runtimeElement == null)
                {
                    continue;
                }

                var elementInfo = this.RuntimeElementInfoFactoryDispatcher.TryGetModelElementInfo(runtimeElement);
                if (elementInfo == null)
                {
                    this.Logger.Warn(Strings.CannotProvideElementInfoForRuntimeElement, runtimeElement.ToString());
                    continue;
                }

                elementInfos.Add(elementInfo);
            }

            cancellationToken.ThrowIfCancellationRequested();

            return elementInfos;
        }

        /// <summary>
        /// Normalizes the runtime element by returing the associated <see cref="TypeInfo"/> instead.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>The normalized runtime type.</returns>
        private object NormalizeRuntimeElement(object runtimeElement)
        {
            var runtimeType = runtimeElement as Type;
            return runtimeType != null ? runtimeType.GetTypeInfo() : runtimeElement;
        }
    }
}