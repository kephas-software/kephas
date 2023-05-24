# Configuration

## Introduction
Provides infrastructure for application configuration.

Typically used areas and classes/interfaces/services:
* Configuration: `IConfiguration<TSettings>`.

## The `IConfiguration<TSettings>` service

While the application configuration should be fine for most cases, when working very strictly component oriented it would be more appropriate to access a configuration targeted to that component, if possible injected through composition. This is possible through the `IConfiguration<TSettings>` [[shared application service|Application-Services]].

* This service is itself a [dynamic](https://www.nuget.org/packages/Kephas.Abstractions) object, where values may be dynamically added.
* Provides the `Settings: TSettings` property, which returns the settings of the specified type.

#### Example of usage

```C#

    // Settings class (DTO). Inherit from the ISettings marker interface to make the settings discoverable over metadata. 

    public class ConsoleSettings : ISettings
    {
        public string ForeColor { get; set; }

        public string BackColor { get; set; }
    }

    // Class consuming IConfiguration<ConsoleSettings>.

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
        protected override async Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            Console.BackgroundColor = Enum.Parse<ConsoleColor>(this.consoleConfig.Settings.BackColor);
            Console.ForegroundColor = Enum.Parse<ConsoleColor>(this.consoleConfig.Settings.ForeColor);
        }
    }

```

## The `Configuration<TSettings>` service implementation

This service implementation is the default for the `IConfiguration<TSettings>` service contract. It is designed to be truly flexible in providing settings of a specific type, by aggregating configuration providers aimed at specific settings types and to which it delegates the settings retrieval. To summarize the flow:

* Gets the configuration providers ordered by their [[override|Application-Services#override-priority]] and [[processing priority|Application-Services#processing-priority]].
* Tries to find a provider handling that specific settings type. If none found, then it looks for another provider handling a compatible settings type. If still none found, then attempts to find one handling all settings types. If still none found, a `NotSupportedException` occurs.
* Delegates the settings retrieval to that provider.
* Caches the settings for faster later use.

### Configuration providers

As described previously, the configuration providers are used to provide settings of a specific type. They are completely free in choosing the proper implementation. A fallback provider is the `AppConfigurationProvider`, which is registered for all settings type, however with the lowest priority, and which gets the settings from the `IAppConfiguration` service.

A configuration provider:
* declares the handled settings type over the `[SettingsType(type)]` attribute.
* provides the settings through the `GetSettings(settingsType: Type): object` method.

Another typical configuration provider would be a file based one, for example JSON or XML.

## Other resources

* [Kephas.Core](https://www.nuget.org/packages/Kephas.Core)
* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
