// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionTestBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for tests using composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Mef.ExportProviders;
    using Kephas.Logging;
    using Kephas.Logging.Composition;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// Base class for tests using composition.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CompositionTestBase
    {
        [TestInitialize]
        public void TestSetup()
        {
            this.OnTestSetup();
        }

        public virtual void OnTestSetup()
        {
        }

        public virtual ContainerConfiguration WithEmptyConfiguration()
        {
            return new ContainerConfiguration();
        }

        public virtual ContainerConfiguration WithDefaultConfiguration()
        {
            var conventions = new MefConventionsBuilder();
            conventions.WithConventionsFrom(this.GetDefaultConventionAssemblies());
            return this.WithEmptyConfiguration()
              .WithDefaultConventions(conventions.GetConventionBuilder())
              .WithParts(this.GetDefaultParts());
        }

        public virtual ContainerConfiguration WithDefaultLogger(ContainerConfiguration configuration)
        {
            configuration.WithProvider(new TypeAffineExportDescriptorProvider<ILogger>(this.GetLogger));
            return configuration;
        }

        public virtual IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly> { typeof(ICompositionContainer).Assembly };
        }

        public virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }

        public virtual ILogger GetLogger(string name)
        {
            return Mock.Create<ILogger>();
        }
    }
}