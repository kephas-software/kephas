// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the serialization test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Serialization.Json.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Testing.Composition;
    using NSubstitute;

    public class SerializationTestBase : InjectionTestBase
    {
        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            var assemblies = new List<Assembly>(base.GetDefaultConventionAssemblies())
                                {
                                    typeof(JsonSerializer).Assembly,      // Kephas.Serialization.NewtonsoftJson
                                };
            return assemblies;
        }

        public virtual ISerializationContext GetSerializationContext(Type? rootObjectType = null, Action<ISerializationContext>? options = null)
        {
            var context = new SerializationContext(
                    Substitute.For<IInjector>(),
                    Substitute.For<ISerializationService>())
            {
                RootObjectType = rootObjectType,
            };
            
            options?.Invoke(context);
            return context;
        }

        protected static DefaultJsonSerializerSettingsProvider GetJsonSerializerSettingsProvider()
        {
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(
                new DefaultTypeResolver(() => AppDomain.CurrentDomain.GetAssemblies()), new RuntimeTypeRegistry(), Substitute.For<ILogManager>());
            return settingsProvider;
        }

        protected Stream GetJson(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var ns = "Kephas.Serialization.Json.Tests"; // assembly.GetName().Name;
            var embeddedJson = $"{ns}.EmbeddedResources.{fileName}";
            return assembly.GetManifestResourceStream(embeddedJson);
        }
    }
}
