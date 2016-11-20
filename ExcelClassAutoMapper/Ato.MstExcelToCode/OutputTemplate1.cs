  
  
  
    
  
  
  
  
  
  
 
 
 

using System.Linq;
using ExcelClassAutoMapper.Entities;

namespace ExcelClassAutoMapper
{
    public class MapInt
    {
				    public INT MapEaiToINT(Client client)
        {
            var iNTEntity = new INT();
            iNTEntity.AddressClientId = Client.Addresses.Address.ClientId;
           return iNTEntity;
        }
    public RP MapEaiToRP(Client client)
        {
            var rPEntity = new RP();
            rPEntity.Abn = Client.Abn;
           return rPEntity;
        }
    public Address MapEaiToAddress(Client client)
        {
            var addressEntity = new Address();
            rPEntity.RpAddress.AddressId = Client.Addresses.Address.AddressId;
            rPEntity.RpAddress.AddressCode = Client.Addresses.Address.AddressTypeCode;
            rPEntity.RpAddress.Line1 = Client.Addresses.Address.Complex.ComplexLine1;
           return addressEntity;
        }

        public INT MapEaiToInt(Client client)
        {
            var intEntity = new INT();
            intEntity.AddressClientId = client.Addresses.First().ClientId;
            return intEntity;
        }
    }
}
