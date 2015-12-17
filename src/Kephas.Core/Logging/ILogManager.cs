// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Manager service for loggers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Manager service for loggers.
    /// </summary>
    [ContractClass(typeof(LogManagerContractClass))]
    public interface ILogManager
    {
        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger for the provided name.</returns>
        ILogger GetLogger(string loggerName);
    }

    /// <summary>
    /// Extension methods for <see cref="ILogManager"/>.
    /// </summary>
    public static class LogManagerExtensions
    {
        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <param name="logManager">The logger factory.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public static ILogger GetLogger(this ILogManager logManager, Type type)
        {
            Contract.Requires(logManager != null);
            Contract.Requires(type != null);

            return logManager.GetLogger(type.FullName);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <typeparam name="TTarget">The type for which the logger should be created.</typeparam>
        /// <param name="logManager">The logger factory.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public static ILogger GetLogger<TTarget>(this ILogManager logManager)
        {
            Contract.Requires(logManager != null);

            return logManager.GetLogger(typeof(TTarget));
        }
    }

    /// <summary>
    /// Contract class for <see cref="ILogManager"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK for contract classes.")]
    [ContractClassFor(typeof(ILogManager))]
    internal abstract class LogManagerContractClass : ILogManager
    {
        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        /// A logger for the provided name.
        /// </returns>
        public ILogger GetLogger(string loggerName)
        {
            Contract.Requires(!string.IsNullOrEmpty(loggerName));
            Contract.Ensures(Contract.Result<ILogger>() != null);

            return null;
        }
    }
}