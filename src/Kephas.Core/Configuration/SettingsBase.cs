// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System;
    using System.Collections.Generic;

    using Kephas.Dynamic;

    /// <summary>
    /// Base class for settings allowing dynamic access to values, case-insensitive.
    /// </summary>
    public abstract class SettingsBase : Expando
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsBase"/> class.
        /// </summary>
        protected SettingsBase()
            : base(new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase))
        {
        }
    }
}