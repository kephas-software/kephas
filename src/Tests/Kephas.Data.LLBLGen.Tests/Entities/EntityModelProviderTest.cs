// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityModelProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the llbl generate entity model provider test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Tests.Entities
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Data.LLBLGen.Entities;
    using Kephas.Data.LLBLGen.Entities.Composition;

    using NUnit.Framework;

    [TestFixture]
    public class EntityModelProviderTest
    {
        [Test]
        public void GetModelTypeInfos_empty()
        {
            var provider =
                new EntityModelProvider(new List<IExportFactory<IEntityFactory, EntityFactoryMetadata>>());
            var typeInfos = provider.GetModelTypeInfos();
            Assert.AreEqual(0, typeInfos.Count());
        }
    }
}