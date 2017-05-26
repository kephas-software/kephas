// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultModelSpaceTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default model space test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Elements;
    using Kephas.Model.Elements.Annotations;
    using Kephas.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultModelSpaceTest
    {
        [Test]
        public void ComputeDimensions_2_dims()
        {
            var context = new ModelConstructionContext(Substitute.For<IAmbientServices>());
            var modelSpace = new DefaultModelSpace(context);
            context.ModelSpace = modelSpace;

            var elementInfos = new List<IElementInfo>();
            elementInfos.AddRange(this.CreateModelDimension(context, "D1", isAggregatable: false, index: 0, dimensionElements: new [] { "E1", "E2" }));
            elementInfos.AddRange(this.CreateModelDimension(context, "D2", isAggregatable: true, index: 1, dimensionElements: new[] { "F1", "F2" }));

            context.ElementInfos = elementInfos;

            var dimensions = modelSpace.ComputeDimensions(context);
            Assert.AreEqual(2, dimensions.Length);
            Assert.AreEqual(2, dimensions[0].Elements.Count());
            Assert.AreEqual(2, dimensions[1].Elements.Count());

            Assert.IsTrue(dimensions.Cast<IConstructableElement>().All(d => d.ConstructionState.IsCompletedSuccessfully));
            Assert.IsTrue(dimensions.SelectMany(d => d.Elements).Cast<IConstructableElement>().All(d => d.ConstructionState.IsCompletedSuccessfully));

            var dimDictionary = dimensions.ToDictionary(d => d.Name, d => d);
            var elemDictionary = dimensions.SelectMany(d => d.Elements).ToDictionary(e => e.Name, e => e);
            Assert.AreSame(elemDictionary["E1"].DeclaringDimension, dimDictionary["D1"]);
            Assert.AreSame(elemDictionary["E2"].DeclaringDimension, dimDictionary["D1"]);
            Assert.AreSame(elemDictionary["F1"].DeclaringDimension, dimDictionary["D2"]);
            Assert.AreSame(elemDictionary["F2"].DeclaringDimension, dimDictionary["D2"]);
            Assert.AreEqual("^D1:E1", elemDictionary["E1"].FullName);
        }

        [Test]
        public void ComputeProjections_2_dims()
        {
            var context = new ModelConstructionContext(Substitute.For<IAmbientServices>());
            var modelSpace = new DefaultModelSpace(context);
            context.ModelSpace = modelSpace;

            var elementInfos = new List<IElementInfo>();
            elementInfos.AddRange(this.CreateModelDimension(context, "D1", isAggregatable: false, index: 0, dimensionElements: new[] { "E1", "E2" }));
            elementInfos.AddRange(this.CreateModelDimension(context, "D2", isAggregatable: true, index: 1, dimensionElements: new[] { "F1", "F2" }));

            context.ElementInfos = elementInfos;

            var dimensions = modelSpace.ComputeDimensions(context);
            var projections = modelSpace.ComputeProjections(context, dimensions);

            Assert.AreEqual(6, projections.Count);
            Assert.IsTrue(projections.Cast<IConstructableElement>().All(d => d.ConstructionState.IsCompletedSuccessfully));

            var projDictionary = projections.ToDictionary(p => p.Name, p => p);
            Assert.IsFalse(projDictionary[":E1:F1"].IsAggregated);
            Assert.IsFalse(projDictionary[":E1:F2"].IsAggregated);
            Assert.IsFalse(projDictionary[":E2:F1"].IsAggregated);
            Assert.IsFalse(projDictionary[":E2:F2"].IsAggregated);
            Assert.IsTrue(projDictionary[":E1"].IsAggregated);
            Assert.IsTrue(projDictionary[":E2"].IsAggregated);
            Assert.AreSame(projDictionary[":E1:F1"].AggregatedProjection, projDictionary[":E1"]);
            Assert.AreSame(projDictionary[":E1:F2"].AggregatedProjection, projDictionary[":E1"]);
            Assert.AreSame(projDictionary[":E2:F1"].AggregatedProjection, projDictionary[":E2"]);
            Assert.AreSame(projDictionary[":E2:F2"].AggregatedProjection, projDictionary[":E2"]);
            Assert.AreEqual(":E2:F2", projDictionary[":E2:F2"].FullName);
        }

        [Test]
        public void ComputeClassifiers_classifier_with_two_properties()
        {
            var context = new ModelConstructionContext(Substitute.For<IAmbientServices>());
            var modelSpace = new DefaultModelSpace(context);
            context.ModelSpace = modelSpace;

            var elementInfos = new List<IElementInfo>();
            elementInfos.AddRange(this.CreateClassifier(context, "Contact", properties: new[] { "Name", "Age" }));

            context.ElementInfos = elementInfos;

            var classifiers = modelSpace.ComputeClassifiers(context).ToList();
            Assert.AreEqual(1, classifiers.Count);

            var classifier = classifiers.Single();
            Assert.AreEqual(2, classifier.Properties.Count());
        }

        [Test]
        public void ComputeClassifiers_aspect_classifier()
        {
            var context = new ModelConstructionContext(Substitute.For<IAmbientServices>());
            var modelSpace = new DefaultModelSpace(context);
            context.ModelSpace = modelSpace;

            var elementInfos = new List<IElementInfo>();
            elementInfos.AddRange(this.CreateClassifier(context, "ContactAspect", properties: new[] { "Employee" }, aspectFor: c => c.Name == "Contact"));

            context.ElementInfos = elementInfos;

            var classifiers = modelSpace.ComputeClassifiers(context).ToList();
            Assert.AreEqual(1, classifiers.Count);

            var classifier = classifiers.Single();
            Assert.IsTrue(classifier.IsAspect);
            Assert.IsTrue(classifier.IsMixin);

            var aspectOfClassifier = Substitute.For<IClassifier>();
            aspectOfClassifier.Name.Returns("Contact");
            Assert.IsTrue(classifier.IsAspectOf(aspectOfClassifier));

            var nonAspectOfClassifier = Substitute.For<IClassifier>();
            nonAspectOfClassifier.Name.Returns("NonContact");
            Assert.IsFalse(classifier.IsAspectOf(nonAspectOfClassifier));
        }

        [Test]
        public void ComputeClassifiers_classifier_with_aspect()
        {
            var context = new ModelConstructionContext(Substitute.For<IAmbientServices>());
            var modelSpace = new DefaultModelSpace(context);
            context.ModelSpace = modelSpace;

            var elementInfos = new List<IElementInfo>();
            elementInfos.AddRange(this.CreateClassifier(context, "Contact", properties: new[] { "Name", "Age" }));
            elementInfos.AddRange(this.CreateClassifier(context, "ContactAspect", properties: new[] { "Employee" }, aspectFor: c => c.Name == "Contact"));

            context.ElementInfos = elementInfos;

            var classifiers = modelSpace.ComputeClassifiers(context).ToList();
            Assert.AreEqual(2, classifiers.Count);

            var classifier = classifiers.Single(c => c.Name == "Contact");
            Assert.AreEqual(3, classifier.Properties.Count());
        }

        private IList<IElementInfo> CreateModelDimension(IModelConstructionContext context, string dimensionName, bool isAggregatable, int index, IEnumerable<string> dimensionElements)
        {
            var elementInfos = new List<IElementInfo>();
            var dimension = new ModelDimension(context, dimensionName)
            {
                IsAggregatable = isAggregatable,
                Index = index
            };
            elementInfos.Add(dimension);

            foreach (var elementName in dimensionElements)
            {
                var element = new ModelDimensionElement(context, elementName) { DimensionName = dimensionName };
                elementInfos.Add(element);
            }

            return elementInfos;
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
                ((IConstructableElement)classifier).AddMember(property);
            }

            if (aspectFor != null)
            {
                var annotation = new AspectAnnotation(context, "aspectFor", aspectFor);
                ((IConstructableElement)classifier).AddMember(annotation);
            }

            if (annotations != null)
            {
                foreach (var annotation in annotations)
                {
                    ((IConstructableElement)classifier).AddMember(annotation);
                }
            }

            return elementInfos;
        }
    }
}
