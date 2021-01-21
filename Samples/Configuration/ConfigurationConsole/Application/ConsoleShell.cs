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
        protected override void BuildServicesContainer(IAmbientServices ambientServices)
        {
            ambientServices
                .WithNLogManager()
                .WithDynamicAppRuntime()
                .BuildWithAutofac();
        }
    }
}