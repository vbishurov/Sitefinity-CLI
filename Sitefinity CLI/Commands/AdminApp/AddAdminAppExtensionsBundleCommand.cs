using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitefinity_CLI.Enums;
using McMaster.Extensions.CommandLineUtils;

namespace Sitefinity_CLI.Commands.AdminApp
{
    using System.ComponentModel;
    using System.Diagnostics;

    [Command(Constants.AddAdminAppExtensionsBundleCommandName, Description = "Adds a new AdminApp extensions bundle", FullName = Constants.AddAdminAppExtensionsBundleCommandFullName)]
    internal class AddAdminAppExtensionsBundleCommand : AddToProjectCommandBase
    {
        protected override string FolderPath => this.Name;

        protected override string CreatedMessage => Constants.AdminAppExtensionsBundleCreatedMessage;

        protected override string TemplatesFolder => Constants.AdminAppExtensionsBundleTemplatesFolderName;

        [Option("--skipInstall", "Skips running npm install after adding the bundle. Default value: [False]", CommandOptionType.SingleValue)]
        [DefaultValue(false)]
        public bool SkipInstall { get; set; } = false;

        public override int OnExecute(CommandLineApplication config)
        {
            if (Directory.Exists(this.TargetFolder))
            {
                if (File.Exists(Path.Combine(this.TargetFolder, "__extensions_index.ts")))
                {
                    Utils.WriteLine(string.Format(Constants.ResourceExistsMessage, config.FullName, this.Name, this.TargetFolder), ConsoleColor.Red);
                    return (int)ExitCode.GeneralError;
                }
            }
            else
            {
                Directory.CreateDirectory(this.TargetFolder);
            }

            if (base.OnExecute(config) == 1)
            {
                return (int)ExitCode.GeneralError;
            }

            var templatePath = Path.Combine(this.CurrentPath, Constants.TemplatesFolderName, this.Version, TemplatesFolder, this.TemplateName);
            if (!Directory.Exists(templatePath))
            {
                Utils.WriteLine(string.Format(Constants.TemplateNotFoundMessage, config.FullName, templatePath), ConsoleColor.Red);
                return (int)ExitCode.GeneralError;
            }

            foreach (string dirPath in Directory.GetDirectories(templatePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(templatePath, this.TargetFolder));

            var files = Directory.GetFiles(templatePath, "*.*", SearchOption.AllDirectories).Where(f => !f.EndsWith(".Template") && !f.EndsWith("templates.json")).ToList();
            foreach (string newPath in files)
            {
                File.Copy(newPath, newPath.Replace(templatePath, this.TargetFolder), true);
                Utils.WriteLine(string.Format(Constants.FileCreatedMessage, Path.GetFileName(newPath), newPath), ConsoleColor.Green);
            }

            if (!this.SkipInstall)
            {
                var psiNpmRunDist = new ProcessStartInfo
                {
                    FileName = "cmd",
                    RedirectStandardInput = true,
                    WorkingDirectory = this.TargetFolder
                };
                var pNpmRunDist = Process.Start(psiNpmRunDist);
                pNpmRunDist.StandardInput.WriteLine("npm install & exit");
                pNpmRunDist.WaitForExit();
            }
            else
            {
                Utils.WriteLine("Skipped running npm install. Make sure to run it manually.", ConsoleColor.Yellow);
            }

            return (int)ExitCode.OK;
        }

        protected override void AddAdditionalTemplateData(Dictionary<string, string> data)
        {
            base.AddAdditionalTemplateData(data);

            data["lowerCaseName"] = this.Name.ToLower();
        }
    }
}
