using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.Interfaces;
using GatherUp.Core.DO;
using GatherUp.Core.DO.Users;
using GatherUp.Core.DO.Finance;

namespace GatherUp.BL.Services
{
    public class FinanceService
    {
        private readonly IRepository<Event> _eventRepo;
        private readonly IRepository<Participant> _participantRepo;
        private readonly IRepository<VendorAllocation> _vendorAllocationRepo;

        public FinanceService(
            IRepository<Event> eventRepo,
            IRepository<Participant> participantRepo,
            IRepository<VendorAllocation> vendorAllocationRepo)
        {
            _eventRepo = eventRepo;
            _participantRepo = participantRepo;
            _vendorAllocationRepo = vendorAllocationRepo;
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
            return GetEventSuppliers(eventId)
                .SelectMany(v => v.Receipts);
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
            return GetEventSuppliers(eventId)
                .SelectMany(vendor => vendor.Receipts)                          
                .OrderByDescending(receipt => receipt.Date)  
                .Select(receipt => new { receipt.ReceiptNumber, receipt.Amount, receipt.Date }) 
                .ToList();
        }
    }
}