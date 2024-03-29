﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultConfigurationStoreTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default configuration store test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Configuration
{
    using Kephas.Configuration;
    using Kephas.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultConfigurationStoreTest
    {
        [Test]
        public void Item_case_insensitive()
        {
            var configStore = new DefaultConfigurationStore(new RuntimeTypeRegistry());
            configStore["myKey"] = "123";

            Assert.AreEqual("123", configStore["MyKey"]);
        }
    }
}