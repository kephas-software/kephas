using System.Diagnostics.Contracts;

namespace Kephas.Model
{
    /// <summary>
    /// Contract for named elements.
    /// </summary>
    [ContractClass(typeof (NamedElementContractClass))]
    public interface INamedElement
    {
        /// <summary>
        /// Gets the name of the model element.
        /// </summary>
        /// <value>
        /// The model element name.
        /// </value>
        string Name { get; }
    }

    /// <summary>
    /// Contract class for <see cref="INamedElement"/>.
    /// </summary>
    [ContractClassFor(typeof(INamedElement))]
    internal abstract class NamedElementContractClass : INamedElement
    {
        /// <summary>
        /// Gets the name of the model element.
        /// </summary>
        /// <value>
        /// The model element name.
        /// </value>
        public string Name
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
                return Contract.Result<string>();
            }
        }
    }
}