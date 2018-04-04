// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Base64DataTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the base 64 data test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Text.Encoding
{
    using System;
    using System.Text;

    using Kephas.Text.Encoding;

    using NUnit.Framework;

    [TestFixture]
    public class Base64DataTest
    {
        [Test]
        public void ToBase64String_no_mime_type()
        {
            var base64String = Base64Data.ToBase64String("Hi there");
            Assert.AreEqual("SGkgdGhlcmU=", base64String);
        }

        [Test]
        public void ToBase64String_with_mime_type()
        {
            var base64String = Base64Data.ToBase64String("Hi there", "application/json");
            Assert.AreEqual("application/json, SGkgdGhlcmU=", base64String);
        }

        [Test]
        public void FromBase64String_no_mime_type()
        {
            var base64Data = Base64Data.FromBase64String("SGkgdGhlcmU=");
            Assert.AreEqual("Hi there", Encoding.UTF8.GetString(base64Data.Bytes));
        }

        [Test]
        public void FromBase64String_with_mime_type()
        {
            var base64Data = Base64Data.FromBase64String("application/json, SGkgdGhlcmU=");
            Assert.AreEqual("Hi there", Encoding.UTF8.GetString(base64Data.Bytes));
            Assert.AreEqual("application/json", base64Data.MimeType);
        }
    }
}