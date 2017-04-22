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
    /// Primitive value type storing the ID of an entity.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="Id"/> type stores internally an <see cref="object"/>.
    /// It is the responsibility of the concrete storage mapper to provide
    /// a concrete value of a specific type (for example <see cref="long"/>).
    /// </para>
    /// <para>
    /// The <see cref="IsUnsetValue"/> static property can be set to change the default check of an unset value.
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
        /// Initializes static members of the <see cref="Id"/> class.
        /// </summary>
        static Id()
        {
            isUnsetValueTester = value =>
            {
                var idValue = value as Id;
                if (!ReferenceEquals(idValue, null))
                {
                    value = idValue.value;
                }

                if (ReferenceEquals(value, null))
                {
                    return true;
                }

                return 0.Equals(value) || 0L.Equals(value) || string.Empty.Equals(value)
                       || Guid.Empty.Equals(value);
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
        /// Gets or sets a function to determine whether a specified value is considered unset.
        /// </summary>
        public static Func<object, bool> IsUnsetValue
        {
            get => isUnsetValueTester;

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
        /// Gets the IDs underlying value.
        /// </summary>
        public object Value => this.value;

        /// <summary>
        /// Gets a value indicating whether this instance is considered unset.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is considered unset; otherwise, <c>false</c>.
        /// </value>
        public bool IsUnset => IsUnsetValue(this.value);

        /// <summary>
        /// Implicit cast that converts the given int to an ID.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static implicit operator Id(int value)
        {
            return new Id(value);
        }

        /// <summary>
        /// Implicit cast that converts the given int? to an ID.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static implicit operator Id(int? value)
        {
            return value == null ? null : new Id(value.Value);
        }

        /// <summary>
        /// Implicit cast that converts the given long to an ID.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static implicit operator Id(long value)
        {
            return new Id(value);
        }

        /// <summary>
        /// Implicit cast that converts the given long? to an ID.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static implicit operator Id(long? value)
        {
            return value == null ? null : new Id(value.Value);
        }

        /// <summary>
        /// Implicit cast that converts the given GUID to an ID.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static implicit operator Id(Guid value)
        {
            return new Id(value);
        }

        /// <summary>
        /// Implicit cast that converts the given Guid? to an ID.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static implicit operator Id(Guid? value)
        {
            return value == null ? null : new Id(value.Value);
        }

        /// <summary>
        /// Implicit cast that converts the given string to an ID.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static implicit operator Id(string value)
        {
            return value == null ? null : new Id(value);
        }

        /// <summary>
        /// Implicit cast that converts the given string to an ID.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator ==(Id value1, Id value2)
        {
            if (ReferenceEquals(value1, null))
            {
                return ReferenceEquals(value2, null) || IsUnsetValue(value2);
            }

            return value1.Equals(value2);
        }

        /// <summary>
        /// Implicit cast that converts the given string to an ID.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator !=(Id value1, Id value2)
        {
            if (ReferenceEquals(value1, null))
            {
                return !ReferenceEquals(value2, null) && !IsUnsetValue(value2);
            }

            return !value1.Equals(value2);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// True if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Id other)
        {
            if (this.IsUnset)
            {
                return other?.IsUnset ?? true;
            }

            return this.value.Equals(other?.value);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// True if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            var idObj = obj as Id;
            if (!ReferenceEquals(idObj, null))
            {
                return this.Equals(idObj);
            }

            if (IsUnsetValue(obj))
            {
                return this.IsUnset;
            }

            return !this.IsUnset && this.value.Equals(obj);
        }

        /// <summary>
        /// Delegates computing the hash code to the underlying object. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode() => this.value?.GetHashCode() ?? 0;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"id({this.value ?? "null"})";
        }
    }
}