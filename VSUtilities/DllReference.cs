
using SA.Utilities.ExtensionMethods;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace VSUtilities
{
    /// <summary>
    /// Visual Studio reference to a DLL
    /// </summary>
    public class DllReference
    {

        public string HintPath { get; set; }
        public string HintPathAbsolute { get; set; }
        public bool HintPathAbsoluteExists { get; private set; }
        public string Assembly { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public string ProjectFile { get; set; }
        public string ProjectRoot { get; private set; }

        public DllReference(string projectFile)
        {
            ProjectFile = projectFile;
        }

        public override string ToString()
        {

            if (HintPath.IsNotNullOrEmpty())
            {
                var fi = new FileInfo(HintPath);
                if (!fi.Exists)
                {
                    Warnings.Add("HintPath not found.");
                }
            }

            HintPathAbsolute = HintPath;

            if (HintPath.StartsWith("."))
            {
                // Try and find the dll referred to
                var projectFileFolder = new FileInfo(ProjectFile).Directory;
                this.ProjectRoot = projectFileFolder.FullName;

                var absoluteAssembly = new FileInfo(Path.Combine(projectFileFolder.FullName, HintPath));
                HintPathAbsoluteExists = absoluteAssembly.Exists;

                HintPathAbsolute = absoluteAssembly.FullName;

                Warnings.Add("Checking: " + absoluteAssembly.FullName);

                if (absoluteAssembly.Exists == false)
                {
                    Warnings.Add("@@" + absoluteAssembly.FullName + " not found.");
                }
            }

            var result = new StringBuilder("[" + GetType().FullName + "]\n");

            result.AppendLine("Assembly: " + Assembly);
            result.AppendLine("ProjectRoot: " + ProjectRoot);
            result.AppendLine("HintPath: " + HintPath);
            result.AppendLine("HintPathAbsolute: " + HintPathAbsolute);
            result.AppendLine("HintPathAbsoluteExists: " + HintPathAbsoluteExists);
            result.AppendLine("IsOutSideProjectPath: " + IsOutSideProjectPath(HintPathAbsolute));

            foreach (var item in Warnings)
            {
                result.AppendLine("Warning: " + item);
            }

            return result.ToString();

        }


        private bool IsOutSideProjectPath(string hintPathAbsolute)
        {
            Trace.WriteLine("    hintPathAbsolute: " + hintPathAbsolute);
            var fiAbs = new FileInfo(hintPathAbsolute).Directory.FullName;


            Trace.WriteLine("         ProjectFile: " + ProjectFile);


            return ProjectFile.StartsWith(fiAbs);
        }

    }

}
