using System;
using System.Linq;
using ExcelClassAutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ato.MstExcelToCode.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            var excel = MstExcelHelper.InitialiseMstExcelMapping(@"C:\VS\ExcelClassAutoMapper\Ato.MstExcelToCode\Example.xlsx");

            var mappings = MstExcelHelper.GetMstEntityMappings(excel).ToList();

            var mappingHierarchy = MstExcelHelper.ConvertFlatMappingToHierarchy(mappings);
            var results = T4MstExcelHelper.MainLoop(mappingHierarchy);
            Assert.IsTrue(results.Length > 0);
        }
    }
}
