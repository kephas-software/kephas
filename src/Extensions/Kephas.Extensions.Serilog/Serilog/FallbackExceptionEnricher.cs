using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Filters;

namespace Kephas.Extensions.Serilog;

/// <summary>
/// Fallback exception enricher which replaces the reflection based exception destructurer.
/// Required for the case when the reflection based destructurer fails to destructure the exception,
/// like in the case.
/// </summary>
internal class FallbackExceptionEnricher(IDestructuringOptions destructuringOptions) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(propertyFactory);

        if (logEvent.Exception is null)
        {
            return;
        }

        var dataDictionary = DestructureException(logEvent.Exception);

        if (dataDictionary is not null)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                destructuringOptions.RootName,
                dataDictionary,
                true));
        }
    }

    private IReadOnlyDictionary<string, object?>? DestructureException(Exception exception)
    {
        var data = new ExceptionPropertiesBag(exception, destructuringOptions.Filter);
        Destructure(exception, data, DestructureException);

        return data.GetResultDictionary();
    }

    private void Destructure(
        Exception exception,
        IExceptionPropertiesBag propertiesBag,
        Func<Exception, IReadOnlyDictionary<string, object?>?> destructureException)
    {
        propertiesBag.AddProperty("Type", exception.GetType().FullName);

        DestructureCommonExceptionProperties(
            exception,
            propertiesBag,
            destructureException);
    }

    private static void DestructureCommonExceptionProperties(
        Exception exception,
        IExceptionPropertiesBag propertiesBag,
        Func<Exception, IReadOnlyDictionary<string, object?>?> innerDestructure)
    {
        if (!string.IsNullOrEmpty(exception.HelpLink))
        {
            propertiesBag.AddProperty(nameof(Exception.HelpLink), exception.HelpLink);
        }

        if (exception.HResult != 0)
        {
            propertiesBag.AddProperty(nameof(Exception.HResult), exception.HResult);
        }

        propertiesBag.AddProperty(nameof(Exception.Message), exception.Message);
        propertiesBag.AddProperty(nameof(Exception.Source), exception.Source);
        propertiesBag.AddProperty(nameof(Exception.StackTrace), exception.StackTrace);

        if (exception.TargetSite is not null)
        {
            propertiesBag.AddProperty(nameof(Exception.TargetSite), exception.TargetSite.ToString());
        }

        if (exception.InnerException is not null)
        {
            propertiesBag.AddProperty(nameof(Exception.InnerException), innerDestructure(exception.InnerException));
        }
    }

    private class ExceptionPropertiesBag : IExceptionPropertiesBag
    {
        /// <summary>
        /// In theory there should not be any properties with same names passed
        /// but just as a defensive and robustness measure, we allow small amount
        /// so that library can keep working in case of minor error (or unexpected
        /// scenario) on the properties provider side.
        /// </summary>
        private const int AcceptableNumberOfSameNameProperties = 5;
        private readonly Exception exception;
        private readonly IExceptionPropertyFilter? filter;
        private readonly Dictionary<string, object?> properties = new();

        /// <summary>
        /// We keep a note on whether the results were collected to be sure that after that there are no changes. This
        /// is the application of fail-fast principle.
        /// </summary>
        private bool resultsCollected;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionPropertiesBag"/> class.
        /// </summary>
        /// <param name="exception">The exception which properties will be added to the bag.</param>
        /// <param name="filter">Filter that should be applied to each property just before adding it to the bag.</param>
        public ExceptionPropertiesBag(Exception exception, IExceptionPropertyFilter? filter = null)
        {
            this.exception = exception ?? throw new ArgumentNullException(nameof(exception));
            this.filter = filter;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, object?> GetResultDictionary()
        {
            resultsCollected = true;
            return properties;
        }

        /// <inheritdoc />
        public void AddProperty(string key, object? value)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (resultsCollected)
            {
                throw new InvalidOperationException($"Cannot add exception property '{key}' to bag, after results were already collected");
            }

            if (filter is not null)
            {
                if (filter.ShouldPropertyBeFiltered(exception, key, value))
                {
                    return;
                }
            }

            AddPairToProperties(key, value);
        }

        /// <inheritdoc />
        public bool ContainsProperty(string key) => properties.ContainsKey(key);

        private static string GetReplacementKey(string key) => key + "$";

        /// <summary>
        /// We want to be as robust as possible
        /// so even in case of multiple properties with the same name
        /// we want to at least try carrying on and keep working.
        /// </summary>
        private void AddPairToProperties(string key, object? value)
        {
            var i = 0;
            while (!properties.TryAdd(key, value) && i < AcceptableNumberOfSameNameProperties)
            {
                key = GetReplacementKey(key);
                i++;
            }
        }
    }
}