// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MixinInheritanceTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the mixin inheritance test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Models.MixinInheritanceModel
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Elements.Annotations;

    using NUnit.Framework;

    [TestFixture]
    public class MixinInheritanceTest : ModelTestBase
    {
        [Test]
        public async Task InitializeAsync_mixin_inheritance()
        {
            var container = this.CreateContainerForModel(typeof(INamed), typeof(IUniquelyNamed), typeof(IParameter));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            // TODO

            var modelSpace = provider.GetModelSpace();
            var namedClassifier = modelSpace.Classifiers.Single(c => c.Name == "Named");
            var uniqueClassifier = modelSpace.Classifiers.Single(c => c.Name == "UniquelyNamed");

            Assert.IsTrue(namedClassifier.IsMixin);
            Assert.AreEqual(1, namedClassifier.Annotations.OfType<MixinAnnotation>().Count());
            Assert.IsTrue(uniqueClassifier.IsMixin);
            Assert.AreEqual(1, uniqueClassifier.Annotations.OfType<MixinAnnotation>().Count());
            Assert.AreSame(namedClassifier, uniqueClassifier.BaseMixins.Single());
            Assert.IsNull(uniqueClassifier.BaseClassifier);
        }

        [Test]
        public async Task InitializeAsync_mixin_not_inherited()
        {
            var container = this.CreateContainerForModel(typeof(INamed), typeof(IUniquelyNamed), typeof(IParameter));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            // TODO

            var modelSpace = provider.GetModelSpace();
            var paramClassifier = modelSpace.Classifiers.Single(c => c.Name == "Parameter");

            Assert.IsFalse(paramClassifier.IsMixin);
            Assert.IsFalse(paramClassifier.Annotations.Any());
        }

        [Test]
        public async Task InitializeAsync_aspect_inheritance_model()
        {
            var container = this.CreateContainerForModel(typeof(IStringAspect), typeof(IStringBuilderAspect));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var stringClassifier = modelSpace.Classifiers.Single(c => c.Name == "StringAspect");
            var sbClassifier = modelSpace.Classifiers.Single(c => c.Name == "StringBuilderAspect");

            Assert.IsTrue(stringClassifier.IsMixin);
            Assert.IsTrue(stringClassifier.IsAspect);
            Assert.AreEqual(1, stringClassifier.Annotations.OfType<AspectAnnotation>().Count());
            Assert.IsTrue(sbClassifier.IsMixin);
            Assert.IsTrue(sbClassifier.IsAspect);
            Assert.AreEqual(1, sbClassifier.Annotations.OfType<AspectAnnotation>().Count());
        }
    }
}