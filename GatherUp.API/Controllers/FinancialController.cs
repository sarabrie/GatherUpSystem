using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Finance;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GatherUp.API.Controllers
{
    [Authorize]
    public class FinancialController : BaseApiController // 🌟 ירושה ממחלקת הבסיס החדשה
    {
        private readonly FinanceService _financeService;

        // העברת ה-EventsService לבנאי של האבא באמצעות base
        public FinancialController(FinanceService financeService, EventsService eventsService)
            : base(eventsService)
        {
            _financeService = financeService;
        }

        [HttpGet("{eventId}/suppliers")]
        public ActionResult<IEnumerable<VendorAllocation>> GetSuppliers(int eventId)
        {
            if (!IsUserManager(eventId)) return Forbid(); // שימוש בפונקציה מהאבא

            IEnumerable<VendorAllocation> suppliers = _financeService.GetEventSuppliers(eventId);
            return Ok(suppliers);
        }

        [HttpPost("{eventId}/suppliers")]
        public ActionResult AddVendor(int eventId, [FromBody] VendorAllocation newVendor)
        {
            if (newVendor == null) return BadRequest("נתוני ספק לא תקינים.");
            if (!IsUserManager(eventId)) return Forbid();

            _financeService.AddVendorToEvent(eventId, newVendor);
            return Ok(new { Message = "הספק נוסף בהצלחה לאירוע." });
        }

        [HttpGet("{eventId}/receipts-report")]
        public ActionResult<IEnumerable<object>> GetReceiptsReport(int eventId)
        {
            if (!IsUserManager(eventId)) return Forbid();

            IEnumerable<object> report = _financeService.GetFlattenedReceiptsReport(eventId);
            return Ok(report);
        }

        [HttpGet("{eventId}/status")]
        public ActionResult GetFinancialStatus(int eventId)
        {
            if (!IsUserManager(eventId)) return Forbid();

            decimal status = _financeService.CalculateEventFinancialStatus(eventId);
            return Ok(new { EventId = eventId, FinancialBalance = status });
        }

        [HttpPost("receipts")]
        public ActionResult AddReceipt([FromBody] ReceiptDetails receipt)
        {
            if (receipt == null) return BadRequest("נתוני קבלה לא תקינים.");
            _financeService.AddReceipt(receipt);
            return Ok(new { Message = "הקבלה נוספה בהצלחה." });
        }
    }
}