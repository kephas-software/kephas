﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContextTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application context test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Tests
{
    using Kephas.Application;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class AppContextTest
    {
        [Test]
        public void Constructor_default_AppRuntime_is_from_di_container()
        {
            var ambientServices = Substitute.For<IAmbientServices>();
            var appRuntime = Substitute.For<IAppRuntime>();

            ambientServices.AppRuntime.Returns(appRuntime);
            var appContext = new AppContext(ambientServices);
            Assert.AreSame(appRuntime, appContext.AppRuntime);
        }
    }
}