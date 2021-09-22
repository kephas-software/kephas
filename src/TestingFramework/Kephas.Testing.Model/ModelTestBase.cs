// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the model test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Testing.Model
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Model;
    using Kephas.Model.Runtime;
    using Kephas.Testing.Composition;
    using NSubstitute;

    /// <summary>
    /// A model test base.
    /// </summary>
    public abstract class ModelTestBase : InjectionTestBase
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
            return this.CreateContainerForModel(ambientServices: null, elements: elements);
        }

        public IInjector CreateContainerForModel(IAmbientServices? ambientServices, params Type[] elements)
        {
            var container = this.CreateContainer(
                ambientServices: ambientServices,
                assemblies: new[] { typeof(IModelSpace).GetTypeInfo().Assembly },
                config: b => b.WithFactory(() => this.GetModelRegistry(elements), isSingleton: true, allowMultiple: true));

            return container;
        }

        public IInjector CreateContainerForModel(Type[] parts, Type[] elements)
        {
            return this.CreateContainerForModel(ambientServices: null, parts: parts, elements: elements);
        }

        public IInjector CreateContainerForModel(IAmbientServices? ambientServices, Type[] parts, Type[] elements)
        {
            var container = this.CreateContainer(
                ambientServices: ambientServices,
                assemblies: new[] { typeof(IModelSpace).GetTypeInfo().Assembly },
                parts: parts,
                config: b => b.WithFactory(() => this.GetModelRegistry(elements), isSingleton: true, allowMultiple: true));

            return container;
        }
    }
}