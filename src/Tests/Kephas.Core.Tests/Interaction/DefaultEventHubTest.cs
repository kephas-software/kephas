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
    using System.Threading.Tasks;

    using Kephas.Interaction;
    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultEventHubTest
    {
        [Test]
        public async Task Subscribe_matching_event()
        {
            var hub = new DefaultEventHub();
            var stringEvent = string.Empty;
            IContext stringContext = null;
            Exception exEvent = null;
            IContext exContext = null;
            using (hub.Subscribe(e => e.GetType() == typeof(string), (e, ctx, token) => { stringEvent = (string)e; stringContext = ctx; return Task.FromResult(0); }))
            using (hub.Subscribe(e => e.GetType() == typeof(Exception), (e, ctx, token) => { exEvent = (Exception)e; exContext = ctx; return Task.FromResult(0); }))
            {
                var expectedContext = Substitute.For<IContext>();
                await hub.PublishAsync("hello", expectedContext);

                Assert.AreEqual("hello", stringEvent);
                Assert.AreSame(expectedContext, stringContext);
                Assert.IsNull(exEvent);
                Assert.IsNull(exContext);
            }
        }

        [Test]
        public async Task Subscribe_typed_matching_event()
        {
            var hub = new DefaultEventHub();
            var stringEvent = string.Empty;
            IContext stringContext = null;
            Exception exEvent = null;
            IContext exContext = null;
            using (hub.Subscribe<string>((e, ctx, token) => { stringEvent = e; stringContext = ctx; return Task.FromResult(0); }))
            using (hub.Subscribe<Exception>((e, ctx, token) => { exEvent = e; exContext = ctx; return Task.FromResult(0); }))
            {
                var expectedContext = Substitute.For<IContext>();
                await hub.PublishAsync("hello", expectedContext);

                Assert.AreEqual("hello", stringEvent);
                Assert.AreSame(expectedContext, stringContext);
                Assert.IsNull(exEvent);
                Assert.IsNull(exContext);
            }
        }
    }
}
