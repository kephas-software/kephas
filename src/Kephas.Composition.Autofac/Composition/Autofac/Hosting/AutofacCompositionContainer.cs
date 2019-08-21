// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacCompositionContainer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac composition container class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Hosting
{
    using global::Autofac;

    /// <summary>
    /// An Autofac composition container.
    /// </summary>
    public class AutofacCompositionContainer : AutofacCompositionContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacCompositionContainer"/> class.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        public AutofacCompositionContainer(ContainerBuilder containerBuilder)
        {
            this.Initialize(containerBuilder.Build());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacCompositionContainer"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public AutofacCompositionContainer(IContainer container)
        {
            this.Initialize(container);
        }
    }
}