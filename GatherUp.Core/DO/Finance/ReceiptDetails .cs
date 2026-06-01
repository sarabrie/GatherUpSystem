using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO.Finance
{
    internal class ReceiptDetails
    {
        public string ReceiptNumber { get; set; }
        public decimal Amount { get; set; } 
        public bool HasRecipt { get; set; }
        public List<ReceiptDetails> Receipts { get; set; } = new List<ReceiptDetails>();
    }
}
