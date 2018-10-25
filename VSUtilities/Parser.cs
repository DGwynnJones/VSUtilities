using Microsoft.Build.Evaluation;
using SA.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace VSUtilities
{
    /// <summary>
    /// Parses Visual Studio project files
    /// </summary>
    public class Parser
    {
        public DirectoryInfo ProjectRoot { get; private set; }
        public string ProjectFile { get; private set; }
        public string ParseResult { get; private set; }
        public string Platform { get; private set; }
        public string PlatformTarget { get; private set; }
        public string TargetFrameworkVersion { get; private set; }
        public string AssemblyName { get; private set; }
        public string OutputPath { get; private set; }
        public string ToolsVersion { get; private set; }

        public IList<string> ProjectReferences { get; private set; } = new List<string>();
        public IList<DllReference> DllReferences { get; private set; } = new List<DllReference>();
        public IList<string> CompileItems { get; private set; } = new List<string>();

        public Parser(string projectFile)
        {
            ProjectFile = projectFile;
            ParseProjectFile(projectFile);
        }

        private void ParseProjectFile(string projectFile)
        {
            var fiProjectFile = new FileInfo(projectFile);
            if (fiProjectFile.Exists == false)
            {
                throw new FileNotFoundException("File not found.", projectFile);
            }
            this.ProjectRoot = fiProjectFile.Directory;


            var proj = GetProject(projectFile);

            if (proj != null)
            {
                AssignProperties(proj);
            }
            else
            {
                Trace.WriteLine("ERROR PARSING PROJECT FILE - " + projectFile);
            }


        }

        private void AssignProperties(Project proj)
        {
            ProjectReferences = new List<string>();
            DllReferences = new List<DllReference>();

            var projRefs = proj.GetItems("ProjectReference");
            foreach (var item in projRefs)
            {
                //this.ProjectReferences.Add(item.EvaluatedInclude);

                var name = item.Metadata.Where(x => x.Name == "Name").First().EvaluatedValue;

                ProjectReferences.Add(name);

            }

            var dllRefs = proj.GetItems("Reference");
            foreach (var item in dllRefs)
            {
                var hintPath = "";
                if (item.Metadata.ToList().Count > 0)
                {
                    hintPath = item.Metadata.ToList()[0].EvaluatedValue;
                }

                DllReferences.Add(new DllReference(ProjectFile) { Assembly = item.EvaluatedInclude, HintPath = hintPath });
            }


            TargetFrameworkVersion = proj.GetPropertyValue("TargetFrameworkVersion");
            AssemblyName = proj.GetPropertyValue("AssemblyName");
            OutputPath = proj.GetPropertyValue("OutputPath");
            Platform = proj.GetProperty("Platform").UnevaluatedValue;
            PlatformTarget = GetPropertyValue(proj, "PlatformTarget");
            ToolsVersion = proj.ToolsVersion;

            //var sb = new StringBuilder();

            var refs = new List<string>();
            var compiles = new List<string>();

            foreach (var item in proj.Items)
            {
                switch (item.ItemType)
                {
                    case "Reference":
                        refs.Add(item.EvaluatedInclude);
                        break;

                    case "Compile":
                        compiles.Add(item.EvaluatedInclude);
                        break;

                    default:
                        //sb.AppendLine(item.ItemType + ": " + item.EvaluatedInclude);
                        break;
                }


            }

            CompileItems = compiles.OrderBy(x => x).ToList();

            ParseResult = "OK";
        }

        private Project GetProject(string projectFile)
        {
            var result = (Project)null;

            try
            {
                var projectCollection = new ProjectCollection();
                projectCollection.LoadProject(projectFile);
                result = projectCollection.LoadedProjects.FirstOrDefault();
            }
            //catch (InvalidProjectFileException exInvalidPRoject)
            catch (Microsoft.Build.Exceptions.InvalidProjectFileException)
            {
                //var p = 0;
                throw new UnknownProjectFormatException(); // exInvalidPRoject;
            }


            return result;
        }

        private string GetValidationMessage()
        {
            var result = "";
            if (DllReferences != null && DllReferences.Where(x => x.Assembly.StartsWith("Filmtrack")).Any())
            {
                result = "DllReferences contain Filmtrack references.";
            }

            foreach (var item in DllReferences)
            {
                if (item.HintPath.StartsWith("."))
                {
                    // Try and find the dll referred to
                    var projectFileFolder = new FileInfo(ProjectFile).Directory;

                    var absoluteAssembly2 = new FileInfo(Path.Combine(projectFileFolder.FullName, item.HintPath));
                }
            }

            return result;
        }

        private string GetPropertyValue(Project project, string key)
        {
            var result = (string)null;
            var prop = project.GetProperty(key);
            if (prop != null)
            {
                result = prop.UnevaluatedValue;
            }
            else
            {
                result = "[null]";
            }
            return result;
        }

        public override string ToString()
        {
            var result = new StringBuilder("[" + GetType().FullName + "]\n");

            result.AppendLine("ProjectFile: " + ProjectFile);
            result.AppendLine("ProjectRoot: " + ProjectRoot);
            result.AppendLine("ParseResult: " + ParseResult);
            result.AppendLine("TargetFrameworkVersion: " + TargetFrameworkVersion);
            result.AppendLine("Platform: " + Platform);
            result.AppendLine("PlatformTarget: " + PlatformTarget);
            result.AppendLine("OutputPathProjectFile: " + OutputPath);
            result.AppendLine("AssemblyName: " + AssemblyName);
            result.AppendLine("Valid: " + GetValidationMessage());
            result.AppendLine("CompileItems: " + CompileItems.Count);

            result.AppendLine("ProjectReferences: ");
            if (ProjectReferences != null)
            {
                foreach (var item in ProjectReferences.OrderBy(x => x))
                {
                    result.AppendLine("    " + item);
                }
            }

            if (DllReferences != null)
            {
                result.AppendLine("DllReferences: ");
                foreach (var item in DllReferences.OrderBy(x => x.Assembly))
                {
                    if (item.Assembly.ToUpper().StartsWith("FILMTRACK"))
                    {
                        result.AppendLine("*   " + item);
                    }
                    else
                    {
                        //result.AppendLine("    " + item);
                        result.Append(Indent.IndentString(item.ToString(), 4));
                    }
                }
            }

            //result.AppendLine("CompileItems: ");
            //foreach (var item in CompileItems)
            //{
            //    result.AppendLine("    " + item);
            //}

            //result.AppendLine("Items: ");
            //result.AppendLine(Items);
            //result.AppendLine("");

            return result.ToString();

        }

    }
}
