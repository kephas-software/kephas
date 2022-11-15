// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiamondInheritanceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the diamond inheritance test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Models.DiamondInheritanceModel
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Testing.Model;
    using NUnit.Framework;

    [TestFixture]
    public class DiamondInheritanceTest : ModelTestBase
    {
        [Test]
        public async Task InitializeAsync_members_inherited_once()
        {
            var container = this.CreateInjectorForModel(typeof(INamed), typeof(IUniquelyNamed), typeof(IParameter), typeof(IAppParameter));
            var provider = container.Resolve<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var namedClassifier = modelSpace.Classifiers.Single(c => c.Name == "Named");
            var parameterClassifier = modelSpace.Classifiers.Single(c => c.Name == "Parameter");
            var uniqueClassifier = modelSpace.Classifiers.Single(c => c.Name == "UniquelyNamed");
            var appParamClassifier = modelSpace.Classifiers.Single(c => c.Name == "AppParameter");

            Assert.AreEqual(2, appParamClassifier.BaseMixins.Count());
            Assert.IsFalse(appParamClassifier.BaseMixins.Any(m => m == namedClassifier));
            Assert.IsTrue(appParamClassifier.BaseMixins.Any(m => m == parameterClassifier));
            Assert.IsTrue(appParamClassifier.BaseMixins.Any(m => m == uniqueClassifier));
            Assert.AreEqual(2, appParamClassifier.Members.Count());
            Assert.AreSame(namedClassifier.Members.First(m => m.Name == "Name"), appParamClassifier.Members.First(m => m.Name == "Name"));
        }
    }
}