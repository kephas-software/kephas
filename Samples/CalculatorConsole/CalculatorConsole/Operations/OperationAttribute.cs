namespace CalculatorConsole.Operations
{
    using System;

    using Kephas.Composition.Metadata;

    public class OperationAttribute : Attribute, IMetadataValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the CalculatorConsole.Operations.OperationAttribute class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public OperationAttribute(string operation)
        {
            this.Value = operation;
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        object IMetadataValue.Value => this.Value;

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        public string Value { get; }
    }
}