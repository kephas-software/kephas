using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Kephas.Model
{
    /// <summary>
    /// The model space is the root model element.
    /// </summary>
    [ContractClass(typeof(ModelSpaceContractClass))]
    public interface IModelSpace : INamedElement
    {
        /// <summary>
        /// Gets the dimensions.
        /// </summary>
        /// <value>
        /// The dimensions.
        /// </value>
        IModelDimension[] Dimensions { get; }

        /// <summary>
        /// Gets the projections.
        /// </summary>
        /// <value>
        /// The projections.
        /// </value>
        IEnumerable<IModelProjection> Projections { get; }

        /// <summary>
        /// Gets the members of the model space.
        /// </summary>
        /// <value>
        /// The model space members.
        /// </value>
        IEnumerable<IModelElement> Members { get; }

        /// <summary>
        /// Gets the classifiers.
        /// </summary>
        /// <value>
        /// The classifiers.
        /// </value>
        IEnumerable<IClassifier> Classifiers { get; }
    }

    /// <summary>
    /// Contract class for <see cref="IModelSpace"/>.
    /// </summary>
    [ContractClassFor(typeof(IModelSpace))]
    internal abstract class ModelSpaceContractClass : IModelSpace
    {
        /// <summary>
        /// Gets the dimensions.
        /// </summary>
        /// <value>
        /// The dimensions.
        /// </value>
        public IModelDimension[] Dimensions
        {
            get
            {
                Contract.Ensures(Contract.Result<IModelDimension[]>() != null);
                return Contract.Result<IModelDimension[]>();
            }
        }

        /// <summary>
        /// Gets the projections.
        /// </summary>
        /// <value>
        /// The projections.
        /// </value>
        public IEnumerable<IModelProjection> Projections
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IModelProjection>>() != null);
                return Contract.Result<IEnumerable<IModelProjection>>();
            }
        }

        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The model element members.
        /// </value>
        public IEnumerable<IModelElement> Members
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IModelElement>>() != null);
                return Contract.Result<IEnumerable<IModelElement>>();
            }
        }

        /// <summary>
        /// Gets the classifiers.
        /// </summary>
        /// <value>
        /// The classifiers.
        /// </value>
        public IEnumerable<IClassifier> Classifiers
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IClassifier>>() != null);
                return Contract.Result<IEnumerable<IClassifier>>();
            }
        }

        /// <summary>
        /// Gets the name of the model element.
        /// </summary>
        /// <value>
        /// The model element name.
        /// </value>
        public abstract string Name { get; }
    }
}