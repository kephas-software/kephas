namespace Kephas.Data.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Model;
    using Kephas.Model.Runtime;
    using Kephas.Testing.Composition.Mef;

    using NSubstitute;

    /// <summary>
    /// A model test base.
    /// </summary>
    public abstract class DataModelTestBase : CompositionTestBase
    {
        public IRuntimeModelRegistry GetModelRegistry(params Type[] elements)
        {
            var registry = Substitute.For<IRuntimeModelRegistry>();
            registry.GetRuntimeElementsAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<object>>(elements));
            return registry;
        }

        public ICompositionContext CreateContainerForModel(params Type[] elements)
        {
            var container = this.CreateContainer(
                new[] { typeof(IModelSpace).GetTypeInfo().Assembly, typeof(IEntityType).Assembly },
                config: b => b.WithFactoryExportProvider(() => this.GetModelRegistry(elements), isShared: true));

            return container;
        }
    }
}