// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTypeRegistryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Reflection.Dynamic
{
    using Kephas.Reflection.Dynamic;
    using NUnit.Framework;

    [TestFixture]
    public class DynamicTypeRegistryTest
    {
        [Test]
        public void Constructor()
        {
            var typeRegistry = new DynamicTypeRegistry();
            CollectionAssert.IsEmpty(typeRegistry.Types);
        }
    }
}