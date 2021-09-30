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
    using Kephas.Testing.Injection;
    using Kephas.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using NSubstitute;

    public abstract class MongoTestBase : InjectionTestBase
    {
        private const string MongoTestDataStoreName = "mongotest";

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

        public override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
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
                    return (MongoTestDataStoreName, true);
                }

                return (null, false);
            }

            public (IDataStore? dataStore, bool canHandle) GetDataStore(string dataStoreName, IContext? context = null)
            {
                if (dataStoreName == MongoTestDataStoreName)
                {
                    return (this.GetTestDataStore(), true);
                }

                return (null, false);
            }

            private IDataStore GetTestDataStore()
            {
                return this.dataStore ??= new DataStore(
                    MongoTestDataStoreName,
                    DataStoreKind.MongoDB.ToString(),
                    dataContextSettings: new DataContextSettings(this.GetTestConnectionString()));
            }

            private string GetTestConnectionString()
            {
                // For Azure, retryable writes are not supported.
                // Retryable writes are not supported. Please disable retryable writes by specifying "retrywrites=false" in the connection string or an equivalent driver specific config.
                var builder = new ConfigurationBuilder().AddUserSecrets<MongoTestBase>();
                var connectionString = builder.Build().GetConnectionString(MongoTestDataStoreName);
                return connectionString;
            }
        }
    }
}