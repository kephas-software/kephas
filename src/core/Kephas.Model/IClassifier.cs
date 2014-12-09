using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Kephas.Model
{
    /// <summary>
    /// Contract for classifiers.
    /// </summary>
    [ContractClass(typeof (ClassifierContractClass))]
    public interface IClassifier : IModelElement
    {
        /// <summary>
        /// Gets the classifier properties.
        /// </summary>
        /// <value>
        /// The classifier properties.
        /// </value>
        IEnumerable<IProperty> Properties { get; }
    }

    /// <summary>
    /// Contract class for <see cref="IClassifier"/>.
    /// </summary>
    [ContractClassFor(typeof(IClassifier))]
    internal abstract class ClassifierContractClass : IClassifier
    {
        /// <summary>
        /// Gets the classifier properties.
        /// </summary>
        /// <value>
        /// The classifier properties.
        /// </value>
        public IEnumerable<IProperty> Properties
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IProperty>>() != null);
                return Contract.Result<IEnumerable<IProperty>>();
            }
        }

        /// <summary>
        /// Gets the name of the model element.
        /// </summary>
        /// <value>
        /// The model element name.
        /// </value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the projection where the model element is defined.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
        public abstract IModelProjection Projection { get; }

        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The model element members.
        /// </value>
        public abstract IEnumerable<IModelElement> Members { get; }

        /// <summary>
        /// Gets the attributes of this model element.
        /// </summary>
        /// <value>
        /// The model element attributes.
        /// </value>
        public abstract IEnumerable<IAttribute> Attributes { get; }
    }
}