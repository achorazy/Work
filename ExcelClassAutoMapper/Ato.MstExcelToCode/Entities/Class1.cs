using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelClassAutoMapper.Entities
{
    public enum MapTypeEnum
    {
        Fact,
        Heading,
        Context
    }

    public class MstEntityMapping
    {
        public MapTypeEnum HeadMapType { get; set; }
        public string HeadContext { get; set; }
        public string HeadSouce { get; set; }
        public string HeadTarget { get; set; }
        public int ColumnIndex { get; set; }
        public List<MstEntityMapping> Children { get; set; }

        public override string ToString()
        {
            return HeadMapType + " - " + HeadContext + " - " + HeadTarget;
        }
    }


    public class INT
    {
        public string AddressClientId { get; set; }
    }

    public class RP
    {
        public string Abn { get; set; }
        public List<RpAddress> RpAddresses { get; set; }
    }

    public class RpAddress
    {
        public string AddressId { get; set; }
        public string AddressCode { get; set; }
        public string Line1 { get; set; }
    }

    //Client.Addresses.Address.ClientId
    public class Client
    {
        public string Abn { get; set; }
        public List<Address> Addresses { get; set; }
    }

    public class Address
    {
        public string ClientId { get; set; }
        public string AddressId { get; set; }
        public string AddressTypeCode { get; set; }
        public Complex Complex { get; set; }
        //.ComplexLine1

    }

    public class Complex
    {
        public string ComplexLine1 { get; set; }
    }
}
