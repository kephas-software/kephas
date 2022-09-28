﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstructorTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the constructor test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Construction
{
    using System;
    using System.Collections.Generic;
    using Kephas.Injection;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Runtime.Configuration;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Construction.Annotations;
    using Kephas.Runtime;
    using Kephas.Testing;
    using NSubstitute;

    public class ConstructorTestBase : TestBase
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
            IModelSpace? modelSpace = null,
            IRuntimeModelElementFactory? factory = null)
        {
            var ambientServices = this.CreateAmbientServices().WithStaticAppRuntime();
            var injector = Substitute.For<IServiceProvider>();
            injector.Resolve<IAmbientServices>().Returns(ambientServices);
            return new ModelConstructionContext(injector)
            {
                ModelSpace = modelSpace ?? Substitute.For<IModelSpace>(),
                RuntimeModelElementFactory = factory ?? this.GetNullRuntimeModelElementFactory(),
            };
        }

        public IRuntimeModelElementFactory GetTestRuntimeModelElementFactory()
        {
            var constructors = new List<IExportFactory<IRuntimeModelElementConstructor, RuntimeModelElementConstructorMetadata>>
                                   {
                                       new ExportFactory<IRuntimeModelElementConstructor, RuntimeModelElementConstructorMetadata>(() => new AttributeAnnotationConstructor(), new RuntimeModelElementConstructorMetadata(typeof(Annotation), typeof(IAnnotation), typeof(Attribute))),
                                       new ExportFactory<IRuntimeModelElementConstructor, RuntimeModelElementConstructorMetadata>(() => new PropertyConstructor(), new RuntimeModelElementConstructorMetadata(typeof(Property), typeof(IProperty), typeof(IRuntimePropertyInfo))),
                                   };

            var configurators = new List<IExportFactory<IRuntimeModelElementConfigurator, RuntimeModelElementConfiguratorMetadata>>();

            var factory = new DefaultRuntimeModelElementFactory(constructors, configurators, new RuntimeTypeRegistry());
            return factory;
        }

        public IRuntimeModelElementFactory GetNullRuntimeModelElementFactory()
        {
            var factory = Substitute.For<IRuntimeModelElementFactory>();
            factory.TryCreateModelElement(Arg.Any<IModelConstructionContext>(), Arg.Any<object>())
                .Returns((INamedElement)null);
            return factory;
        }
    }
}