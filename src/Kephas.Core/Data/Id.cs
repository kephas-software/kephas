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

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Helper class for IDs.
    /// </summary>
    public static class Id
    {
        /// <summary>
        /// The is unset value tester predicate.
        /// </summary>
        private static Func<object, bool> isUnsetTester;

        /// <summary>
        /// Initializes static members of the <see cref="Id"/> class.
        /// </summary>
        static Id()
        {
            isUnsetTester = value =>
            {
                if (ReferenceEquals(value, null))
                {
                    return true;
                }

                return 0.Equals(value) ||
                        0L.Equals(value) ||
                        string.Empty.Equals(value) ||
                        Guid.Empty.Equals(value);
            };
        }

        /// <summary>
        /// Gets or sets a function to determine whether a specified value is considered unset.
        /// </summary>
        public static Func<object, bool> IsUnset
        {
            get => isUnsetTester;

            set
            {
                Requires.NotNull(value, nameof(value));

                isUnsetTester = value;
            }
        }
    }
}