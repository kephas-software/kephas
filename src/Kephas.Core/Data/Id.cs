// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Id.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Simple value type storing the ID of an entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Helper class for IDs.
    /// </summary>
    public static class Id
    {
        private static readonly List<object> EmptyValues = new List<object> { 0, 0L, 0d, string.Empty, Guid.Empty };
        private static readonly List<Func<object?, bool>> TemporaryChecks = new List<Func<object?, bool>>();

        /// <summary>
        /// The predicate for <see cref="IsEmpty"/>.
        /// </summary>
        private static Func<object?, bool> isEmpty;

        /// <summary>
        /// The predicate for <see cref="IsTemporary"/>.
        /// </summary>
        private static Func<object?, bool> isTemporary;

        /// <summary>
        /// Initializes static members of the <see cref="Id"/> class.
        /// </summary>
        static Id()
        {
            isEmpty = value =>
            {
                return ReferenceEquals(value, null)
                       || EmptyValues.Any(v => v.Equals(value));
            };

            isTemporary = value =>
                {
                    return value switch
                    {
                        null => false,
                        int intValue => intValue < 0,
                        long longValue => longValue < 0,
                        _ => TemporaryChecks.Any(check => check(value))
                    };
                };
        }

        /// <summary>
        /// Gets or sets a function to determine whether a specified value is considered temporary.
        /// </summary>
        /// <remarks>
        /// A temporary value indicate that a proper id will be provided at a later time,
        /// for example when creating a new entity.
        /// </remarks>
        public static Func<object?, bool> IsTemporary
        {
            get => isTemporary;

            set
            {
                value = value ?? throw new ArgumentNullException(nameof(value));

                isTemporary = value;
            }
        }

        /// <summary>
        /// Gets or sets a function to determine whether a specified value is considered empty.
        /// </summary>
        public static Func<object?, bool> IsEmpty
        {
            get => isEmpty;

            set
            {
                value = value ?? throw new ArgumentNullException(nameof(value));

                isEmpty = value;
            }
        }

        /// <summary>
        /// Adds a value considered empty.
        /// </summary>
        /// <param name="value">The empty value.</param>
        public static void AddEmptyValue(object? value)
        {
            if (value != null && !EmptyValues.Contains(value))
            {
                EmptyValues.Add(value);
            }
        }

        /// <summary>
        /// Removes a value considered empty.
        /// </summary>
        /// <param name="value">The empty value to be removed.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public static bool RemoveEmptyValue(object? value)
        {
            if (value != null)
            {
                return EmptyValues.Remove(value);
            }

            return false;
        }

        /// <summary>
        /// Adds a check function for temporary value.
        /// </summary>
        /// <param name="check">The check function to be added.</param>
        public static void AddTemporaryValueCheck(Func<object?, bool> check)
        {
            Requires.NotNull(check, nameof(check));

            if (!TemporaryChecks.Contains(check))
            {
                TemporaryChecks.Add(check);
            }
        }

        /// <summary>
        /// Removes a temporary value check function.
        /// </summary>
        /// <param name="check">The check function to be removed.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public static bool RemoveTemporaryValueCheck(Func<object?, bool> check)
        {
            Requires.NotNull(check, nameof(check));

            return TemporaryChecks.Remove(check);
        }
    }
}