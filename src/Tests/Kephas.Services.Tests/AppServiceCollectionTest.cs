// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="AppServiceCollection" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Kephas;
    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AppServiceCollection"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AppServiceCollectionTest : AppServiceCollectionTestBase
    {
        protected override IServiceProvider BuildServiceProvider(IAppServiceCollection appServices)
        {
            return this.CreateServicesBuilder(appServices).BuildWithDependencyInjection();
        }
    }
}