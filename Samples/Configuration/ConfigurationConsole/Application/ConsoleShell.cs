namespace StartupConsole.Application
{
    using Kephas;
    using Kephas.Application;
    using Kephas.Logging.NLog;

    /// <summary>
    /// A console shell.
    /// </summary>
    public class ConsoleShell : AppBase
    {
        protected override void ConfigureAmbientServices(IAmbientServices ambientServices)
        {
            ambientServices
                .WithNLogManager()
                .WithDynamicAppRuntime()
                .WithAutofacCompositionContainer();
        }
    }
}