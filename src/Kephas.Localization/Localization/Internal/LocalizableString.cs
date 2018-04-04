// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizableString.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the localizable string class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Localization.Internal
{
    using System;
    using System.Reflection;

    /// <summary>
    /// A localizable string.
    /// </summary>
    internal class LocalizableString
    {
        /// <summary>
        /// Name of the property.
        /// </summary>
        private readonly string propertyName;

        /// <summary>
        /// The cached result.
        /// </summary>
        private Func<string> cachedResult;

        /// <summary>
        /// The property value.
        /// </summary>
        private string propertyValue;

        /// <summary>
        /// The resource type.
        /// </summary>
        private Type resourceType;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizableString"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public LocalizableString(string propertyName)
        {
            this.propertyName = propertyName;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value
        {
            get => this.propertyValue;
            set
            {
                if (this.propertyValue == value)
                {
                    return;
                }

                this.ClearCache();
                this.propertyValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the resource type.
        /// </summary>
        /// <value>
        /// The resource type.
        /// </value>
        public Type ResourceType
        {
            get => this.resourceType;
            set
            {
                if (this.resourceType == value)
                {
                    return;
                }

                this.ClearCache();
                this.resourceType = value;
            }
        }

        /// <summary>
        /// Gets the localizable value.
        /// </summary>
        /// <returns>
        /// The localizable value.
        /// </returns>
        public string GetLocalizableValue()
        {
            if (this.cachedResult == null)
            {
                if (this.propertyValue == null || this.resourceType == null)
                {
                    this.cachedResult = () => this.propertyValue;
                }
                else
                {
                    var property = this.resourceType.GetRuntimeProperty(this.propertyValue);
                    var resourceStringNotAccessible = false;
                    if (!this.resourceType.GetTypeInfo().IsVisible || property == null || property.PropertyType != typeof(string))
                    {
                        resourceStringNotAccessible = true;
                    }
                    else
                    {
                        var getMethod = property.GetMethod;
                        if ((object)getMethod == null || !getMethod.IsPublic || !getMethod.IsStatic)
                        {
                            resourceStringNotAccessible = true;
                        }
                    }
                    if (resourceStringNotAccessible)
                    {
                        // TODO localize
                        var exceptionMessage = string.Format(
                            "Localization failed for {0}: cannot get {2} from {1}.",
                            this.propertyName,
                            this.resourceType.FullName,
                            this.propertyValue);
                        this.cachedResult = () => { throw new InvalidOperationException(exceptionMessage); };
                    }
                    else
                    {
                        this.cachedResult = () => (string)property.GetValue(null, null);
                    }
                }
            }
            return this.cachedResult();
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        private void ClearCache()
        {
            this.cachedResult = null;
        }
    }
}