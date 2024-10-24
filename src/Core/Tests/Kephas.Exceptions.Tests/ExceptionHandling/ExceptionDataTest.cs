// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionDataTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the exception data test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.ExceptionHandling;

using System;

using Kephas.ExceptionHandling;
using Xunit;

public class ExceptionDataTest
{
    [Fact]
    public void ExceptionData_copy_from_exception_default()
    {
        var exceptionData = new ExceptionData(new Exception("hello"));

        Assert.Equal("hello", exceptionData.Message);
        Assert.Equal(typeof(Exception).FullName, exceptionData.ExceptionType);
        Assert.Equal(SeverityLevel.Error, exceptionData.Severity);
    }

    [Fact]
    public void ExceptionData_copy_from_exception_default_with_severity()
    {
        var exceptionData = new ExceptionData(new DataException("hello") { Severity = SeverityLevel.Critical });

        Assert.Equal("hello", exceptionData.Message);
        Assert.Equal(typeof(DataException).FullName, exceptionData.ExceptionType);
        Assert.Equal(SeverityLevel.Critical, exceptionData.Severity);
    }

    [Fact]
    public void ExceptionData_copy_from_exception_overwrite_severity()
    {
        var exceptionData = new ExceptionData(new Exception("hello"), SeverityLevel.Warning);

        Assert.Equal("hello", exceptionData.Message);
        Assert.Equal(typeof(Exception).FullName, exceptionData.ExceptionType);
        Assert.Equal(SeverityLevel.Warning, exceptionData.Severity);
    }

    [Fact]
    public void ExceptionData_copy_from_exception_with_severity_overwrite_severity()
    {
        var exceptionData = new ExceptionData(new DataException("hello") { Severity = SeverityLevel.Critical }, SeverityLevel.Information);

        Assert.Equal("hello", exceptionData.Message);
        Assert.Equal(typeof(DataException).FullName, exceptionData.ExceptionType);
        Assert.Equal(SeverityLevel.Information, exceptionData.Severity);
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
        Severity = SeverityLevel.Error;
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
        Severity = SeverityLevel.Error;
    }

    /// <summary>
    /// Gets or sets the severity.
    /// </summary>
    /// <value>
    /// The severity.
    /// </value>
    public SeverityLevel Severity { get; set; }
}