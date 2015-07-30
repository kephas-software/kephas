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

    /// <summary>
    /// Simple value type storing the ID of an entity.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="Id"/> type stores internally an <see cref="object"/>. 
    /// It is the responsibility of the concrete storage mapper to provide
    /// a concrete value of a specific type (for example <see cref="long"/>).
    /// </para>
    /// <para>
    /// The IsUnsetValue static property can be changed to fit the 
    /// </para>
    /// </remarks>
    public class Id : IEquatable<Id>
    {
        /// <summary>
        /// The is unset value tester predicate.
        /// </summary>
        private static Func<object, bool> isUnsetValueTester;

        /// <summary>
        /// The underlying value.
        /// </summary>
        private readonly object value;

        /// <summary>
        /// Initializes static members of the the <see cref="Id"/> class.
        /// </summary>
        static Id()
        {
            isUnsetValueTester = value =>
            {
                if (value == null)
                {
                    return true;
                }

                return (0.Equals(value) || 0L.Equals(value) || string.Empty.Equals(value)
                        || Guid.Empty.Equals(value));
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Id"/> class.
        /// </summary>
        public Id()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Id"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Id(object value)
        {
            var idValue = value as Id;
            if (idValue != null)
            {
                this.value = idValue.value;
            }
            else
            {
                this.value = IsUnsetValue(value) ? null : value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is considered unset.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is considered unset; otherwise, <c>false</c>.
        /// </value>
        public bool IsUnset => this.value == null;

        /// <summary>
        /// Gets or sets a function to determine whether a specified value is considered unset.
        /// </summary>
        /// <returns></returns>
        public static Func<object, bool> IsUnsetValue
        {
            get
            {
                return isUnsetValueTester;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                isUnsetValueTester = value;
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Id other)
        {
            if (this.IsUnset)
            {
                return other.IsUnset;
            }

            return this.value.Equals(other.value);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            var idObj = obj as Id;
            if (idObj != null)
            {
                return this.Equals(idObj);
            }

            if (IsUnsetValue(obj))
            {
                return this.IsUnset;
            }

            if (this.IsUnset)
            {
                return false;
            }

            return this.value.Equals(obj);
        }

        /// <summary>
        /// Delegates computing the hash code to the underlying object. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode() => this.value?.GetHashCode() ?? 0;
    }
}