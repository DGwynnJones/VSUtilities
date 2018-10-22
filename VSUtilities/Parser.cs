
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
        public string ParseResult { get; private set; }
        public string Platform { get; private set; }
        public string PlatformTarget { get; private set; }
        public string AssemblyName { get; private set; }
        public string OutputPath { get; private set; }
        public string ToolsVersion { get; private set; }

        public IList<string> ProjectReferences { get; private set; }
        public IList<string> DllReferences { get; private set; }

        public IList<string> CompileItems { get; private set; }
        //public string Items { get; private set; }

        public Parser()
        {
            CompileItems = new List<string>();
        }

        public void ParseProjectFile(string projectFile)
        {
            if (!new FileInfo(projectFile).Exists)
            {
                throw new FileNotFoundException("File not found.", projectFile);
            }

            ProjectFile = projectFile;

            var projectCollection = new ProjectCollection();

            try
            {
                projectCollection.LoadProject(projectFile);

                var coll = projectCollection.LoadedProjects;
                var proj = projectCollection.LoadedProjects.FirstOrDefault();

                ProjectReferences = new List<string>();
                DllReferences = new List<string>();

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
                    DllReferences.Add(item.EvaluatedInclude);
                }



                AssemblyName = proj.GetPropertyValue("AssemblyName");
                OutputPath = proj.GetPropertyValue("OutputPath");
                Platform = proj.GetProperty("Platform").UnevaluatedValue;
                Platform = GetPropertyValue(proj, "PlatformTarget");
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

                //Items = sb.ToString();

                //References = refs.OrderBy(x => x).ToList();
                CompileItems = compiles.OrderBy(x => x).ToList();

                ParseResult = "OK";
            }
            catch (System.Exception ex)
            {
                ParseResult = "ERROR READING FILE: " + ex.Message;
                Trace.WriteLine("ERROR READING FILE: " + ex.Message);
                //throw ex;
            }

        }

        public string GetValidationMessage()
        {
            var result = "";
            if (DllReferences.Where(x => x.StartsWith("Filmtrack")).Any())
            {
                result = "DllReferences contain Filmtrack references.";
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
            result.AppendLine("ParseResult: " + ParseResult);
            result.AppendLine("Platform: " + Platform);
            result.AppendLine("PlatformTarget: " + PlatformTarget);
            result.AppendLine("OutputPathProjectFile: " + OutputPath);
            result.AppendLine("AssemblyName: " + AssemblyName);
            result.AppendLine("Valid: " + GetValidationMessage());

            result.AppendLine("ProjectReferences: ");
            foreach (var item in ProjectReferences.OrderBy(x => x))
            {
                result.AppendLine("    " + item);
            }

            result.AppendLine("DllReferences: ");
            foreach (var item in DllReferences.OrderBy(x => x))
            {
                if (item.ToUpper().StartsWith("FILMTRACK"))
                {
                    result.AppendLine("*   " + item);
                }
                else
                {
                    result.AppendLine("    " + item);
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
