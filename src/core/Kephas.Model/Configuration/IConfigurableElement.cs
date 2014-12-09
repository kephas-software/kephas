namespace Kephas.Model.Configuration
{
    /// <summary>
    /// Contract for elements supporting configuration.
    /// </summary>
    public interface IConfigurableElement
    {
        /// <summary>
        /// Completes the configuration.
        /// </summary>
        void CompleteConfiguration();
    }
}