using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatherUp.Core.Interfaces;
using System.Xml.Serialization;
namespace GatherUp.Core.DO.Finance
{
    public class VendorAllocation:IEntity
    {
        [XmlAttribute]
        public int Id { get; set; }
        [XmlElement]
        public string Name { get; set; } = string.Empty;
        [XmlElement]
        public decimal AmountOwed { get; set; } = Decimal.Zero;
        [XmlElement]
        public bool HasReceipt { get; set; }= false;
        [XmlElement]
        public List<ReceiptDetails> Receipts { get; set; } = new List<ReceiptDetails>();
    }
}
