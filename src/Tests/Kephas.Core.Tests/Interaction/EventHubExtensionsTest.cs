// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultEventHubTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default event hub test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Interaction
{
    using System;

    using Kephas.Interaction;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class EventHubExtensionsTest
    {
        [Test]
        public void Subscribe_wrong_args()
        {
            var calls = 0;
            Assert.Throws<ArgumentNullException>(() => EventHubExtensions.Subscribe<string>(null, async (e, c, t) => calls++));

            Assert.Throws<ArgumentNullException>(() => EventHubExtensions.Subscribe<string>(Substitute.For<IEventHub>(), null));
        }
    }
}
