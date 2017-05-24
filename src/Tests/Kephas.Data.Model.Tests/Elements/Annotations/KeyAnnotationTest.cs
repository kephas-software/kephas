// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyAnnotationTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the key annotation test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class KeyAnnotationTest
    {
        [Test]
        public void Configure()
        {
            var context = new ModelConstructionContext(Substitute.For<IAmbientServices>());
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
                property.PropertyType = typeof(string).AsRuntimeTypeInfo();
                ((IWritableNamedElement)classifier).AddMember(property);
            }

            if (aspectFor != null)
            {
                var annotation = new AspectAnnotation(context, "aspectFor", aspectFor);
                ((IWritableNamedElement)classifier).AddMember(annotation);
            }

            if (annotations != null)
            {
                foreach (var annotation in annotations)
                {
                    ((IWritableNamedElement)classifier).AddMember(annotation);
                }
            }

            return elementInfos;
        }
    }
}