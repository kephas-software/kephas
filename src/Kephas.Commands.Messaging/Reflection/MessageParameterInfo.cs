// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageParameterInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Messaging.Reflection
{
    using System.ComponentModel;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Reflection.Dynamic;
    using Kephas.Runtime.AttributedModel;

    /// <summary>
    /// Provides metadata for a parameter of an operation based on a message.
    /// </summary>
    public class MessageParameterInfo : DynamicParameterInfo
    {
        private readonly IPropertyInfo propertyInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageParameterInfo"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="position">The parameter position.</param>
        internal MessageParameterInfo(MessageOperationInfo container, IPropertyInfo propertyInfo, int position)
        {
            Requires.NotNull(propertyInfo, nameof(propertyInfo));

            this.propertyInfo = propertyInfo;
            this.Name = propertyInfo.Name;
            this.Position = position;
            propertyInfo.Annotations.ForEach(this.AddAnnotation);
            this.IsIn = this.Annotations.OfType<InAttribute>().Any();
            this.IsOut = this.Annotations.OfType<OutAttribute>().Any();
            this.ValueType = propertyInfo.ValueType;
            this.IsOptional = this.Annotations.OfType<DefaultValueAttribute>().Any();
        }

        /// <summary>
        /// Gets the display information.
        /// </summary>
        /// <returns>The display information.</returns>
        public override IDisplayInfo? GetDisplayInfo()
        {
            return this.propertyInfo.GetDisplayInfo();
        }
    }
}