// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionDataTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the exception data test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.ExceptionHandling
{
    using System;
    using Kephas.Data;
    using Kephas.ExceptionHandling;
    using NUnit.Framework;

    [TestFixture]
    public class ExceptionDataTest
    {
        [Test]
        public void ExceptionData_copy_from_exception_default()
        {
            var exceptionData = new ExceptionData(new Exception("hello"));

            Assert.AreEqual("hello", exceptionData.Message);
            Assert.AreEqual(typeof(Exception).FullName, exceptionData.ExceptionType);
            Assert.AreEqual(SeverityLevel.Error, exceptionData.Severity);
        }

        [Test]
        public void ExceptionData_copy_from_exception_default_with_severity()
        {
            var exceptionData = new ExceptionData(new DataException("hello") { Severity = SeverityLevel.Fatal });

            Assert.AreEqual("hello", exceptionData.Message);
            Assert.AreEqual(typeof(DataException).FullName, exceptionData.ExceptionType);
            Assert.AreEqual(SeverityLevel.Fatal, exceptionData.Severity);
        }

        [Test]
        public void ExceptionData_copy_from_exception_overwrite_severity()
        {
            var exceptionData = new ExceptionData(new Exception("hello"), SeverityLevel.Warning);

            Assert.AreEqual("hello", exceptionData.Message);
            Assert.AreEqual(typeof(Exception).FullName, exceptionData.ExceptionType);
            Assert.AreEqual(SeverityLevel.Warning, exceptionData.Severity);
        }

        [Test]
        public void ExceptionData_copy_from_exception_with_severity_overwrite_severity()
        {
            var exceptionData = new ExceptionData(new DataException("hello") { Severity = SeverityLevel.Fatal }, SeverityLevel.Info);

            Assert.AreEqual("hello", exceptionData.Message);
            Assert.AreEqual(typeof(DataException).FullName, exceptionData.ExceptionType);
            Assert.AreEqual(SeverityLevel.Info, exceptionData.Severity);
        }
    }
}