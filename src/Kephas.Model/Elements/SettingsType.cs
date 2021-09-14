// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using Kephas.Composition;
    using Kephas.Configuration.Reflection;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Construction;

    /// <summary>
    /// Classifier class for application services.
    /// </summary>
    public class SettingsType : ClassifierBase<ISettingsType>, ISettingsType
    {
        /// <summary>
        /// The application service attribute.
        /// </summary>
        private readonly ISettingsInfo settingsInfo;

        /// <summary>
        /// Context for the composition.
        /// </summary>
        private readonly IInjector injector;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsType" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="settingsInfo">The settings information.</param>
        /// <param name="name">The name.</param>
        public SettingsType(
            IModelConstructionContext constructionContext,
            ISettingsInfo settingsInfo,
            string name)
            : base(constructionContext, name)
        {
            Requires.NotNull(settingsInfo, nameof(settingsInfo));

            this.settingsInfo = settingsInfo;
            this.injector = constructionContext.Injector;
        }
    }
}
