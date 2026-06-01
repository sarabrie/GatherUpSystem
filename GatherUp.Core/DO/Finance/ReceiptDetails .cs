using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO.Finance
{
    public record ReceiptDetails(string ReceiptNumber, decimal Amount, DateTime Date);
}
