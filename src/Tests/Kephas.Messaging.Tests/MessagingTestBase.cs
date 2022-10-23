// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Interaction;
    using Kephas.Security.Authorization;
    using Kephas.Testing;
    using NSubstitute;

    public class MessagingTestBase : TestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(IMessageProcessor).Assembly,       /* Kephas.Messaging */
                typeof(IAppLifecycleBehavior).Assembly,   /* Kephas.Application.Abstractions */
                typeof(IAuthorizationService).Assembly,   /* Kephas.Security */
                typeof(IEventHub).Assembly,               /* Kephas.Interaction */
            };
        }

        protected virtual IServiceProvider CreateMessagingContainerMock()
        {
            var container = Substitute.For<IServiceProvider>();

            return container;
        }
    }
}
