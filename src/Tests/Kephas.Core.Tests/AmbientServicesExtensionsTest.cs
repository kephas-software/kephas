// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="AmbientServicesBuilder" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Diagnostics.Logging;
    using Kephas.Logging;
    using Kephas.Testing;
    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AmbientServicesExtensions"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AmbientServicesExtensionsTest : TestBase
    {
        [Test]
        public void Constructor_register_ambient_services()
        {
            var ambientServices = this.CreateAmbientServices();

            Assert.AreSame(ambientServices, ambientServices.GetServiceInstance(typeof(IAmbientServices)));
        }

        [Test]
        public void WithLogManager_success()
        {
            var ambientServices = this.CreateAmbientServices();
            ambientServices.WithLogManager(new DebugLogManager());

            Assert.IsTrue(ambientServices.GetServiceInstance<ILogManager>() is DebugLogManager);
        }
    }
}
