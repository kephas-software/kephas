// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyExpandoTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lazy expando test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Dynamic
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class LazyExpandoTest
    {
        [Test]
        public void Indexer_get()
        {
            var expando = new LazyExpando(s => s);
            Assert.AreEqual("hello", expando["hello"]);
        }

        [Test]
        public void Indexer_get_no_value_resolver()
        {
            var expando = new LazyExpando();
            Assert.IsNull(expando["hello"]);
        }

        [Test]
        public void Indexer_circular_reference_exception()
        {
            var expando = new CircularExpando();
            expando.ValueResolver = s => s == "a" ? expando["b"] :
                                         s == "b" ? expando["a"] : null;

            Assert.Throws<CircularDependencyException>(() =>
                {
                    var x = expando["a"];
                });
        }

        [Test]
        public void Indexer_circular_reference_exception_ensure_not_locked()
        {
            var expando = new CircularExpando();
            expando.ValueResolver = s => s == "a" ? expando["b"] :
                                         s == "b" ? expando["a"] : null;

            Assert.Throws<CircularDependencyException>(() =>
                {
                    var x = expando["a"];
                });

            expando.ValueResolver = s => "hello";
            Assert.AreEqual("hello", expando["a"]);
        }

        public class CircularExpando : LazyExpando
        {
            public new Func<string, object> ValueResolver
            {
                get => base.ValueResolver;
                set => base.ValueResolver = value;
            }
        }
    }
}