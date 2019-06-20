// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultProjectedTypeResolverTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default projected type resolver test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Model
{
    using Kephas.Model;
    using Kephas.Model.AttributedModel;
    using Kephas.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultProjectedTypeResolverTest
    {
        [Test]
        public void ResolveProjectedType_annotated_type_name()
        {
            var typeResolver = Substitute.For<ITypeResolver>();
            typeResolver.ResolveType("my type", Arg.Any<bool>()).Returns(typeof(string));

            var resolver = new DefaultProjectedTypeResolver(typeResolver);
            var resolvedType = resolver.ResolveProjectedType(typeof(MyTypeProjection));
            Assert.AreSame(resolvedType, typeof(string));
        }

        [Test]
        public void ResolveProjectedType_annotated_type()
        {
            var typeResolver = Substitute.For<ITypeResolver>();

            var resolver = new DefaultProjectedTypeResolver(typeResolver);
            var resolvedType = resolver.ResolveProjectedType(typeof(IntProjection));
            Assert.AreSame(resolvedType, typeof(int));
        }

        [Test]
        public void ResolveProjectedType_not_annotated_type()
        {
            var typeResolver = Substitute.For<ITypeResolver>();

            var resolver = new DefaultProjectedTypeResolver(typeResolver);
            var resolvedType = resolver.ResolveProjectedType(typeof(string));
            Assert.AreSame(resolvedType, typeof(string));
        }

        [ProjectionFor("my type")]
        public class MyTypeProjection
        {
        }

        [ProjectionFor(typeof(int))]
        public class IntProjection
        {
        }
    }
}