// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstructorTestBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the constructor test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Construction
{
    using System;
    using System.Collections.Generic;

    using Kephas.Composition;
    using Kephas.Dynamic;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Runtime.Configuration;
    using Kephas.Model.Runtime.Configuration.Composition;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Construction.Annotations;
    using Kephas.Model.Runtime.Construction.Composition;

    using NUnit.Framework.Constraints;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    public class ConstructorTestBase
    {
        /// <summary>
        /// Gets the construction context.
        /// </summary>
        /// <param name="modelSpace">The model space.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>
        /// The construction context.
        /// </returns>
        public IModelConstructionContext GetConstructionContext(
            IModelSpace modelSpace = null,
            IRuntimeModelElementFactory factory = null)
        {
            return new ModelConstructionContext
                       {
                           ModelSpace = modelSpace ?? Mock.Create<IModelSpace>(),
                           RuntimeModelElementFactory = factory ?? this.GetNullRuntimeModelElementFactory(),
                       };
        }

        public IRuntimeModelElementFactory GetTestRuntimeModelElementFactory()
        {
            var constructors = new List<IExportFactory<IRuntimeModelElementConstructor, RuntimeModelElementConstructorMetadata>>
                                   {
                                       this.CreateExportFactory((IRuntimeModelElementConstructor)new AnnotationConstructor(), new RuntimeModelElementConstructorMetadata(typeof(Annotation), typeof(IAnnotation), typeof(Attribute))),
                                       this.CreateExportFactory((IRuntimeModelElementConstructor)new PropertyConstructor(), new RuntimeModelElementConstructorMetadata(typeof(Property), typeof(IProperty), typeof(IDynamicPropertyInfo))),
                                   };

            var configurators = new List<IExportFactory<IRuntimeModelElementConfigurator, RuntimeModelElementConfiguratorMetadata>>();

            var factory = new DefaultRuntimeModelElementFactory(constructors, configurators);
            return factory;
        }

        public IRuntimeModelElementFactory GetNullRuntimeModelElementFactory()
        {
            var factory = Mock.Create<IRuntimeModelElementFactory>();
            factory.Arrange(f => f.TryCreateModelElement(Arg.IsAny<IModelConstructionContext>(), Arg.AnyObject))
                .Returns((INamedElement)null);
            return factory;
        }

        public IExportFactory<T, TMetadata> CreateExportFactory<T, TMetadata>(T service, TMetadata metadata)
        {
            var export = Mock.Create<IExport<T, TMetadata>>();
            export.Arrange(ex => ex.Value).Returns(service);

            var exportFactory = Mock.Create<IExportFactory<T, TMetadata>>();
            exportFactory.Arrange(ex => ex.Metadata).Returns(metadata);
            exportFactory.Arrange(ex => ex.CreateExport()).Returns(export);

            return exportFactory;
        }
    }
}