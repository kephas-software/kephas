namespace CalculatorConsole.Operations
{
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services;

    /// <summary>
    /// An operation metadata.
    /// </summary>
    public class OperationMetadata : AppServiceMetadata
    {
        /// <summary>
        /// The processing priority metadata key.
        /// </summary>
        public static readonly string OperationKey = nameof(Operation);

        /// <summary>
        /// Initializes a new instance of the CalculatorConsole.Operations.OperationMetadata class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public OperationMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.Operation = (string)metadata.TryGetValue(OperationKey, string.Empty);
        }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public string Operation { get; }
    }
}