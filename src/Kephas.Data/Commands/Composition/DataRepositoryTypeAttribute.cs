using System;
using System.Diagnostics.Contracts;
using Kephas.Composition.Metadata;

namespace Kephas.Data.Commands.Composition
{
    public class DataRepositoryTypeAttribute : IMetadataValue<Type>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepositoryTypeAttribute"/> class.
        /// </summary>
        /// <param name="dataRepositoryType">Type of the data repository.</param>
        public DataRepositoryTypeAttribute(Type dataRepositoryType)
        {
            Contract.Requires(dataRepositoryType != null);

            this.Value = dataRepositoryType;
        }

        /// <summary>
        /// Gets the associated data repository type.
        /// </summary>
        public Type Value { get; }

        /// <summary>
        /// Gets the associated data repository type.
        /// </summary>
        object IMetadataValue.Value => this.Value;
    }
}