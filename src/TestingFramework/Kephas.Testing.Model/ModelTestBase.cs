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

    using Kephas.Configuration;
    using Kephas.Injection.Builder;
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

        public IAppServiceCollectionBuilder CreateServicesBuilderForModel(params Type[] elements)
        {
            return this.CreateServicesBuilderForModel(ambientServices: null, elements: elements);
        }

        public IAppServiceCollectionBuilder CreateServicesBuilderForModel(
            IAmbientServices? ambientServices,
            params Type[] elements)
        {
            var builder = this
                    .CreateServicesBuilder(ambientServices: ambientServices)
                    .WithAssemblies(typeof(IModelSpace).Assembly);

            builder.AmbientServices.Add(_ => this.GetModelRegistry(elements), b => b.Singleton().AllowMultiple());

            return builder;
        }

        public IAppServiceCollectionBuilder CreateServicesBuilderForModel(Type[] parts, Type[] elements)
        {
            return this.CreateServicesBuilderForModel(ambientServices: null, parts: parts, elements: elements);
        }

        public IAppServiceCollectionBuilder CreateServicesBuilderForModel(
            IAmbientServices? ambientServices,
            Type[] parts,
            Type[] elements)
        {
            return this.CreateServicesBuilderForModel(ambientServices, elements)
                .WithParts(parts);
        }
    }
}