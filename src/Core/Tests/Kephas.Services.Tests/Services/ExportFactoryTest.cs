// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the export factory extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Services;
    using NSubstitute;
    using Xunit;

        public class ExportFactoryTest
    {
        [Fact]
        public void CreateExportedValue()
        {
            var service = Substitute.For<ITestService>();
            var factory = new ExportFactory<ITestService>(() => service);

            var exportedValue = factory.CreateExportedValue();

            Assert.Same(service, exportedValue);
        }

        public interface ITestService {}
    }
}
