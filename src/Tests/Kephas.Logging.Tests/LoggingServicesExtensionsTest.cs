﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingServicesExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="AmbientServicesBuilder" />.
// </summary>
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
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class LoggingServicesExtensionsTest : TestBase
    {
        [Test]
        public void WithLogManager_success()
        {
            var ambientServices = this.CreateAmbientServices();
            new AppServiceCollectionBuilder(ambientServices).WithLogManager(new DebugLogManager());

            Assert.IsTrue(ambientServices.GetServiceInstance<ILogManager>() is DebugLogManager);
        }
    }
}