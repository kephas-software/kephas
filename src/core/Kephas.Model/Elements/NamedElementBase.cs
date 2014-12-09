using System.Diagnostics.Contracts;
using Kephas.Model.Configuration;

namespace Kephas.Model.Elements
{
    /// <summary>
    /// Base class for named elements.
    /// </summary>
    public abstract class NamedElementBase : INamedElement, IConfigurableElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedElementBase"/> class.
        /// </summary>
        /// <param name="name">The element name.</param>
        protected NamedElementBase(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));

            this.Name = name;
        }

        /// <summary>
        /// Gets the name of the model element.
        /// </summary>
        /// <value>
        /// The model element name.
        /// </value>
        public string Name { get; protected set; }

        /// <summary>
        /// Completes the configuration.
        /// </summary>
        public void CompleteConfiguration()
        {
            this.OnConfigurationComplete();
        }

        /// <summary>
        /// Called when the configuration is complete.
        /// </summary>
        protected virtual void OnConfigurationComplete()
        {
        }
    }
}