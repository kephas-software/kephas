// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingServicesExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Diagnostics.Logging;
    using Kephas.Logging;
    using Kephas.Services.Builder;
    using Kephas.Testing;
    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="LoggingServicesExtensions"/>.
    /// </summary>
    [TestFixture]
    public class LoggingServicesExtensionsTest : TestBase
    {
        [Test]
        public void WithLogManager_success()
        {
            var appServices = this.CreateAppServices();
            new AppServiceCollectionBuilder(appServices).WithLogManager(new DebugLogManager());

            Assert.IsTrue(appServices.GetServiceInstance<ILogManager>() is DebugLogManager);
        }
    }
}
