// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using global::MongoDB.Bson.Serialization.Conventions;
    using Kephas.Data.MongoDB.Application;
    using Kephas.Data.Store;
    using Kephas.Services;
    using Kephas.Testing.Composition;
    using Kephas.Threading.Tasks;
    using NSubstitute;

    public abstract class MongoTestBase : CompositionTestBase
    {
        static MongoTestBase()
        {
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);
        }

        protected MongoTestBase()
        {
            new MongoAppLifecycleBehavior()
                .BeforeAppInitializeAsync(Substitute.For<IContext>())
                .WaitNonLocking();
        }

        public override IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>(base.GetDefaultParts())
            {
                typeof(TestMongoDataStoreMatcher),
                typeof(TestMongoNamingStrategy),
            };
        }

        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>(base.GetDefaultConventionAssemblies())
            {
                typeof(IDataContext).GetTypeInfo().Assembly, /* Kephas.Data */
                typeof(MongoDataContext).GetTypeInfo().Assembly, /* Kephas.Data.MongoDB */
            };
        }

        public class TestMongoNamingStrategy : IMongoNamingStrategy
        {
            public string GetCollectionName(IDataContext dataContext, Type entityType)
            {
                var name = entityType.Name;
                if (name.EndsWith("MongoEntity"))
                {
                    name = name.Substring(0, name.Length - "MongoEntity".Length);
                }

                return name.ToLower();
            }
        }

        public class TestMongoDataStoreMatcher : IDataStoreMatcher
        {
            private IDataStore? dataStore;

            public (string? dataStoreName, bool canHandle) GetDataStoreName(Type entityType, IContext? context = null)
            {
                if (entityType.Name.EndsWith("MongoEntity"))
                {
                    return ("mongotest", true);
                }

                return (null, false);
            }

            public (IDataStore? dataStore, bool canHandle) GetDataStore(string dataStoreName, IContext? context = null)
            {
                if (dataStoreName == "mongotest")
                {
                    return (this.GetTestDataStore(), true);
                }

                return (null, false);
            }

            private IDataStore GetTestDataStore()
            {
                return this.dataStore ??= new DataStore(
                    "mongotest",
                    DataStoreKind.MongoDB.ToString(),
                    dataContextSettings: new DataContextSettings(this.GetTestConnectionString()));
            }

            private string GetTestConnectionString()
            {
                // For Azure, retryable writes are not supported.
                // Retryable writes are not supported. Please disable retryable writes by specifying "retrywrites=false" in the connection string or an equivalent driver specific config.
                return
                    @"mongodb://dev-unit-testing:dgRWXmr1UaQZWIhvu6e93F9TBuEK69O19uDe2gWXFE05NHWLRT7d5ycGpeGlwfWWSblmEs9m3XqrCYCquUoT9w==@dev-unit-testing.mongo.cosmos.azure.com:10255/test?ssl=true&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@dev-unit-testing@&retrywrites=false";
            }
        }
    }
}