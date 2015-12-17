// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebAppContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Interface for web application context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.Application
{
    using Kephas.Application;

    /// <summary>
    /// Interface for web application context.
    /// </summary>
    public interface IWebAppContext : IAppContext
    {
         /// <summary>
         /// Gets the application builder.
         /// </summary>
         /// <value>
         /// The application builder.
         /// </value>
         IApplicationBuilder AppBuilder { get; }
    }
}