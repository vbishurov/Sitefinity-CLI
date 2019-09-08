using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using Sitefinity_CLI.Enums;
using McMaster.Extensions.CommandLineUtils;
using Sitefinity_CLI.Model;
using Newtonsoft.Json;

namespace Sitefinity_CLI.Commands.AdminApp
{
    [Command(Constants.AddAdminAppExtensionCommandName, Description = "Adds a new AdminApp extension to a bundle", FullName = Constants.AddAdminAppExtensionCommandFullName)]
    internal class AddAdminAppExtensionCommand : AddToProjectCommandBase
    {
        private string BundlePath => Path.Combine(this.ProjectRootPath, this.BundleName);

        private string BundleIndexPath => Path.Combine(this.BundlePath, "__extensions_index.ts");
        
        private string KebabCaseName => this.kebabCaseName ?? (this.kebabCaseName = this.GetKebabCase(this.Name));

        private string ColumnName { get; set; }

        private string ColumnTitle { get; set; }

        protected override string FolderPath => Path.Combine(this.BundleName, this.KebabCaseName);

        protected override string CreatedMessage => Constants.AdminAppExtensionCreatedMessage;

        protected override string TemplatesFolder => Constants.AdminAppExtensionTemplatesFolderName;
        
        [Option(Constants.BundleNameOptionTemplate, Constants.BundleNameOptionDescription + Constants.DefaultAdminAppExtensionsBundleName, CommandOptionType.SingleValue)]
        [DefaultValue(Constants.DefaultAdminAppExtensionsBundleName)]
        public string BundleName { get; set; }

        [Option(Constants.TemplateNameOptionTemplate, Constants.TemplateNameOptionDescription + Constants.DefaultAdminAppExtensionName, CommandOptionType.SingleValue)]
        [DefaultValue(Constants.DefaultAdminAppExtensionName)]
        public override string TemplateName { get; set; } = Constants.DefaultAdminAppExtensionName;

        public override int OnExecute(CommandLineApplication config)
        {
            if (!File.Exists(this.BundleIndexPath))
            {
                Utils.WriteLine(string.Format(Constants.AdminAppExtensionsBundleNotFoundMessage, this.Name, this.BundlePath, this.BundleName), ConsoleColor.Red);
                return (int)ExitCode.GeneralError;
            }

            if (!Directory.Exists(this.TargetFolder))
            {
                Directory.CreateDirectory(this.TargetFolder);
            }
            else if (File.Exists(Path.Combine(this.TargetFolder, "index.ts")))
            {
                Utils.WriteLine(string.Format(Constants.ResourceExistsMessage, "AdminApp Extension", this.Name, this.TargetFolder), ConsoleColor.Red);
                return (int)ExitCode.GeneralError;
            }
            
            if (base.OnExecute(config) == 1)
            {
                return (int)ExitCode.GeneralError;
            }

            TsModuleModifier.RegisterExtension(this.BundleIndexPath, this.Name + "Module", this.KebabCaseName);

            return (int) ExitCode.OK;
        }

        protected override IEnumerable<FileModel> GetFileModels()
        {
            var templatePath = Path.Combine(this.CurrentPath, Constants.TemplatesFolderName, this.Version, this.TemplatesFolder, this.TemplateName);

            var templatesModelsJson = File.ReadAllText(Path.Combine(templatePath, "templates.json"));

            var models = JsonConvert.DeserializeObject<IEnumerable<FileModel>>(templatesModelsJson);

            foreach (var model in models)
            {
                model.FilePath = Path.Combine(this.TargetFolder, string.Format(model.FilePath, this.KebabCaseName));
                model.TemplatePath = Path.Combine(templatePath, model.TemplatePath);
            }

            return models;
        }

        protected override void AddAdditionalTemplateData(Dictionary<string, string> data)
        {
            base.AddAdditionalTemplateData(data);

            if (this.ColumnName == null)
            {
                this.ColumnName = Prompt.GetString("Please enter the column name", promptColor: ConsoleColor.Yellow);
            }

            if (this.ColumnTitle == null)
            {
                this.ColumnTitle = Prompt.GetString("Please enter the column title", promptColor: ConsoleColor.Yellow);
            }

            data["componentKebabCaseName"] = this.GetKebabCase(this.Name);
            data["componentPascalCaseName"] = this.GetPascalCase(this.Name);
            data["columnName"] = this.ColumnName;
            data["columnTitle"] = this.ColumnTitle;
        }

        /// <summary>
        /// Converts string to kebab-case
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns>TheString</returns>
        private string GetKebabCase(string s)
        {
            return Regex.Replace(s, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "-$1", RegexOptions.Compiled).Trim().ToLower();
        }

        private string kebabCaseName;
    }
}
