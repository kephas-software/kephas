// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenEntityEntry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the llbl generate entity information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen
{
    using System.Collections.Generic;

    using Kephas.Data.Capabilities;
    using Kephas.Dynamic;
    using Kephas.Model;
    using Kephas.Reflection;

    /// <summary>
    /// The entity entry specialized for the LLBLGen infrastructure.
    /// </summary>
    public class LLBLGenEntityEntry : EntityEntry
    {
        /// <summary>
        /// The model type resolver.
        /// </summary>
        private readonly IModelTypeResolver modelTypeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="LLBLGenEntityEntry" /> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="modelTypeResolver">The model type resolver.</param>
        public LLBLGenEntityEntry(object entity, IModelTypeResolver modelTypeResolver)
            : base(entity)
        {
            this.modelTypeResolver = modelTypeResolver;
        }

        /// <summary>
        /// Creates the original entity as a stamp of the current entity.
        /// </summary>
        /// <returns>The new original entity.</returns>
        protected override IExpando CreateOriginalEntity()
        {
            var runtimeTypeInfo = this.Entity.GetTypeInfo(this.DataContext?.AmbientServices?.TypeRegistry);
            var typeInfo = this.modelTypeResolver.ResolveModelType(runtimeTypeInfo, throwOnNotFound: false) ?? runtimeTypeInfo;
            var originalValues = new Dictionary<string, object>();
            foreach (var prop in typeInfo.Properties)
            {
                originalValues[prop.Name] = prop.GetValue(this.Entity);
            }

            var original = new Expando(originalValues);
            return original;
        }
    }
}