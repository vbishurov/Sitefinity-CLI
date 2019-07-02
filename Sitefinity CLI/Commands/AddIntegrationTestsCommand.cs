﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Sitefinity_CLI.Model;

namespace Sitefinity_CLI.Commands
{
    [Command(Constants.AddIntegrationTestsCommandName, Description = "Adds a new custom widget to the current project.", FullName = Constants.AddIntegrationTestsCommandFullName)]
    internal class AddIntegrationTestsCommand : AddToSitefinityCommandBase
    {

        protected override string FolderPath => string.Empty;

        protected override string CreatedMessage => Constants.IntegrationTestsCreatedMessage;

        protected override string TemplatesFolder => throw new System.NotImplementedException();

        protected Guid ProjectGuid { get; set; } = Guid.NewGuid();

        protected string SolutionPath { get; set; }

        protected string BinFolder { get; set; }

        protected override IEnumerable<FileModel> GetFileModels()
        {
            var templatePath = Path.Combine(this.CurrentPath, Constants.TemplatesFolderName, this.Version, Constants.IntegrationTestsTemplateFolderName, this.TemplateName);

            var models = new List<FileModel>()
            {
                new FileModel()
                {
                    FilePath = Path.Combine(this.ProjectRootPath, "Properties", string.Concat(Constants.AssemblyInfoFileName, Constants.CSharpFileExtension)),
                    TemplatePath = Path.Combine(templatePath, string.Concat(Constants.AssemblyInfoFileName, ".Template"))
                },
                new FileModel()
                {
                    FilePath = Path.Combine(this.ProjectRootPath, string.Concat(Constants.IntegrationTestsFolderName, Constants.CsprojFileExtension)),
                    TemplatePath = Path.Combine(templatePath, string.Concat( Constants.CsProjTemplateName, ".Template"))

                },
                new FileModel()
                {
                    FilePath = Path.Combine(this.ProjectRootPath, string.Concat(Constants.IntegrationTestClassName, Constants.CSharpFileExtension)),
                    TemplatePath = Path.Combine(templatePath, string.Concat(Constants.IntegrationTestClassName, ".Template"))

                },
                new FileModel()
                {
                    FilePath = Path.Combine(this.ProjectRootPath, string.Concat(Constants.PackagesFileName, Constants.ConfigFileExtension)),
                    TemplatePath = Path.Combine(templatePath, string.Concat(Constants.PackagesFileName, Constants.ConfigFileExtension, ".Template"))
                },
            };

            return models;
        }

        protected override int CreateFileFromTemplate(string filePath, string templatePath, string resourceFullName, object data)
        {
            if (data is IDictionary<string, string> dictionary)
            {
                dictionary["binFolder"] = this.BinFolder;
                dictionary["projectGuid"] = this.ProjectGuid.ToString();
            }
            else
            {
                return 1;
            }

            return base.CreateFileFromTemplate(filePath, templatePath, resourceFullName, data);
        }

        public override int OnExecute(CommandLineApplication config)
        {
            var currentPath = Path.GetDirectoryName(this.ProjectRootPath);

            while (Directory.EnumerateFiles(currentPath, @"*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault() == null)
            {
                currentPath = Directory.GetParent(currentPath)?.ToString();
                if (string.IsNullOrEmpty(currentPath))
                {
                    return 1;
                }
            }

            // TODO: refactor
            var webApp = Directory.EnumerateFiles(currentPath, @"*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();
            this.SolutionPath = webApp;

            this.ProjectRootPath = Path.Combine(currentPath, Constants.IntegrationTestsFolderName);

            Directory.CreateDirectory(this.ProjectRootPath);

            var sitefinityPath = Directory.EnumerateFiles(currentPath, "Telerik.Sitefinity.dll", SearchOption.AllDirectories).FirstOrDefault();
            var binFolder = Path.GetDirectoryName(sitefinityPath);

            if (string.IsNullOrEmpty(binFolder))
            {
                return 1;
            }

            if (Path.IsPathRooted(binFolder))
            {
                binFolder = Path.GetRelativePath(this.ProjectRootPath, binFolder);
            }

            this.BinFolder = binFolder;

            if (base.OnExecute(config) == 1)
            {
                return 1;
            }

            SlnModifier.AddFile(this.SolutionPath, this.createdFiles.FirstOrDefault(x => x.EndsWith(Constants.CsprojFileExtension)), this.ProjectGuid);

            return 0;
        }
    }
}
