// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConflictingBaseMembersTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the conflicting base members test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Models.ConflictingBaseMembersModel
{
    using System.Linq;
    using System.Threading.Tasks;

    using NUnit.Framework;

    [TestFixture]
    public class ConflictingBaseMembersTest : ModelTestBase
    {
        [Test]
        public async Task InitializeAsync_conflicting_base_members_solved()
        {
            var container = this.CreateContainerForModel(typeof(INamed), typeof(IIdentifiable), typeof(EntityBase), typeof(NamedEntityBase));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var namedClassifier = modelSpace.Classifiers.Single(c => c.Name == "NamedEntityBase");
            var idClassifier = modelSpace.Classifiers.Single(c => c.Name == "EntityBase");

            var idMember = idClassifier.GetMember("Id");
            var derivedIdMember = namedClassifier.GetMember("Id");
            Assert.AreSame(idMember, derivedIdMember);
        }
    }
}