// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFeatureManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IFeatureManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using Kephas.Services;

    /// <summary>
    /// Singleton service contract for managers of features within the application.
    /// </summary>
    /// <remarks>
    /// An application feature is a functional area of the application.
    /// It supports initialization and finalization.
    /// </remarks>
    [SingletonAppServiceContract(AllowMultiple = true)]
    public interface IFeatureManager : IAsyncInitializable<IAppContext>, IAsyncFinalizable<IAppContext>
    {
    }
}