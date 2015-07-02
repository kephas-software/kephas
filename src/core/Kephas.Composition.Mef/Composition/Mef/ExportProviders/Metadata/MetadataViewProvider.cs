// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetadataViewProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Helper class for providing metadata view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.ExportProviders.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Composition.Hosting;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Kephas.Composition.Mef.Resources;

    /// <summary>
    /// Helper class for providing metadata view.
    /// </summary>
    internal static class MetadataViewProvider
    {
        /// <summary>
        /// The get metadata value method.
        /// </summary>
        private static readonly MethodInfo GetMetadataValueMethod = typeof(MetadataViewProvider).GetTypeInfo().GetDeclaredMethod("GetMetadataValue");

        /// <summary>
        /// Gets the metadata view provider.
        /// </summary>
        /// <typeparam name="TMetadata">The type of the metadata.</typeparam>
        /// <remarks>
        /// While not called through the composition pipeline, we use the dependency mechanism to surface errors
        /// with appropriate context information.
        /// </remarks>
        /// <returns>The metadata view provider.</returns>
        public static Func<IDictionary<string, object>, TMetadata> GetMetadataViewProvider<TMetadata>()
        {
            if (typeof(TMetadata) == typeof(IDictionary<string, object>))
            {
                return m => (TMetadata)m;
            }

            if (!typeof(TMetadata).GetTypeInfo().IsClass)
            {
                throw new CompositionFailedException(string.Format(Strings.MetadataViewProvider_InvalidViewImplementation, typeof(TMetadata).Name));
            }

            var ti = typeof(TMetadata).GetTypeInfo();
            var dictionaryConstructor = ti.DeclaredConstructors.SingleOrDefault(ci =>
            {
                var ps = ci.GetParameters();
                return ci.IsPublic && ps.Length == 1 && ps[0].ParameterType == typeof(IDictionary<string, object>);
            });

            if (dictionaryConstructor != null)
            {
                var providerArg = Expression.Parameter(typeof(IDictionary<string, object>), "metadata");
                return Expression.Lambda<Func<IDictionary<string, object>, TMetadata>>(
                        Expression.New(dictionaryConstructor, providerArg),
                        providerArg)
                    .Compile();
            }

            var parameterlessConstructor = ti.DeclaredConstructors.SingleOrDefault(ci => ci.IsPublic && ci.GetParameters().Length == 0);
            if (parameterlessConstructor != null)
            {
                var providerArg = Expression.Parameter(typeof(IDictionary<string, object>), "metadata");
                var resultVar = Expression.Variable(typeof(TMetadata), "result");

                var blockExprs = new List<Expression>();
                blockExprs.Add(Expression.Assign(resultVar, Expression.New(parameterlessConstructor)));

                foreach (var prop in typeof(TMetadata).GetTypeInfo().DeclaredProperties
                    .Where(prop =>
                        prop.GetMethod != null && prop.GetMethod.IsPublic && !prop.GetMethod.IsStatic &&
                        prop.SetMethod != null && prop.SetMethod.IsPublic && !prop.SetMethod.IsStatic))
                {
                    var dva = Expression.Constant(prop.GetCustomAttribute<DefaultValueAttribute>(false), typeof(DefaultValueAttribute));
                    var name = Expression.Constant(prop.Name, typeof(string));
                    var m = GetMetadataValueMethod.MakeGenericMethod(prop.PropertyType);
                    var assign = Expression.Assign(
                        Expression.Property(resultVar, prop),
                        Expression.Call(null, m, providerArg, name, dva));
                    blockExprs.Add(assign);
                }

                blockExprs.Add(resultVar);

                return Expression.Lambda<Func<IDictionary<string, object>, TMetadata>>(
                        Expression.Block(new[] { resultVar }, blockExprs), providerArg)
                    .Compile();
            }

            throw new CompositionFailedException(string.Format(Strings.MetadataViewProvider_InvalidViewImplementation, typeof(TMetadata).Name));
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="metadata">The metadata.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The metadata value.</returns>
        public static TValue GetMetadataValue<TValue>(IDictionary<string, object> metadata, string name, DefaultValueAttribute defaultValue)
        {
            object result;
            if (metadata.TryGetValue(name, out result))
            {
                return (TValue)result;
            }

            if (defaultValue != null)
            {
                return (TValue)defaultValue.Value;
            }

            // This could be significantly improved by describing the target metadata property.
            var message = string.Format(Strings.MetadataViewProvider_MissingMetadata, name);
            throw new CompositionFailedException(message);
        }
    }
}