// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Testing;
    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AppServiceCollectionExtensions"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AppServiceCollectionExtensionsTest : TestBase
    {
        [Test]
        public void Constructor_register_ambient_services()
        {
            var appServices = this.CreateAppServices();

            Assert.AreSame(appServices, appServices.GetServiceInstance(typeof(IAppServiceCollection)));
        }
    }
}
