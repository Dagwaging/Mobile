using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RHITMobile.Secure.Data_Import;
using System.Diagnostics;

namespace ServiceTest
{
    [TestClass]
    public class ImportTests
    {
        [TestMethod]
        public void TestImport()
        {
            Importer importer = new Importer(new ConsoleLogger(), "C:\\InputData");
            importer.ImportData();
        }
    }
}
