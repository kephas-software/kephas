// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelAmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection.Conventions;
using Kephas.Injection.Lite;

namespace Kephas.Model
{
    /// <summary>
    /// Extension methods for <see cref="IAmbientServices"/>. 
    /// </summary>
    public static class ModelAmbientServicesExtensions
    {
        /// <summary>
        /// Adds service registrations for model.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAmbientServices WithModel(
            this IAmbientServices ambientServices)
        {
            return ambientServices
                .RegisterMultiple<IAppServiceInfoProvider>(b => b.WithType<ModelAppServiceInfoProvider>());
        }
    }
}