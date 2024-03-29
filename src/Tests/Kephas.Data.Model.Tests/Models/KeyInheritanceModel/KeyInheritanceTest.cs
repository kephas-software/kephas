﻿namespace Kephas.Data.Model.Tests.Models.KeyInheritanceModel
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Data.Model.Elements.Annotations;
    using Kephas.Model;
    using Kephas.Testing.Model;
    using NUnit.Framework;

    [TestFixture]
    public class KeyInheritanceTest : DataModelTestBase
    {
        [Test]
        public async Task InitializeAsync_key_inheritance()
        {
            var container = this.CreateServicesBuilder()
                .WithModelElements(typeof(IUniqueName), typeof(IUniqueGuid), typeof(IPlugin))
                .BuildWithDependencyInjection();
            var provider = container.Resolve<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var nameClassifier = modelSpace.Classifiers.Single(c => c.Name == "UniqueName");
            var guidClassifier = modelSpace.Classifiers.Single(c => c.Name == "UniqueGuid");
            var pluginClassifier = modelSpace.Classifiers.OfType<IEntityType>().Single(c => c.Name == "Plugin");

            var nameKey = nameClassifier.Members.OfType<IKey>().Single();
            var guidKey = guidClassifier.Members.OfType<IKey>().Single(k => k.Name == "$Guid");
            Assert.AreEqual(2, pluginClassifier.Annotations.OfType<KeyAnnotation>().Count());
            Assert.AreEqual(2, pluginClassifier.Keys.Count());
            Assert.IsTrue(pluginClassifier.Keys.Contains(nameKey));
            Assert.IsTrue(pluginClassifier.Keys.Contains(guidKey));
        }
    }
}