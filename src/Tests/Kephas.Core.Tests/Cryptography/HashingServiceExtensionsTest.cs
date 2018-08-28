// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashingServiceExtensionsTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the hashing service extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Cryptography
{
    using System.Text;

    using Kephas.Cryptography;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class HashingServiceExtensionsTest
    {
        [Test]
        public void Hash()
        {
            var hashingService = Substitute.For<IHashingService>();
            var valueString = "123";
            var hashingContext = Substitute.For<IHashingContext>();
            var hashBytes = Encoding.UTF8.GetBytes("8910");
            hashingService.Hash(Arg.Any<byte[]>(), hashingContext).Returns(ci =>
                {
                    var value = Encoding.UTF8.GetString(ci.ArgAt<byte[]>(0));
                    return value == valueString ? hashBytes : null;
                });

            var hash = HashingServiceExtensions.Hash(hashingService, valueString, hashingContext);
            var hashString = Encoding.UTF8.GetString(hash);
            Assert.AreEqual("8910", hashString);
        }
    }
}