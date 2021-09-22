namespace Kephas.Data.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Injection;
    using Kephas.Model;
    using Kephas.Model.Runtime;
    using Kephas.Testing.Injection;
    using NSubstitute;

    /// <summary>
    /// A model test base.
    /// </summary>
    public abstract class DataModelWithSystemCompositionInjectionTestBase : SystemCompositionInjectionTestBase
    {
        public IRuntimeModelRegistry GetModelRegistry(params Type[] elements)
        {
            var registry = Substitute.For<IRuntimeModelRegistry>();
            registry.GetRuntimeElementsAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<object>>(elements));
            return registry;
        }

        public IInjector CreateContainerForModel(params Type[] elements)
        {
            var container = this.CreateInjector(
                assemblies: new[] { typeof(IModelSpace).GetTypeInfo().Assembly, typeof(IEntityType).Assembly },
                config: b => b.WithFactory(() => this.GetModelRegistry(elements), isSingleton: true));

            return container;
        }
    }
}