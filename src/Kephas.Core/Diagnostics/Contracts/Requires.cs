// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Requires.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides contract checks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Diagnostics.Contracts
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    using Kephas.Resources;

    /// <summary>
    /// Provides contract checks.
    /// </summary>
    public static class Requires
    {
        /// <summary>
        /// Requires that the argument is not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the required argument is null.</exception>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        [DebuggerStepThrough]
        [ContractArgumentValidator]
        public static void NotNull<T>(T value, string parameterName)
            where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            Contract.EndContractBlock();
        }

        /// <summary>
        /// Requires that the argument is not null or empty.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the required argument is null or empty.</exception>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        [DebuggerStepThrough]
        [ContractArgumentValidator]
        public static void NotNullOrEmpty(string value, string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Length == 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.Requires_NotNullOrEmpty_EmptyArgument_Exception, parameterName), parameterName);
            }

            Contract.EndContractBlock();
        }
    }
}