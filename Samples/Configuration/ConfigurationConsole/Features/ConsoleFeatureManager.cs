namespace ConfigurationConsole.Features
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using ConfigurationConsole.Configuration;

    using Kephas.Application;
    using Kephas.Configuration;

    /// <summary>
    /// Manager for console features.
    /// </summary>
    public class ConsoleFeatureManager : FeatureManagerBase
    {
        private readonly IConfiguration<ConsoleSettings> consoleConfig;

        public ConsoleFeatureManager(IConfiguration<ConsoleSettings> consoleConfig)
        {
            this.consoleConfig = consoleConfig;
        }

        /// <summary>Initializes the feature asynchronously.</summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        protected override Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            Console.BackgroundColor = this.consoleConfig.GetSettings().BackColor;
            Console.ForegroundColor = this.consoleConfig.GetSettings().ForeColor;

            return Task.CompletedTask;
        }
    }
}