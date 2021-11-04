// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Requires.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using System.Runtime.CompilerServices;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>(T value, string parameterName)
            where T : class?
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNullOrEmpty(string? value, string parameterName)
        {
            NotNull(value, parameterName);

            if (value!.Length == 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, AbstractionStrings.Requires_NotNullOrEmpty_EmptyArgument_Exception, parameterName), parameterName);
            }

            Contract.EndContractBlock();
        }

        /// <summary>
        /// Requires that the argument is not null or empty.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the required argument is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the required argument is empty.</exception>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        [DebuggerStepThrough]
        [ContractArgumentValidator]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNullOrEmpty<T>(T[]? value, string parameterName)
        {
            NotNull(value, parameterName);

            if (value!.Length == 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, AbstractionStrings.Requires_NotNullOrEmpty_EmptyArgument_Exception, parameterName), parameterName);
            }

            Contract.EndContractBlock();
        }
    }
}