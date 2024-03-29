﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the serialization test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Testing;
    using NSubstitute;

    public class SerializationTestBase : TestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>(base.GetAssemblies())
                                {
                                    typeof(ISerializationService).Assembly,
                                    typeof(DefaultTypeResolver).Assembly,   // Kephas.Reflection
                                    typeof(JsonSerializer).Assembly,        // Kephas.Serialization.NewtonsoftJson
                                };
            return assemblies;
        }

        public virtual ISerializationContext GetSerializationContext(Type? rootObjectType = null, Action<ISerializationContext>? options = null)
        {
            var context = new SerializationContext(
                    Substitute.For<IServiceProvider>(),
                    Substitute.For<ISerializationService>())
            {
                RootObjectType = rootObjectType,
            };

            options?.Invoke(context);
            return context;
        }

        protected static DefaultJsonSerializerSettingsProvider GetJsonSerializerSettingsProvider(IInjectableFactory? injectableFactory = null)
        {
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(
                new DefaultTypeResolver(() => AppDomain.CurrentDomain.GetAssemblies()),
                new RuntimeTypeRegistry(),
                injectableFactory,
                Substitute.For<ILogManager>());
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
