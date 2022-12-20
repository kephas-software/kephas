// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashCodeGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Hash code generator.
    /// </summary>
    internal class HashCodeGenerator
    {
        private const long Seed = 4270L;
        private long generatedHash;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashCodeGenerator"/> class.
        /// </summary>
        /// <param name="seed">Optional. The seed for hash.</param>
        internal HashCodeGenerator(long? seed = null)
        {
            this.generatedHash = seed ?? Seed;
        }

        /// <summary>
        /// Gets the generated hash.
        /// </summary>
        internal int GeneratedHash => this.generatedHash.GetHashCode();

        /// <summary>
        /// Combines the hashcode of the provided integer value.
        /// </summary>
        /// <param name="i">The integer value.</param>
        /// <returns>This <see cref="HashCodeGenerator"/>.</returns>
        internal HashCodeGenerator Combine(int i) => this.CombineHashCode(i);

        /// <summary>
        /// Combines the hash code of the provided string value.
        /// </summary>
        /// <remarks>To make the hash code stable over successive runs uses a custom string hasher.</remarks>
        /// <param name="s">The string.</param>
        /// <returns>This <see cref="HashCodeGenerator"/>.</returns>
        internal HashCodeGenerator CombineStable(string? s) => this.CombineHashCode(GetHashCode(s));

        /// <summary>
        /// Combines the value of the object's hash code provided by the equality comparer.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <param name="comparer">The equality comparer.</param>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <returns>This <see cref="HashCodeGenerator"/>.</returns>
        internal HashCodeGenerator Combine<TValue>(TValue o, IEqualityComparer<TValue> comparer) =>
            o == null ? this : this.CombineHashCode(comparer.GetHashCode(o));

        /// <summary>
        /// Combines the value of the object's hash code.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <typeparam name="T">The value type.</typeparam>
        /// <returns>This <see cref="HashCodeGenerator"/>.</returns>
        internal HashCodeGenerator Combine<T>(T o) =>
            o == null ? this : this.CombineHashCode(o.GetHashCode());

        internal HashCodeGenerator CombineSequence<T>(IEnumerable<T>? sequence)
        {
            if (sequence != null)
            {
                foreach (var obj in sequence)
                {
                    this.CombineHashCode(obj?.GetHashCode() ?? 0);
                }
            }

            return this;
        }

        internal HashCodeGenerator CombineDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>>? dictionary)
        {
            if (dictionary == null)
            {
                return this;
            }

            foreach (var (key, value) in dictionary.OrderBy(kv => kv.Key))
            {
                this.CombineHashCode(key!.GetHashCode());
                this.CombineHashCode(value!.GetHashCode());
            }

            return this;
        }

        internal static int GetHashCode(string? str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }

            unchecked
            {
                var hash1 = (5381 << 16) + 5381;
                var hash2 = hash1;

                for (var i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                    {
                        break;
                    }

                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        /// <summary>Create a unique hash code for the given set of items.</summary>
        /// <returns>The generated hash.</returns>
        internal static int GetHashCode<T1, T2>(T1 o1, T2 o2)
        {
            var hashCodeCombiner = new HashCodeGenerator(Seed);
            hashCodeCombiner.CombineHashCode(o1?.GetHashCode() ?? 0);
            hashCodeCombiner.CombineHashCode(o2?.GetHashCode() ?? 0);
            return hashCodeCombiner.GeneratedHash;
        }

        /// <summary>Create a unique hash code for the given set of items.</summary>
        /// <returns>The generated hash.</returns>
        internal static int GetHashCode<T1, T2, T3>(T1 o1, T2 o2, T3 o3)
        {
            var hashCodeCombiner = new HashCodeGenerator(Seed);
            hashCodeCombiner.CombineHashCode(o1?.GetHashCode() ?? 0);
            hashCodeCombiner.CombineHashCode(o2?.GetHashCode() ?? 0);
            hashCodeCombiner.CombineHashCode(o3?.GetHashCode() ?? 0);
            return hashCodeCombiner.GeneratedHash;
        }
 
        private HashCodeGenerator CombineHashCode(int i)
        {
            unchecked
            {
                this.generatedHash = (this.generatedHash << 5) + this.generatedHash ^ (long)i;
            }

            return this;
        }
    }
}