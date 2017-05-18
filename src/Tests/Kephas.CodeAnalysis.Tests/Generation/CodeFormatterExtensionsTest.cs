// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeFormatterExtensionsTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the code formatter extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.CodeAnalysis.Tests.Generation
{
    using Kephas.Generation;

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