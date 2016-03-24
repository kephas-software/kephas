namespace StartupConsole.Application
{
    using Kephas.Application;

    /// <summary>
    /// A console application.
    /// </summary>
    public class ConsoleApplication : ApplicationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBase"/> class.
        /// </summary>
        /// <param name="appBootstrapper">The application bootstrapper.</param>
        public ConsoleApplication(IAppBootstrapper appBootstrapper)
            : base("demo-console", appBootstrapper)
        {
        }
    }
}
