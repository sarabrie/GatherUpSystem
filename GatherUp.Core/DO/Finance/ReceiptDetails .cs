using System;
using GatherUp.Core.Interfaces;

namespace GatherUp.Core.DO.Finance
{
    public class ReceiptDetails : IEntity
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
//בגלל שאין סרילזציה בניהול הקובץ אין כאן אנוטציות