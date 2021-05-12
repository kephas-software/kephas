// --------------------------------------------------------------------------------------------------------------------
// <IUsePermission.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Security.Authorization
{
    using Kephas.Security.Authorization;
    using Kephas.Security.Authorization.AttributedModel;

    /// <summary>
    /// Declares the 'use' permission. Data can be used as a 'blackbox'.
    /// This is useful in situations when the data content may not be accessed, but still used in certain situations.
    /// Note: reading data may not entitle to using it.
    /// An example could be credit cards, which may be used without being allowed to read their content.
    /// </summary>
    [PermissionInfo(DataPermissionTokenName.Use, Scoping.Type | Scoping.Instance)]
    public interface IUsePermission : IQueryPermission
    {
    }
}