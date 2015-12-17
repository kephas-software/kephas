// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UndefinedTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="Undefined" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="Undefined"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class UndefinedTest
    {
        [Test]
        public void ToString_undefined()
        {
            var actual = Undefined.Value.ToString();
            var expected = "(undefined)";

            Assert.AreEqual(expected, actual);
        }
    }
}