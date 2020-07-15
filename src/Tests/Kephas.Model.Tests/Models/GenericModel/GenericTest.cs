// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Runtime;
    using Kephas.Testing.Model;
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
            var typeRegistry = container.GetExport<IRuntimeTypeRegistry>();
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var classifier = modelSpace.TryGetClassifier(typeRegistry.GetTypeInfo(typeof(IComplex<decimal, int>)));
            var complexClassifier = modelSpace.Classifiers.Single(c => c.Name == "Complex`2" && c.IsGenericTypeDefinition());

            Assert.IsNotNull(classifier);
            Assert.AreSame(complexClassifier, classifier.GenericTypeDefinition);
            Assert.IsTrue(modelSpace.Classifiers.Contains(classifier));
        }
    }
}