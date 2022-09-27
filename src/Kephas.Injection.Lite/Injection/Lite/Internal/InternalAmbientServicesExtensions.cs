// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternalAmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the internal ambient services extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Internal
{
    using System;

    using Kephas.Dynamic;

    internal static class InternalAmbientServicesExtensions
    {
        internal static TMetadata CreateTypedMetadata<TMetadata>(this IServiceInfo serviceInfo)
        {
            return (TMetadata)Activator.CreateInstance(typeof(TMetadata), serviceInfo.Metadata);
        }
    }
}