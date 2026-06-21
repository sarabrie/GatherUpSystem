using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.Interfaces;
using GatherUp.Core.DO;
using GatherUp.Core.DO.Users;
using GatherUp.Core.DO.Finance;
using System.Numerics;

namespace GatherUp.BL.Services
{
    public class FinanceService
    {
        private readonly IRepository<Event> _eventRepo;
        private readonly IRepository<Participant> _participantRepo;
        private readonly IRepository<VendorAllocation> _vendorAllocationRepo;
        private readonly IRepository<ReceiptDetails> _receiptRepo;

        public FinanceService(
            IRepository<Event> eventRepo,
            IRepository<Participant> participantRepo,
            IRepository<VendorAllocation> vendorAllocationRepo,
            IRepository<ReceiptDetails> receiptRepo)
        {
            _eventRepo = eventRepo;
            _participantRepo = participantRepo;
            _vendorAllocationRepo = vendorAllocationRepo;
            _receiptRepo = receiptRepo;
        }

        public IEnumerable<VendorAllocation> GetEventSuppliers(int eventId)
        {
            Event ev = _eventRepo.GetById(eventId);
            if (ev == null || ev.VendorIds == null)
                return Enumerable.Empty<VendorAllocation>();

            return _vendorAllocationRepo.GetAll().Where(v => ev.VendorIds.Contains(v.Id));
        }

        public IEnumerable<ReceiptDetails> GetEventReceipts(int eventId)
        {
            IEnumerable<int> receiptIds = GetEventSuppliers(eventId).SelectMany(v => v.ReceiptIds);
            return _receiptRepo.GetAll().Where(r => receiptIds.Contains(r.Id));
        }

        public decimal CalculateEventFinancialStatus(int eventId)
        {
            return _participantRepo.GetAll()
            .Where(p => p.IsAttending == true && p.HasPaid == true)
            .Sum(p => p.AmountContributed)
            - GetEventSuppliers(eventId).Sum(v => v.AmountOwed);
        }

        public IEnumerable<object> GetFlattenedReceiptsReport(int eventId)
        {
            return GetEventReceipts(eventId)
                .OrderByDescending(receipt => receipt.Date)
                .Select(receipt => new { receipt.ReceiptNumber, receipt.Amount, receipt.Date })
                .ToList();
        }

        public void AddReceipt(ReceiptDetails receipt)
        {
            if (receipt == null) return;
            _receiptRepo.Add(receipt);
        }
        public void AddVendorToEvent(int eventId, VendorAllocation newVendor)
        {
            // 1. שליפת האירוע הקיים מה-XML באמצעות ה-EventRepository
            var currentEvent = _eventRepo.GetById(eventId);
            if (currentEvent == null) return;

            // 2. הוספת ה-Id של הספק לרשימת הספקים של האירוע (הגיונית)
            if (currentEvent.VendorIds == null)
            {
                currentEvent.VendorIds = new List<int>();
            }

            currentEvent.VendorIds.Add(newVendor.Id);

            // 3. שמירה ועדכון של האירוע ב-XML
            _eventRepo.Update(currentEvent);
        }
    }
}