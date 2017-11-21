// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Id.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        /// <summary>
        /// Gets the list of unset values.
        /// </summary>
        private static readonly List<object> EmptyValues = new List<object> { 0, 0L, 0d, string.Empty, Guid.Empty };

        /// <summary>
        /// The predicate for <see cref="IsEmpty"/>.
        /// </summary>
        private static Func<object, bool> isEmpty;

        /// <summary>
        /// The predicate for <see cref="IsTemporary"/>.
        /// </summary>
        private static Func<object, bool> isTemporary;

        /// <summary>
        /// Initializes static members of the <see cref="Id"/> class.
        /// </summary>
        static Id()
        {
            isEmpty = value =>
                {
                    if (ReferenceEquals(value, null))
                    {
                        return true;
                    }

                    return EmptyValues.Any(v => v.Equals(value));
                };

            isTemporary = value =>
                {
                    if (ReferenceEquals(value, null))
                    {
                        return false;
                    }

                    if (value is int)
                    {
                        return (int)value < 0;
                    }

                    if (value is long)
                    {
                        return (long)value < 0;
                    }

                    return false;
                };
        }

        /// <summary>
        /// Gets or sets a function to determine whether a specified value is considered temporary.
        /// </summary>
        /// <remarks>
        /// A temporary value indicate that a proper id will be provided at a later time,
        /// for example when creating a new entity.
        /// </remarks>
        public static Func<object, bool> IsTemporary
        {
            get => isTemporary;

            set
            {
                Requires.NotNull(value, nameof(value));

                isTemporary = value;
            }
        }

        /// <summary>
        /// Gets or sets a function to determine whether a specified value is considered empty.
        /// </summary>
        public static Func<object, bool> IsEmpty
        {
            get => isEmpty;

            set
            {
                Requires.NotNull(value, nameof(value));

                isEmpty = value;
            }
        }

        /// <summary>
        /// Adds a value considered empty.
        /// </summary>
        /// <param name="value">The empty value.</param>
        public static void AddEmptyValue(object value)
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
        public static bool RemoveEmptyValue(object value)
        {
            if (value != null)
            {
                return EmptyValues.Remove(value);
            }

            return false;
        }
    }
}