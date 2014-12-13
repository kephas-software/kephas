// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelHelper.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Helper class for model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Helper class for model.
    /// </summary>
    internal static class ModelHelper
    {
        /// <summary>
        /// The name discriminators.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, string> NameDiscriminators = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// Gets the member name discriminator for the provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The member name discriminator.</returns>
        public static string GetMemberNameDiscriminator(this Type type)
        {
            Contract.Requires(type != null);

            return NameDiscriminators.GetOrAdd(
                type,
                t =>
                {
                    var attr = t.GetTypeInfo().GetCustomAttribute<MemberNameDiscriminatorAttribute>();
                    if (attr == null)
                    {
                        return string.Empty;
                    }

                    return attr.NameDiscriminator;
                });
        }
    }
}