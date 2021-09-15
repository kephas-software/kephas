// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityModelProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the llbl generate entity model provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Data.LLBLGen.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Data.LLBLGen.Entities.Composition;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// A LLBLGen entity model provider.
    /// </summary>
    public class EntityModelProvider : IEntityModelProvider
    {
        /// <summary>
        /// The model type infos.
        /// </summary>
        private readonly List<IRuntimeTypeInfo> modelTypeInfos;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityModelProvider"/> class.
        /// </summary>
        /// <param name="entityFactoriesCollection">Collection of entity factories.</param>
        /// <param name="typeRegistry">The type registry.</param>
        public EntityModelProvider(
            ICollection<IExportFactory<IEntityFactory, EntityFactoryMetadata>> entityFactoriesCollection,
            IRuntimeTypeRegistry typeRegistry)
        {
            this.modelTypeInfos = entityFactoriesCollection
                .Select(f => typeRegistry.GetTypeInfo(f.Metadata.EntityType))
                .ToList();
        }

        /// <summary>
        /// Gets the model <see cref="ITypeInfo"/>s in this collection.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the model <see cref="ITypeInfo"/>s in this
        /// collection.
        /// </returns>
        public IEnumerable<ITypeInfo> GetModelTypeInfos()
        {
            return this.modelTypeInfos;
        }
    }
}