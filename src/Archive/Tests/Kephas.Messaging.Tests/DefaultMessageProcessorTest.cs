// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageProcessorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="DefaultMessageProcessor" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultMessageProcessorTest : DefaultMessageProcessorTestBase
    {
        protected override IServiceProvider BuildServiceProvider()
        {
            return this.CreateServicesBuilder().BuildWithDependencyInjection();
        }
    }
}