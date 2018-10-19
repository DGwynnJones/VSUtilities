
using NUnit.Framework;
using SA.UnitTesting;
using System.Diagnostics;

namespace VSUtilities.Tests
{
    [TestFixture]
    public class TestBaseTester1 : TestBase
    {

        [Test]
        public void Basic_1()
        {
            Trace.WriteLine("1-1");

        }

        [Test]
        public void Basic_2()
        {
            Trace.WriteLine("1-2");

        }

    }
    [TestFixture]
    public class TestBaseTester2 : TestBase
    {

        [Test]
        public void Basic_1()
        {
            Trace.WriteLine("2-1");

        }

        [Test]
        public void Basic_2()
        {
            Trace.WriteLine("2-2");

        }

    }
}


//OneTimeSetUp from SetUpFixture(once per assembly)
//     OneTimeSetUp from TestFixture(once per test class class)
//          SetUp(before each test of the class)
//               Test1
//          TearDown(after each test of the class)
//          SetUp
//               Test2
//          TearDown
//...
//     OneTimeTearDown from TestFixture(once per test class)
//     OneTimeSetUp from TestFixture
//...
//     OneTimeTearDown from TestFixture
//OneTimeTearDown from SetUpFixture(once per assembly)