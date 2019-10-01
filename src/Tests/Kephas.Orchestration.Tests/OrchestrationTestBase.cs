// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the orchestration test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Tests
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Composition.ExportFactoryImporters;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Security.Authentication;
    using Kephas.Testing.Composition;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class OrchestrationTestBase : ApplicationTestBase
    {
        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            var assemblies = new List<Assembly>(base.GetDefaultConventionAssemblies())
                                {
                                    typeof(IMessageProcessor).Assembly,
                                    typeof(IOrchestrationManager).Assembly,
                                };
            return assemblies;
        }

        protected virtual ICompositionContext CreateSubstituteContainer()
        {
            var container = Substitute.For<ICompositionContext>();

            container.GetExport(typeof(IExportFactoryImporter<IBrokeredMessageBuilder>), Arg.Any<string>())
                .Returns(ci =>
                    new ExportFactoryImporter<IBrokeredMessageBuilder>(
                        new ExportFactory<IBrokeredMessageBuilder>(
                            () =>
                            {
                                return new BrokeredMessageBuilder(Substitute.For<IAppManifest>(), Substitute.For<IAuthenticationService>());
                            })));

            return container;
        }
    }
}
