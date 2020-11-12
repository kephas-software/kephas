// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoJsonConverterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using NUnit.Framework;

    [TestFixture]
    public class ExpandoJsonConverterTest : SerializationTestBase
    {
        [Test]
        public async Task SerializeAsync_Expando()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new Expando
            {
                ["Description"] = "John Doe",
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""Description"":""John Doe""}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_IExpando()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(@"{""Description"":""John Doe""}", GetSerializationContext(typeof(IExpando)));

            Assert.IsInstanceOf<Expando>(obj);
            var expando = (Expando)obj;
            Assert.AreEqual("John Doe", expando["Description"]);
        }

        [Test]
        public async Task SerializeAsync_ExpandoEntity()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new ExpandoEntity
            {
                Description = "John Doe",
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ExpandoJsonConverterTest+ExpandoEntity"",""description"":""John Doe""}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_ExpandoEntity()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ExpandoJsonConverterTest+ExpandoEntity"",""description"":""John Doe""}");

            Assert.IsInstanceOf<ExpandoEntity>(obj);
            var expando = (ExpandoEntity)obj;
            Assert.AreEqual("John Doe", expando.Description);
        }

        [Test]
        public async Task SerializeAsync_Wrapper()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new Wrapper { Value = new ExpandoEntity { Description = "John Doe" } };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ExpandoJsonConverterTest+Wrapper"",""value"":{""$type"":""Kephas.Serialization.Json.Tests.Converters.ExpandoJsonConverterTest+ExpandoEntity"",""description"":""John Doe""}}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_Wrapper()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ExpandoJsonConverterTest+Wrapper"",""value"":{""$type"":""Kephas.Serialization.Json.Tests.Converters.ExpandoJsonConverterTest+ExpandoEntity"",""description"":""John Doe""}}");

            Assert.IsInstanceOf<Wrapper>(obj);
            var wrapper = (Wrapper)obj;

            Assert.IsInstanceOf<ExpandoEntity>(wrapper.Value);
            var expando = (ExpandoEntity)wrapper.Value;
            Assert.AreEqual("John Doe", expando.Description);
        }

        [Test]
        public async Task DeserializeAsync_Typeless_Expando()
        {
            var typelessJson = @"
{
  ""connection-strategy"": {
    ""match"": {
            ""script-type"": {
                ""sh"": ""DemoTasks/ConnectionStrategies/Shell/ProxyConnection""
            }
        },
    ""override"": {
      ""gateway"": {
        ""ref"": ""Systems-with-gateway-wsdev1/wsdev1""
      }
    },
    ""connection-flow"": [
      {
        ""name"": ""one""
      },
      {
        ""name"": ""two""
      }
    ]
  }
}";
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(typelessJson, this.GetSerializationContext(typeof(IExpando)));

            Assert.IsInstanceOf<Expando>(obj);
            var expando = (Expando)obj;
            var strategy = (IIndexable)expando["connection-strategy"];
            Assert.IsNotNull(strategy);
            var connectionFlow = (IList<object?>)strategy["connection-flow"];
            Assert.AreEqual(2, connectionFlow.Count);
        }

        [Test]
        public async Task DeserializeAsync_Typed_Expando()
        {
            var typedJson = @"
{
    ""instances"": {
        ""JobServer"": {
            ""autoStart"": true,
            ""enabledFeatures"": [ ""web"" ],
            ""host"": {
                ""urls"": [
                    {
                        ""url"": ""http://*:1234"",
                        ""host"": ""localhost""
                    },
                    {
                        ""url"": ""https://*:1235"",
                        ""host"": ""localhost"",
                        ""certificate"": ""my-server""
                    }
                ]
            },
            ""otherFolder"": ""MyOtherFolder""
        },
        ""WebServer"": {
            ""autoStart"": true,
            ""enabledFeatures"": [ ""jobs-client"", ""web"" ],
            ""host"": {
                ""urls"": [
                    {
                        ""url"": ""http://*:2345"",
                        ""host"": ""localhost""
                    },
                    {
                        ""url"": ""https://*:2346"",
                        ""host"": ""localhost"",
                        ""certificate"": ""my-server""
                    }
                ]
            },
            ""otherFolder"": ""TheOtherFolder""
        }
    },
    ""setupCommands"": [
        ""installPlugin Plugin1 @latest"",
        ""installPlugin Plugin2 @latest""
    ]
}";
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(typedJson, this.GetSerializationContext(typeof(SystemSettings)));

            Assert.IsInstanceOf<SystemSettings>(obj);
            var systemSettings = (SystemSettings)obj;
            Assert.AreEqual(2, systemSettings.Instances.Count);
            Assert.AreEqual(2, systemSettings.SetupCommands.Length);
        }

        public class ExpandoEntity : Expando
        {
            public string Description { get; set; }
        }

        public class Wrapper
        {
            public IExpando Value { get; set; }
        }

        /// <summary>
        /// Settings for the application runtime.
        /// </summary>
        public class SystemSettings : Expando
        {
            /// <summary>
            /// Gets or sets the settings for the application instances.
            /// </summary>
            public IDictionary<string, AppSettings> Instances { get; } = new Dictionary<string, AppSettings>(StringComparer.InvariantCultureIgnoreCase);

            /// <summary>
            /// Gets or sets the commands to be executed upon startup, when the application is started for the first time.
            /// </summary>
            /// <remarks>
            /// The application will take care to remove the executed commands from this list once they were executed.
            /// </remarks>
            public object[]? SetupCommands { get; set; }
        }

        /// <summary>
        /// Settings for the application instances.
        /// </summary>
        public class AppSettings : Expando
        {
            /// <summary>
            /// Gets or sets a value indicating whether application instances should be automatically started.
            /// </summary>
            public bool AutoStart { get; set; } = true;

            /// <summary>
            /// Get or sets the application startup args.
            /// </summary>
            public Expando? Args { get; set; }

            /// <summary>
            /// Gets or sets the environment variables to set for the child process.
            /// </summary>
            public Expando? EnvironmentVariables { get; set; }

            /// <summary>
            /// Gets or sets the features which are enabled by the application instance.
            /// </summary>
            /// <remarks>
            /// Required features are enabled by default and cannot be disabled.
            /// Also, if a feature is enabled, all dependencies are enabled, too.
            /// </remarks>
            public string[]? EnabledFeatures { get; set; }

            /// <summary>
            /// Gets or sets the commands to be executed upon startup, when the application is started for the first time.
            /// </summary>
            /// <remarks>
            /// The application will take care to remove the executed commands from this list once they were executed.
            /// </remarks>
            public object[]? SetupCommands { get; set; }

            /// <summary>
            /// Gets or sets the commands to be executed upon startup, each time the application is started.
            /// </summary>
            public object[]? StartupCommands { get; set; }

            /// <summary>
            /// Gets or sets the commands to be executed upon shutdown, each time the application is stopped.
            /// </summary>
            public object[]? ShutdownCommands { get; set; }

            /// <summary>
            /// Gets or sets the settings for hosting.
            /// </summary>
            public HostSettings? Host { get; set; }
        }

        /// <summary>
        /// Settings for the hosting services.
        /// </summary>
        public class HostSettings : Expando
        {
            /// <summary>
            /// Gets or sets a value indicating whether the application should run as a service (Windows service or Unix daemon).
            /// </summary>
            public bool RunAsService { get; set; }

            /// <summary>
            /// Gets or sets the urls to listen to.
            /// </summary>
            public UrlSettings[]? Urls { get; set; }
        }

        /// <summary>
        /// Settings for the endpoint URL.
        /// </summary>
        public class UrlSettings : Expando
        {
            /// <summary>
            /// Gets or sets the endpoint URL template.
            /// </summary>
            public string? Url { get; set; }

            /// <summary>
            /// Gets or sets the name of the certificate used for this endpoint. Typically, the certificate is retrieved using the
            /// </summary>
            public string? Certificate { get; set; }
        }
    }
}