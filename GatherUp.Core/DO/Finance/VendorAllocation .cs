using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO.Finance
{
    internal class VendorAllocation
    {
        public string Name { get; set; }
        public decimal AmountOwed { get; set; }
        public bool HasReciept { get; set; }
        List<ReceiptDetails> Receipts { get; set; } = new List<ReceiptDetails>();
    }
}
