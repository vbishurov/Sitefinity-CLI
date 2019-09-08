using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sitefinity_CLI.Tests.TsModuleModifierTests
{
    using System.IO;
    using System.Xml.Linq;

    [TestClass]
    public class RegisterExtensionTests
    {
        private string IndexWithModulesPath = $"{Directory.GetCurrentDirectory()}/TsModuleModifierTests/Input/WithModules.ts";
        private string IndexWithoutModulesPath = $"{Directory.GetCurrentDirectory()}/TsModuleModifierTests/Input/WithoutModules.ts";
        private string ExpectedIndexWithModulesPath = $"{Directory.GetCurrentDirectory()}/TsModuleModifierTests/Expected/WithModules.ts";
        private string ExpectedIndexWithoutModulesPath = $"{Directory.GetCurrentDirectory()}/TsModuleModifierTests/Expected/WithoutModules.ts";

        // save the initial state of the index files
        private readonly string _initialIndexWithModules;
        private readonly string _initialIndexWithoutModules;
        private readonly string _expectedIndexWithModules;
        private readonly string _expectedIndexWithoutModules;

        public RegisterExtensionTests()
        {
            this._initialIndexWithModules = File.ReadAllText(this.IndexWithModulesPath);
            this._initialIndexWithoutModules = File.ReadAllText(this.IndexWithoutModulesPath);
            this._expectedIndexWithModules = File.ReadAllText(this.ExpectedIndexWithModulesPath);
            this._expectedIndexWithoutModules = File.ReadAllText(this.ExpectedIndexWithoutModulesPath);
        }

        [TestMethod]
        public void SuccessfullyRegisterNewExtension_When_IndexFileHasRegisteredModules()
        {
            FileModifierResult result = TsModuleModifier.RegisterExtension(this.IndexWithModulesPath, "YetAnotherModule", "yet-another");
            Assert.IsTrue(result.Success);

            var currentState = File.ReadAllText(this.IndexWithModulesPath);

            Assert.AreEqual(this._expectedIndexWithModules, currentState);
        }

        [TestMethod]
        public void SuccessfullyRegisterNewExtension_When_IndexFileHasNoRegisteredModules()
        {
            FileModifierResult result = TsModuleModifier.RegisterExtension(this.IndexWithoutModulesPath, "TestModule", "test");
            Assert.IsTrue(result.Success);

            var currentState = File.ReadAllText(this.IndexWithoutModulesPath);

            Assert.AreEqual(this._expectedIndexWithoutModules, currentState);
        }

        [TestCleanup]
        public void CleanUp()
        {
            // return the index files to their initial state
            File.WriteAllText(this.IndexWithModulesPath, this._initialIndexWithModules);
            File.WriteAllText(this.IndexWithoutModulesPath, this._initialIndexWithoutModules);
        }
    }
}
