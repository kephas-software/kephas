// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the generic test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Models.GenericModel
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class GenericTest : ModelTestBase
    {
        [Test]
        public async Task InitializeAsync_generic_inheritance()
        {
            var container = this.CreateContainerForModel(typeof(IComplex<,>), typeof(IIntComplex), typeof(IFloatComplex));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var complexClassifier = modelSpace.Classifiers.Single(c => c.Name == "Complex`2" && c.IsGenericTypeDefinition());
            var intComplexClassifier = modelSpace.Classifiers.Single(c => c.Name == "IntComplex");
            var floatComplexClassifier = modelSpace.Classifiers.Single(c => c.Name == "FloatComplex");

            Assert.IsTrue(complexClassifier.IsGenericTypeDefinition());
            Assert.IsFalse(intComplexClassifier.IsGenericType());
            Assert.IsFalse(floatComplexClassifier.IsGenericType());
        }

        [Test]
        public async Task TryGetClassifier_generic_inheritance()
        {
            var container = this.CreateContainerForModel(typeof(IComplex<,>), typeof(IIntComplex), typeof(IFloatComplex));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var classifier = modelSpace.TryGetClassifier(typeof(IComplex<decimal, int>).AsRuntimeTypeInfo());
            var complexClassifier = modelSpace.Classifiers.Single(c => c.Name == "Complex`2" && c.IsGenericTypeDefinition());

            Assert.IsNotNull(classifier);
            Assert.AreSame(complexClassifier, classifier.GenericTypeDefinition);
            Assert.IsTrue(modelSpace.Classifiers.Contains(classifier));
        }
    }
}