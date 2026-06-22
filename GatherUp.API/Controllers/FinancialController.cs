using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Finance;

namespace GatherUp.API.Controllers
{
    [Authorize]
    public class FinancialController : BaseApiController
    {
        private readonly FinanceService _financeService;

        public FinancialController(FinanceService financeService, EventsService eventsService)
            : base(eventsService)
        {
            _financeService = financeService;
        }

        [HttpGet("{eventId}/suppliers")]
        public ActionResult GetSuppliers(int eventId)
        {
            if (!IsUserManager(eventId)) return Forbid();
            return Ok(_financeService.GetEventSuppliers(eventId));
        }

        [HttpPost("{eventId}/suppliers")]
        public ActionResult AddVendor(int eventId, [FromBody] VendorAllocation newVendor)
        {
            if (newVendor == null) return BadRequest(new { error = "נתוני ספק לא תקינים." });
            if (!IsUserManager(eventId)) return Forbid();
            _financeService.AddVendorToEvent(eventId, newVendor);
            return Ok(new { message = "הספק נוסף בהצלחה לאירוע." });
        }

        [HttpGet("{eventId}/receipts-report")]
        public ActionResult GetReceiptsReport(int eventId)
        {
            if (!IsUserManager(eventId)) return Forbid();
            return Ok(_financeService.GetFlattenedReceiptsReport(eventId));
        }

        [HttpGet("{eventId}/status")]
        public ActionResult GetFinancialStatus(int eventId)
        {
            if (!IsUserManager(eventId)) return Forbid();
            decimal status = _financeService.CalculateEventFinancialStatus(eventId);
            return Ok(new { eventId, financialBalance = status });
        }

        [HttpPost("receipts")]
        public ActionResult AddReceipt([FromBody] ReceiptDetails receipt)
        {
            if (receipt == null) return BadRequest(new { error = "נתוני קבלה לא תקינים." });
            _financeService.AddReceipt(receipt);
            return Ok(new { message = "הקבלה נוספה בהצלחה." });
        }
    }
}
