﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeFormatterExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the code formatter extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.CodeAnalysis.Tests.Generation
{
    using Kephas.CodeAnalysis.Generation;

    using NUnit.Framework;

    [TestFixture]
    public class CodeFormatterExtensionsTest
    {
        [Test]
        public void CurrentIndent()
        {
            var formatter = new CodeFormatter();
            Assert.AreEqual(string.Empty, formatter.CurrentIndent());

            formatter.IncreaseIndent();
            Assert.AreEqual("  ", formatter.CurrentIndent());
        }
    }
}