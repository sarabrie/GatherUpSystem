using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatherUp.Core.Interfaces;
namespace GatherUp.Core.DO.Finance
{
    public class VendorAllocation:IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal AmountOwed { get; set; } = Decimal.Zero;
        public bool HasReceipt { get; set; }= false;
        public List<ReceiptDetails> Receipts { get; set; } = new List<ReceiptDetails>();
    }
}
