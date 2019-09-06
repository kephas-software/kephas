// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Internal
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services.Reflection;

    internal interface IServiceInfo : IAppServiceInfo
    {
        object GetService(IAmbientServices ambientServices);

        Type ServiceType { get; }

        IDictionary<string, object> Metadata { get; }
    }
}