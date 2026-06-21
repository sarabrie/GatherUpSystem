using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Finance;
using System.Collections.Generic;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinancialController : ControllerBase
    {
        private readonly FinanceService _financeService;

        public FinancialController(FinanceService financeService)
        {
            _financeService = financeService;
        }

        // שליפת הספקים של אירוע מסוים
        [HttpGet("{eventId}/suppliers")]
        public ActionResult<IEnumerable<VendorAllocation>> GetSuppliers(int eventId)
        {
            var suppliers = _financeService.GetEventSuppliers(eventId);
            return Ok(suppliers);
        }

        // הוספת ספק (Vendor) חדש לאירוע
        [HttpPost("{eventId}/suppliers")]
        public ActionResult AddVendor(int eventId, [FromBody] VendorAllocation newVendor)
        {
            if (newVendor == null)
            {
                return BadRequest("נתוני ספק לא תקינים.");
            }

            _financeService.AddVendorToEvent(eventId, newVendor);
            return Ok(new { Message = "הספק נוסף בהצלחה לאירוע." });
        }

        // קבלת דוח קבלות שטוח וממוין
        [HttpGet("{eventId}/receipts-report")]
        public ActionResult<IEnumerable<object>> GetReceiptsReport(int eventId)
        {
            var report = _financeService.GetFlattenedReceiptsReport(eventId);
            return Ok(report);
        }

        // חישוב המצב הפיננסי הדינמי הנוכחי של האירוע
        [HttpGet("{eventId}/status")]
        public ActionResult GetFinancialStatus(int eventId)
        {
            decimal status = _financeService.CalculateEventFinancialStatus(eventId);
            return Ok(new { EventId = eventId, FinancialBalance = status });
        }

        // העלאת/הוספת קבלה חדשה למערכת
        [HttpPost("receipts")]
        public ActionResult AddReceipt([FromBody] ReceiptDetails receipt)
        {
            if (receipt == null)
            {
                return BadRequest("נתוני קבלה לא תקינים.");
            }

            _financeService.AddReceipt(receipt);
            return Ok(new { Message = "הקבלה נקלטה בהצלחה במערכת." });
        }
    }
}