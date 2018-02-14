// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using Kephas.Model.Construction;

    /// <summary>
    /// An application service.
    /// </summary>
    public class AppService : ClassifierBase<IAppService>, IAppService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassifierBase{TModelContract}" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public AppService(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }
    }
}