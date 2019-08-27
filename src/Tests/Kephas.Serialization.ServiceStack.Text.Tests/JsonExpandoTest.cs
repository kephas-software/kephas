// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonExpandoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the JSON expando test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Kephas.Dynamic;
    using Kephas.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class JsonExpandoTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            var typeResolver = Substitute.For<ITypeResolver>();
            typeResolver.ResolveType(Arg.Any<string>(), Arg.Any<bool>()).Returns((Type)null);
            var configurator = new DefaultJsonSerializerConfigurator(typeResolver);
            configurator.ConfigureJsonSerialization();
        }

        [Test, Order(1)]
        public void Deserialize_simple()
        {
            dynamic expando = new JsonExpando(@"{ ""some"": ""kind"", ""of"": { ""dynamic"": ""nesting : or [ string ]""}, ""array"": [1, 2, { ""in"": ""array"" }, [ 4, true ]]}");
            Assert.AreEqual("kind", expando.some);
            Assert.AreEqual("nesting : or [ string ]", expando.of.dynamic);

            IList<object> array = expando.array;
            Assert.IsInstanceOf<JsonExpando>(array[2]);

            dynamic nestedObj = array[2];
            Assert.AreEqual("array", nestedObj["in"]);
        }

        [Test, Order(2)]
        public void Deserialize_complex()
        {
            var expando = this.GetExpandoJson("ComplexJson");
            IList<object> myFlow = (IList<object>)expando["my-flow"];
            Assert.AreEqual(2, myFlow.Count);
            Assert.IsInstanceOf<JsonExpando>(myFlow[0]);
            Assert.IsInstanceOf<JsonExpando>(myFlow[1]);
        }

        private IExpando GetExpandoJson(string resourceName)
        {
            var assembly = this.GetType().Assembly;
            var assemblyNamespace = assembly.GetName().Name;
            var assemblyResourceName = $"{assemblyNamespace}.Resources.{resourceName}.json";
            using (var data = assembly.GetManifestResourceStream(assemblyResourceName))
            using (var textReader = new StreamReader(data))
            {
                var str = textReader.ReadToEnd();
                return new JsonExpando(str);
            }
        }
    }
}