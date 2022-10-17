// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Application
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Testing.Injection;

    /// <summary>
    /// Base class for application tests using dependency injection.
    /// </summary>
    /// <content>
    /// It includes:
    /// * The default convention assemblies contain:
    ///   * Kephas.Core
    ///   * Kephas.Application
    /// </content>
    public class ApplicationTestBase : TestBase
    {
        /// <summary>
        /// Gets the default convention assemblies to be considered when building the container. By
        /// default it includes Kephas.Core and Kephas.Application.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the default convention assemblies in
        /// this collection.
        /// </returns>
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
                       {
                           typeof(IAppLifecycleBehavior).Assembly,         // Kephas.Application.Abstractions
                           typeof(IAppManager).Assembly,     /* Kephas.Application*/
                       };
        }
    }
}
