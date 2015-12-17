// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.Hosting
{
    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class WebAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the universal windows hosting environment to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="applicationEnvironment">The application environment.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithWebHostingEnvironment(this AmbientServicesBuilder ambientServicesBuilder, IApplicationEnvironment applicationEnvironment)
        {
            // TODO support code contracts
            // Contract.Requires(ambientServicesBuilder != null);

            return ambientServicesBuilder.WithHostingEnvironment(new WebHostingEnvironment(ambientServicesBuilder.AmbientServices.LogManager, applicationEnvironment));
        }
    }
}