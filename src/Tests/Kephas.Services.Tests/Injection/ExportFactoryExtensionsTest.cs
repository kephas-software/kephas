// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the export factory extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Injection
{
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Services;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ExportFactoryExtensionsTest
    {
        [Test]
        public void GetExportedValue_Initializable()
        {
            var initializable = Substitute.For<IInitializable>();
            IExportFactory<IInitializable> factory = new ExportFactory<IInitializable>(() => initializable);

            var initializableValue = factory.CreateExportedValue();

            Assert.AreSame(initializable, initializableValue);
            initializable.Received(0).Initialize(Arg.Any<IContext>());
        }

        [Test]
        public void CreateInitializedValue_context_Initializable()
        {
            var initializable = Substitute.For<IInitializable>();
            var factory = Substitute.For<IExportFactory<IInitializable>>();
            factory.CreateExportedValue().Returns(initializable);

            var context = Substitute.For<IContext>();
            var initializableValue = factory.CreateInitializedValue(context);

            Assert.AreSame(initializable, initializableValue);
            initializable.Received(1).Initialize(context);
        }

        [Test]
        public async Task CreateInitializedValueAsync_context_AsyncInitializable()
        {
            var initializable = Substitute.For<IAsyncInitializable>();
            var factory = Substitute.For<IExportFactory<IAsyncInitializable>>();
            factory.CreateExportedValue().Returns(initializable);

            var context = Substitute.For<IContext>();
            var initializableValue = await factory.CreateInitializedValueAsync(context);

            Assert.AreSame(initializable, initializableValue);
            initializable.Received(1).InitializeAsync(context, Arg.Any<CancellationToken>());
        }

        [Test]
        public void CreateInitializedValue_context_AsyncInitializable()
        {
            var initializable = Substitute.For<IAsyncInitializable>();
            var factory = Substitute.For<IExportFactory<IAsyncInitializable>>();
            factory.CreateExportedValue().Returns(initializable);

            var context = Substitute.For<IContext>();
            var initializableValue = factory.CreateInitializedValue(context);

            Assert.AreSame(initializable, initializableValue);
            initializable.Received(1).InitializeAsync(context, Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task CreateInitializedValueAsync_context_Initializable()
        {
            var initializable = Substitute.For<IInitializable>();
            var factory = Substitute.For<IExportFactory<IInitializable>>();
            factory.CreateExportedValue().Returns(initializable);

            var context = Substitute.For<IContext>();
            var initializableValue = await factory.CreateInitializedValueAsync(context);

            Assert.AreSame(initializable, initializableValue);
            initializable.Received(1).Initialize(context);
        }
    }
}