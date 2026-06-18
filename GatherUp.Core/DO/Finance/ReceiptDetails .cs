using System;
using System.Diagnostics.CodeAnalysis;
using GatherUp.Core.Interfaces;

namespace GatherUp.Core.DO.Finance
{
    public class ReceiptDetails : IEntity
    {
        [SetsRequiredMembers]
        public ReceiptDetails()
        {
        }
        public int Id { get; set; }
        public required string ReceiptNumber { get; set; } = string.Empty;
        public required decimal Amount { get; set; }
        public required string ReceiptFilePath { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
    }

}
//בגלל שאין סרילזציה בניהול הקובץ אין כאן אנוטציות