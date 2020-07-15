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

    using Kephas.Composition;
    using Kephas.Model;
    using Kephas.Model.Runtime;
    using Kephas.Testing.Composition;
    using NSubstitute;

    /// <summary>
    /// A model test base.
    /// </summary>
    public abstract class ModelTestBase : CompositionTestBase
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
                assemblies: new [] { typeof(IModelSpace).GetTypeInfo().Assembly }, 
                config: b => b.WithFactory(() => this.GetModelRegistry(elements), isSingleton: true, allowMultiple: true));

            return container;
        }

        public ICompositionContext CreateContainerForModel(Type[] parts, Type[] elements)
        {
            var container = this.CreateContainer(
                assemblies: new[] { typeof(IModelSpace).GetTypeInfo().Assembly },
                parts: parts,
                config: b => b.WithFactory(() => this.GetModelRegistry(elements), isSingleton: true, allowMultiple: true));

            return container;
        }
    }
}