using System;
using System.Collections.Generic;
using System.Linq;
using ExcelClassAutoMapper.Entities;
using LinqToExcel;

namespace ExcelClassAutoMapper
{
    public class T4MstExcelHelper
    {
        public static string MainLoop(List<MstEntityMapping> mappingHierarchy)
        {
            var result = string.Empty;
            foreach (var mapping in mappingHierarchy)
            {
                if (mapping.HeadMapType == MapTypeEnum.Heading)
                {
                    result += BuildMethodStart(mapping);
                    result += MainLoop(mapping.Children);
                    result += BuildMethodEnd(mapping);
                }
                else
                {
                    result += BuildPropertyMapping(mapping);
                }
            }
            return result;
        }

        public static string BuildMethodStart(MstEntityMapping mapping)
        {
            return "    public " + mapping.HeadContext + " MapEaiTo" + mapping.HeadContext + @"(Client client)
        {
            var " + GetEntityName(mapping) + " = new " +
                   mapping.HeadContext + "();" + Environment.NewLine;

        }

        public static string BuildPropertyMapping(MstEntityMapping mapping)
        {
            return "            " + GetEntityName(mapping) + "." + mapping.HeadTarget + " = " + mapping.HeadSouce + ";" + Environment.NewLine;
        }

        public static string BuildMethodEnd(MstEntityMapping mapping)
        {
            return @"           return " + GetEntityName(mapping) + @";
        }" + Environment.NewLine;
        }

        public static string GetEntityName(MstEntityMapping mapping)
        {
            return Char.ToLowerInvariant(mapping.HeadContext[0]) + mapping.HeadContext.Substring(1) + "Entity";
        }
    }

    public class MstExcelHelper
    {
        public static IEnumerable<MstEntityMapping> GetMstEntityMappings(ExcelQueryFactory excel)
        {
            var mappings = (from c in excel.Worksheet<MstEntityMapping>()
                                //where c.State == "IN" && c.Employees > 500
                            select c);
            return mappings;
        }

        public static ExcelQueryFactory InitialiseMstExcelMapping(string excelFilePath)
        {
            var excel = new ExcelQueryFactory(excelFilePath);
            excel.AddMapping<MstEntityMapping>(x => x.HeadContext, "HeadContext");
            //excel.AddMapping<MstEntityMapping>("Employees", "Employee Count");       //maps the "Employees" property to the "Employee Count" column

            excel.AddTransformation<MstEntityMapping>(x => x.HeadMapType,
                cellValue => (MapTypeEnum)Enum.Parse(typeof(MapTypeEnum), cellValue));
            return excel;
        }

        public static List<MstEntityMapping> ConvertFlatMappingToHierarchy(List<MstEntityMapping> mappings)
        {
            var results = new List<MstEntityMapping>();
            foreach (var mapping in mappings)
            {
                if (mapping.HeadMapType == MapTypeEnum.Heading)
                {
                    results.Add(mapping);
                }
                else //if (mapping.HeadMapType == MapTypeEnum.Heading)
                {
                    results.Last().Children = results.Last().Children ?? new List<MstEntityMapping>();
                    results.Last().Children.Add(mapping);
                }
            }
            return results;
        }
    }
}