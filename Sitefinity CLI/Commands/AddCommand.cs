using McMaster.Extensions.CommandLineUtils;
using Sitefinity_CLI.Commands.AdminApp;

namespace Sitefinity_CLI.Commands
{
    [HelpOption]
    [Command(Constants.AddCommandName, Description = "Create a new resource.")]
    [Subcommand(Constants.AddResourcePackageCommandName, typeof(AddResourcePackageCommand))]
    [Subcommand(Constants.AddPageTemplateCommandName, typeof(AddPageTemplateCommand))]
    [Subcommand(Constants.AddGridWidgetCommandName, typeof(AddGridWidgetCommand))]
    [Subcommand(Constants.AddCustomWidgetCommandName, typeof(AddCustomWidgetCommand))]
    [Subcommand(Constants.AddModuleCommandName, typeof(AddModuleCommand))]
    [Subcommand(Constants.AddIntegrationTestsCommandName, typeof(AddIntegrationTestsCommand))]
    [Subcommand(Constants.AddAdminAppExtensionsBundleCommandName, typeof(AddAdminAppExtensionsBundleCommand))]
    [Subcommand(Constants.AddAdminAppExtensionCommandName, typeof(AddAdminAppExtensionCommand))]
    internal class AddCommand
    {
        protected int OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return 1;
        }
    }
}
