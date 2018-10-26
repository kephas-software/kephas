// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientQueryConversionContextTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the client query conversion context test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Tests.Queries.Conversion
{
    using System.Collections.Generic;

    using Kephas.Data.Client.Queries.Conversion;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class ClientQueryConversionContextTest
    {
        [Test]
        public void Options_empty()
        {
            var context = new ClientQueryConversionContext(Substitute.For<IDataContext>());
            Assert.IsFalse(context.UseMemberAccessConvention);
        }

        [Test]
        public void Options_pascal_case()
        {
            var context = new ClientQueryConversionContext(Substitute.For<IDataContext>())
            {
                Options = new Dictionary<string, object> { { "UseMemberAccessConvention", true } }
            };

            Assert.IsTrue(context.UseMemberAccessConvention);
        }

        [Test]
        public void Options_camel_case()
        {
            var context = new ClientQueryConversionContext(Substitute.For<IDataContext>())
                              {
                                  Options = new Dictionary<string, object> { { "useMemberAccessConvention", true } }
                              };

            Assert.IsTrue(context.UseMemberAccessConvention);
        }
    }
}