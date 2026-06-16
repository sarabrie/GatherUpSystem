using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.Interfaces;
using GatherUp.Core.DO.Finance;
using GatherUp.Core.DO.Users;
using GatherUp.Core.DO;

namespace GatherUp.BL.Services
{
    public class FinanceService
    {
        private readonly IRepository<VendorAllocation> _vendorRepo;
        private readonly IRepository<ReceiptDetails> _receiptRepo;
        private readonly IRepository<Participant> _participantRepo;
        private readonly IRepository<Event> _eventRepo;

        public FinanceService(
            IRepository<VendorAllocation> vendorRepo,
            IRepository<ReceiptDetails> receiptRepo,
            IRepository<Participant> participantRepo,
            IRepository<Event> eventRepo)
        {
            _vendorRepo = vendorRepo;
            _receiptRepo = receiptRepo;
            _participantRepo = participantRepo;
            _eventRepo = eventRepo;
        }

        public IEnumerable<VendorAllocation> GetEventSuppliersAndReceipts(int eventId)
        {
            Event ev = _eventRepo.GetById(eventId);

            if (ev == null || ev.VendorIds == null)
                return Enumerable.Empty<VendorAllocation>();

            IEnumerable<VendorAllocation> allVendors = _vendorRepo.GetAll();

            return allVendors.Where(v => ev.VendorIds.Contains(v.Id));
        }

        public decimal GetTotalEstimatedExpenses()
        {
            return _vendorRepo.GetAll()
                .Sum(v => v.AmountOwed);
        }

        public decimal GetTotalActualExpenses()
        {
            return _receiptRepo.GetAll()
                .Sum(r => r.Amount);
        }

        public decimal GetEventBalance()
        {
            decimal totalIncome = _participantRepo.GetAll()
                .Where(p => p.HasPaid)
                .Sum(p => p.AmountContributed);

            decimal totalExpenses = GetTotalActualExpenses();

            return totalIncome - totalExpenses;
        }
    }
}