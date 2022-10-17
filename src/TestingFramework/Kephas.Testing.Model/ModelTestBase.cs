﻿// --------------------------------------------------------------------------------------------------------------------
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

    using Kephas.Configuration;
    using Kephas.Model;
    using Kephas.Model.Runtime;
    using Kephas.Operations;
    using Kephas.Runtime;
    using NSubstitute;

    /// <summary>
    /// A model test base.
    /// </summary>
    public abstract class ModelTestBase : TestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(IRuntimeTypeInfo).Assembly,  // Kephas.Reflection
                typeof(IConfiguration<>).Assembly,  // Kephas.Configuration
                typeof(IOperationResult).Assembly,  // Kephas.Operations
            };
        }

        public IRuntimeModelRegistry GetModelRegistry(params Type[] elements)
        {
            var registry = Substitute.For<IRuntimeModelRegistry>();
            registry.GetRuntimeElementsAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<object>>(elements));
            return registry;
        }

        public IServiceProvider CreateInjectorForModel(params Type[] elements)
        {
            return this.CreateInjectorForModel(ambientServices: null, elements: elements);
        }

        public IServiceProvider CreateInjectorForModel(IAmbientServices? ambientServices, params Type[] elements)
        {
            var container = this.BuildServiceProvider(
                ambientServices: ambientServices,
                assemblies: new[] { typeof(IModelSpace).Assembly },
                config: b => b.ForFactory(_ => this.GetModelRegistry(elements)).Singleton().AllowMultiple());

            return container;
        }

        public IServiceProvider CreateInjectorForModel(Type[] parts, Type[] elements)
        {
            return this.CreateInjectorForModel(ambientServices: null, parts: parts, elements: elements);
        }

        public IServiceProvider CreateInjectorForModel(IAmbientServices? ambientServices, Type[] parts, Type[] elements)
        {
            var container = this.BuildServiceProvider(
                ambientServices: ambientServices,
                assemblies: new[] { typeof(IModelSpace).Assembly },
                parts: parts,
                config: b => b.ForFactory(_ => this.GetModelRegistry(elements)).Singleton().AllowMultiple());

            return container;
        }
    }
}