﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientQueryConversionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the client query conversion context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion
{
    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// A client query conversion context.
    /// </summary>
    public class ClientQueryConversionContext : DataOperationContext, IClientQueryConversionContext
    {
        /// <summary>
        /// Options for controlling the operation.
        /// </summary>
        private object? options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientQueryConversionContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public ClientQueryConversionContext(IDataContext dataContext)
            : base(dataContext)
        {
        }

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public object? Options
        {
            get => this.options;
            set
            {
                this.options = value;
                if (value != null)
                {
                    var expandoValue = value.ToDynamic();
                    this.UseMemberAccessConvention = expandoValue.GetLaxValue(
                        nameof(this.UseMemberAccessConvention),
                        this.UseMemberAccessConvention);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this object uses the member access convention.
        /// </summary>
        /// <remarks>
        /// The member access convention considers all strings starting with . (dot) as member access expressions.
        /// </remarks>
        /// <value>
        /// True if the member access convention should be used, false if not.
        /// </value>
        public bool UseMemberAccessConvention { get; set; }
    }
}