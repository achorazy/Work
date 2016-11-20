using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelClassAutoMapper.Entities;
using LinqToExcel;

namespace ExcelClassAutoMapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.Run();
        }

        private void Run()
        {
            var excel = MstExcelHelper.InitialiseMstExcelMapping("Example.xlsx");

            var mappings = MstExcelHelper.GetMstEntityMappings(excel).ToList();

            var mappingHierachy = MstExcelHelper.ConvertFlatMappingToHierachy(mappings);
        }

        
    }
}
