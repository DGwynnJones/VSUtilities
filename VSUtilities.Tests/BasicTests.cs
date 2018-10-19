
using NUnit.Framework;
using SA.UnitTesting;
using System.Diagnostics;
using System.IO;

namespace VSUtilities.Tests
{
    [TestFixture]
    public class BasicTests : TestBase
    {

        string _TestFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestFiles");
        string _TestProjectFile1 = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestFiles\SA.Projects\Avails\Avails.csproj");
        string _TestSolutionFile = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestFiles\SA.Web.sln");

        string _ProductionFolder = @"C:\Users\dgwynnjones\source\repos\SA.RMS2\SA.Web";


        [Test]
        public void Basic_()
        {
            var obj = new Parser();
            obj.ParseProjectFile(_TestProjectFile1);

            Trace.WriteLine(obj.ToString());
        }


        [Test]
        public void Open_Project_File()
        {
            var obj = new Parser();
            obj.ParseProjectFile(_TestProjectFile1);

            Trace.WriteLine(obj);

            Assert.That(obj.AssemblyName, Is.Not.Empty);
            Assert.That(obj.OutputPath, Is.Not.Empty);
            Assert.That(obj.Platform, Is.Not.Empty);
            Assert.That(obj.ProjectFile, Is.Not.Empty);

        }

        [Test]
        public void Open_Project_Files()
        {
            var files = Directory.GetFiles(_TestFolder, "*.csproj", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                Trace.WriteLine("File: " + file);
                var obj = new Parser();
                obj.ParseProjectFile(file);

                Trace.WriteLine(obj);



            }

        }

        [Test]
        public void Open_Project_Files_PROD()
        {
            var files = Directory.GetFiles(_ProductionFolder, "*.csproj", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fi = new FileInfo(file);

                Trace.WriteLine("File: " + fi.Name);

                var obj = new Parser();
                obj.ParseProjectFile(file);

                Trace.WriteLine(obj);
            }

        }

    }
}
