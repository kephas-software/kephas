// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportEntitySuccessfulMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the import entity successful message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using Kephas.Data.IO.Resources;

    /// <summary>
    /// The import entity successful message.
    /// </summary>
    public class ImportEntitySuccessfulMessage : DataIOMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportEntitySuccessfulMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="entity">The entity.</param>
        public ImportEntitySuccessfulMessage(string message, object entity)
          : base(GetMessage(entity, message))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportEntitySuccessfulMessage"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public ImportEntitySuccessfulMessage(object entity)
          : base(GetMessage(entity))
        {
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// A formatted message.
        /// </returns>
        private static string GetMessage(object entity, string message = null)
        {
            return string.Format(Strings.ImportEntitySuccessfulMessage_Message, entity, entity.GetType(), message);
        }
    }
}