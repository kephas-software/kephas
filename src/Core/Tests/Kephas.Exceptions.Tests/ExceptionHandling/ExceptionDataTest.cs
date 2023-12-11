// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionDataTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the exception data test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Kephas.Tests.ExceptionHandling
{
    using System;

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
            Assert.AreEqual(LogLevel.Error, exceptionData.Severity);
        }

        [Test]
        public void ExceptionData_copy_from_exception_default_with_severity()
        {
            var exceptionData = new ExceptionData(new DataException("hello") { Severity = LogLevel.Critical });

            Assert.AreEqual("hello", exceptionData.Message);
            Assert.AreEqual(typeof(DataException).FullName, exceptionData.ExceptionType);
            Assert.AreEqual(LogLevel.Critical, exceptionData.Severity);
        }

        [Test]
        public void ExceptionData_copy_from_exception_overwrite_severity()
        {
            var exceptionData = new ExceptionData(new Exception("hello"), LogLevel.Warning);

            Assert.AreEqual("hello", exceptionData.Message);
            Assert.AreEqual(typeof(Exception).FullName, exceptionData.ExceptionType);
            Assert.AreEqual(LogLevel.Warning, exceptionData.Severity);
        }

        [Test]
        public void ExceptionData_copy_from_exception_with_severity_overwrite_severity()
        {
            var exceptionData = new ExceptionData(new DataException("hello") { Severity = LogLevel.Critical }, LogLevel.Information);

            Assert.AreEqual("hello", exceptionData.Message);
            Assert.AreEqual(typeof(DataException).FullName, exceptionData.ExceptionType);
            Assert.AreEqual(LogLevel.Information, exceptionData.Severity);
        }
    }

    /// <summary>
    /// Exception for signalling generic data errors.
    /// </summary>
    internal class DataException : Exception, IErrorNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DataException(string message)
            : base(message)
        {
            this.Severity = LogLevel.Error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataException"/>
        ///  class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DataException(string message, Exception inner)
            : base(message, inner)
        {
            this.Severity = LogLevel.Error;
        }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public LogLevel Severity { get; set; }
    }
}
