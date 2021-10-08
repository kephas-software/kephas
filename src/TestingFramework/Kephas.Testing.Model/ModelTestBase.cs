// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the model test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Model
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
    public abstract class ModelTestBase : InjectionTestBase
    {
        public IRuntimeModelRegistry GetModelRegistry(params Type[] elements)
        {
            var registry = Substitute.For<IRuntimeModelRegistry>();
            registry.GetRuntimeElementsAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<object>>(elements));
            return registry;
        }

        public IInjector CreateInjectorForModel(params Type[] elements)
        {
            return this.CreateInjectorForModel(ambientServices: null, elements: elements);
        }

        public IInjector CreateInjectorForModel(IAmbientServices? ambientServices, params Type[] elements)
        {
            var container = this.CreateInjector(
                ambientServices: ambientServices,
                assemblies: new[] { typeof(IModelSpace).Assembly },
                config: b => b.ForFactory(_ => this.GetModelRegistry(elements)).Singleton().AllowMultiple());

            return container;
        }

        public IInjector CreateInjectorForModel(Type[] parts, Type[] elements)
        {
            return this.CreateInjectorForModel(ambientServices: null, parts: parts, elements: elements);
        }

        public IInjector CreateInjectorForModel(IAmbientServices? ambientServices, Type[] parts, Type[] elements)
        {
            var container = this.CreateInjector(
                ambientServices: ambientServices,
                assemblies: new[] { typeof(IModelSpace).Assembly },
                parts: parts,
                config: b => b.ForFactory(_ => this.GetModelRegistry(elements)).Singleton().AllowMultiple());

            return container;
        }
    }
}