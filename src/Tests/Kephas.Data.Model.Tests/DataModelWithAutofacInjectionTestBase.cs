// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataModelWithAutofacInjectionTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Model;
    using Kephas.Model.Runtime;
    using Kephas.Testing.Services;
    using NSubstitute;

    /// <summary>
    /// A model test base.
    /// </summary>
    public abstract class DataModelWithAutofacInjectionTestBase : AutofacInjectionTestBase
    {
        public IRuntimeModelRegistry GetModelRegistry(params Type[] elements)
        {
            var registry = Substitute.For<IRuntimeModelRegistry>();
            registry.GetRuntimeElementsAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<object>>(elements));
            return registry;
        }

        public IServiceProvider CreateInjectorForModel(params Type[] elements)
        {
            var container = this.CreateInjector(
                assemblies: new[] { typeof(IModelSpace).Assembly, typeof(IEntityType).Assembly },
                config: b => b.ForFactory(_ => this.GetModelRegistry(elements)).Singleton());

            return container;
        }
    }
}