﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextCacheTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context cache test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Caching
{
    using System;
    using System.Collections.Generic;

    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataContextCacheTest
    {
        [Test]
        public void Indexer_get()
        {
            var cache = new DataContextCache();

            var entity = "123";
            var eInfo = new EntityEntry(entity);
            cache[eInfo.Id] = eInfo;

            Assert.AreSame(eInfo, cache[eInfo.Id]);
        }

        [Test]
        public void Indexer_set_not_null()
        {
            var cache = new DataContextCache();
            Assert.Throws<ArgumentNullException>(() => cache[1] = null);
            Assert.Throws<ArgumentNullException>(() => cache[null] = Substitute.For<IEntityEntry>());
        }

        [Test]
        public void Indexer_set()
        {
            var items = new Dictionary<object, IEntityEntry>();
            var mappings = new Dictionary<object, IEntityEntry>();
            var cache = new TestDataContextCache(items, mappings);

            var entity = "123";
            var eInfo = new EntityEntry(entity);
            cache[eInfo.Id] = eInfo;

            Assert.AreSame(eInfo, items[eInfo.Id]);
            Assert.AreSame(eInfo, mappings[entity]);
        }

        [Test]
        public void Indexer_set_key_and_entityentry_id_must_match()
        {
            var cache = new DataContextCache();

            var entity = "123";
            var eInfo = new EntityEntry(entity);
            Assert.Throws<ArgumentException>(() => cache[Guid.NewGuid()] = eInfo);
        }

        [Test]
        public void Indexer_set_replace()
        {
            var items = new Dictionary<object, IEntityEntry>();
            var mappings = new Dictionary<object, IEntityEntry>();
            var cache = new TestDataContextCache(items, mappings);

            var entity = "123";
            var entityReplace = "234";
            var eInfo = new EntityEntry(entity);
            var eReplace = Substitute.For<IEntityEntry>();
            eReplace.Id.Returns(eInfo.Id);
            eReplace.Entity.Returns(entityReplace);

            cache[eInfo.Id] = eInfo;
            cache[eInfo.Id] = eReplace;

            Assert.AreSame(eReplace, items[eInfo.Id]);
            Assert.AreSame(eReplace, mappings[entityReplace]);
            Assert.IsFalse(mappings.ContainsKey(entity));
        }

        public class TestDataContextCache : DataContextCache
        {
            public TestDataContextCache(IDictionary<object, IEntityEntry> items, IDictionary<object, IEntityEntry> mappings)
                : base(items, mappings)
            {
            }
        }
    }
}