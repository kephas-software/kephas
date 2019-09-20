// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultIdGeneratorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default identifier generator test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Data;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultIdGeneratorTest
    {
        [Test]
        [TestCase(20, 1000)]
        [TestCase(50, 1000)]
        public async Task GenerateId_unique(int threads, int idCount)
        {
            var generator = new DefaultIdGenerator(new IdGeneratorSettings());

            var tasks = new List<Task<long[]>>();
            for (var i = 0; i < threads; i++)
            {
                tasks.Add(new Task<long[]>(
                    () =>
                        {
                            var list = new List<long>();
                            for (var j = 0; j < idCount; j++)
                            {
                                list.Add(generator.GenerateId());
                            }

                            return list.ToArray();
                        }));
            }

            tasks.ForEach(t => t.Start());
            var idLists = await Task.WhenAll(tasks);
            var allIds = idLists.SelectMany(l => l).ToList();
            var distinctIds = allIds.Distinct().ToList();

            Assert.AreEqual(allIds.Count, distinctIds.Count);
        }
    }
}