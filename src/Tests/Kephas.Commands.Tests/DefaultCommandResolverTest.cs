// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultCommandResolverTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Tests
{
    using System;

    using Kephas.Services.Composition;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultCommandResolverTest
    {
        [Test]
        public void ResolveCommand_not_found_exception()
        {
            var resolver = new DefaultCommandResolver(Array.Empty<Lazy<ICommandRegistry, AppServiceMetadata>>());
            Assert.Throws<CommandNotFoundException>(() => resolver.ResolveCommand("test"));
        }

        [Test]
        public void ResolveCommand_not_found_fail_silently()
        {
            var resolver = new DefaultCommandResolver(Array.Empty<Lazy<ICommandRegistry, AppServiceMetadata>>());
            Assert.IsNull(resolver.ResolveCommand("test", throwOnNotFound: false));
        }
    }
}