// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesBuilderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="AmbientServicesBuilder" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Diagnostics.Logging;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AmbientServicesBuilderTest
    {
        [Test]
        public void WithLogManager_success()
        {
            var ambientServices = new AmbientServices();
            var builder = new AmbientServicesBuilder(ambientServices);
            builder.WithLogManager(new DebugLogManager());

            Assert.IsTrue(ambientServices.LogManager is DebugLogManager);
        }
    }
}
