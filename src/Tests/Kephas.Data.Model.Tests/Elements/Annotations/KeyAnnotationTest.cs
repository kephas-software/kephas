﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyAnnotationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the key annotation test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Runtime;

namespace Kephas.Data.Model.Tests.Elements.Annotations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Data.Model.AttributedModel;
    using Kephas.Data.Model.Elements.Annotations;
    using Kephas.Model;
    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Elements;
    using Kephas.Model.Elements.Annotations;
    using Kephas.Reflection;
    using Kephas.Testing;
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class KeyAnnotationTest : TestBase
    {
        [Test]
        public void Configure()
        {
            var ambientServices = this.CreateAmbientServices();
            var injector = Substitute.For<IServiceProvider>();
            injector.Resolve<IAmbientServices>().Returns(ambientServices);
            var context = new ModelConstructionContext(injector);
            var modelSpace = new DefaultModelSpace(context);
            context.ModelSpace = modelSpace;

            var keyAttr = new KeyAttribute(new[] { "gigi" });
            var annotation = new KeyAnnotation(context, "gigi_key", keyAttr);

            var elementInfos = new List<IElementInfo>();
            elementInfos.AddRange(this.CreateClassifier(context, "Contact", properties: new[] { "Name", "gigi" }, annotations: new IAnnotation[] { annotation }));

            context.ElementInfos = elementInfos;

            var classifiers = modelSpace.ComputeClassifiers(context).ToList();
            var keys = classifiers.Single().Members.OfType<IKey>().ToList();
            Assert.AreEqual(1, keys.Count);

            var key = keys.Single();
            Assert.AreEqual("$gigi_key", key.Name);
        }

        private IList<IElementInfo> CreateClassifier(
            IModelConstructionContext context,
            string name,
            string[] properties,
            Func<IModelElement, bool> aspectFor = null,
            IAnnotation[] annotations = null)
        {
            var elementInfos = new List<IElementInfo>();
            var classifier = new Classifier(context, name);
            elementInfos.Add(classifier);

            foreach (var propName in properties)
            {
                var property = new Property(context, propName);
                property.ValueType = context.RuntimeTypeRegistry.GetTypeInfo(typeof(string));
                ((IConstructibleElement)classifier).AddMember(property);
            }

            if (aspectFor != null)
            {
                var annotation = new AspectAnnotation(context, "aspectFor", aspectFor);
                ((IConstructibleElement)classifier).AddMember(annotation);
            }

            if (annotations != null)
            {
                foreach (var annotation in annotations)
                {
                    ((IConstructibleElement)classifier).AddMember(annotation);
                }
            }

            return elementInfos;
        }
    }
}