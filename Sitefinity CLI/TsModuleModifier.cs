using System.IO;

namespace Sitefinity_CLI
{
    public static class TsModuleModifier
    {
        public static FileModifierResult RegisterExtension(string bundleIndexFilePath, string extensionName, string folderName)
        {
            string tempFilePath = bundleIndexFilePath + "_temp";
            using (var input = File.OpenText(bundleIndexFilePath))
            {
                using (var output = new StreamWriter(tempFilePath))
                {
                    string line;
                    bool isFirstLine = true;
                    while (null != (line = input.ReadLine()))
                    {
                        if (isFirstLine)
                        {
                            output.WriteLine($"import {{ {extensionName} }} from \"./{folderName}\";");
                            isFirstLine = false;
                        }
                        
                        output.WriteLine(line);

                        if (line.Contains("return ["))
                        {
                            output.Write($"{new string(' ', 12)}{extensionName}");

                            var nextLine = input.ReadLine();
                            if (nextLine != null && !nextLine.Contains("];"))
                            {
                                output.WriteLine($",");
                            }
                            else
                            {
                                output.WriteLine();
                            }

                            output.WriteLine(nextLine);
                        }
                    }
                }
            }

            File.Replace(tempFilePath, bundleIndexFilePath, null);

            return new FileModifierResult { Success = true, Message = $"Extension {extensionName} successfully registered!"};
        }
    }
}
