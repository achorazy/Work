using System.Linq;
using ExcelClassAutoMapper.Entities;

namespace ExcelClassAutoMapper
{
    public class MapIntA
    {
        public INT MapEaiToInt(Client client)
        {
            var intEntity = new INT();
            intEntity.AddressClientId = client.Addresses.First().ClientId;
            return intEntity;
        }
    }
}
