
using Microsoft.Build.Evaluation;
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
        public string ProjectFile { get; private set; }

        public string Platform { get; private set; }
        public string AssemblyName { get; private set; }
        public string OutputPath { get; private set; }
        public string ToolsVersion { get; private set; }

        public IList<string> References { get; private set; }
        public IList<string> CompileItems { get; private set; }

        public Parser()
        {
            References = new List<string>();
            CompileItems = new List<string>();
        }

        public void ParseProjectFiles(string folder)
        {

        }

        public string Items { get; private set; }

        public void ParseProjectFile(string projectFile)
        {
            if (!new FileInfo(projectFile).Exists)
            {
                throw new FileNotFoundException("File not found.", projectFile);
            }

            ProjectFile = projectFile;

            var projectCollection = new ProjectCollection();

            var coll = projectCollection.LoadedProjects;


            try
            {

                var proj = projectCollection.LoadedProjects.First();

                projectCollection.LoadProject(projectFile);

                AssemblyName = proj.GetPropertyValue("AssemblyName");
                OutputPath = proj.GetPropertyValue("OutputPath");
                Platform = proj.GetProperty("Platform").EvaluatedValue;
                Platform = proj.GetProperty("PlatformTarget").EvaluatedValue;
                ToolsVersion = proj.ToolsVersion;

                var sb = new StringBuilder();

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
                            sb.AppendLine(item.ItemType + ": " + item.EvaluatedInclude);
                            break;
                    }


                }

                Items = sb.ToString();

                References = refs.OrderBy(x => x).ToList();
                CompileItems = compiles.OrderBy(x => x).ToList();

            }
            catch (System.Exception ex)
            {
                Trace.WriteLine("ERROR READING FILE: " + ex.Message);
                //throw;
            }

        }

        public override string ToString()
        {
            var result = new StringBuilder("[" + GetType().FullName + "]\n");
            //result.AppendLine("ProjectFile: " + ProjectFile);
            result.AppendLine("Platform: " + Platform);
            result.AppendLine("OutputPathProjectFile: " + OutputPath);
            result.AppendLine("AssemblyName: " + AssemblyName);

            result.AppendLine("References: ");
            foreach (var item in References)
            {
                result.AppendLine("    " + item);
            }

            result.AppendLine("CompileItems: ");
            foreach (var item in CompileItems)
            {
                result.AppendLine("    " + item);
            }

            result.AppendLine("Items: ");
            result.AppendLine(Items);
            result.AppendLine("");

            return result.ToString();

        }

    }
}
