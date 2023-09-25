# Configuration Abstractions

## Introduction
Provides abstractions for application configuration.

Typically used areas and classes/interfaces/services:
* Configuration: `ISettings`.

## The `ISettings` interface

Marker interface for settings discoverable over metadata.

#### Example of usage

```C#

    // Console Settings class (DTO). Inherit from the ISettings marker interface to make the settings discoverable over metadata. 

    public class ConsoleSettings : ISettings
    {
        public string ForeColor { get; set; }

        public string BackColor { get; set; }
    }

    // Class consuming IConfiguration<ConsoleSettings>.

    public class ConsoleFeatureManager : FeatureManagerBase
    {
        private readonly IOptions<ConsoleSettings> consoleConfig;

        public ConsoleFeatureManager(IOptions<ConsoleSettings> consoleConfig)
        {
            this.consoleConfig = consoleConfig;
        }

        /// <summary>Initializes the feature asynchronously.</summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        protected override async Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            Console.BackgroundColor = Enum.Parse<ConsoleColor>(this.consoleConfig.Settings.BackColor);
            Console.ForegroundColor = Enum.Parse<ConsoleColor>(this.consoleConfig.Settings.ForeColor);
        }
    }

```
* 
* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
