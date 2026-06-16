using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.Interfaces;
using GatherUp.Core.DO.Users; 
using GatherUp.Core.DO;  

namespace GatherUp.BL.Services
{
    public class EventManagerService
    {
        private readonly IRepository<Participant> _participantRepo;
        private readonly IRepository<Event> _eventRepo;

        public EventManagerService(IRepository<Participant> participantRepo, IRepository<Event> eventRepo)
        {
            _participantRepo = participantRepo;
            _eventRepo = eventRepo;
        }

        public void AddParticipantToEvent(int eventId, Participant participant)
        {
            _participantRepo.Add(participant);

            Event ev = _eventRepo.GetById(eventId);
            if (ev != null)
            {
                if (ev.ParticipantIds == null)
                    ev.ParticipantIds = new List<int>();

                ev.ParticipantIds.Add(participant.Id);
                _eventRepo.Update(ev);
            }
        }

        public IEnumerable<Participant> GetAttendingParticipants()
        {
            return _participantRepo.GetAll()
                .Where(p => p.IsAttending == true);
        }

        public IEnumerable<Participant> GetUndecidedParticipants()
        {
            return _participantRepo.GetAll()
                .Where(p => p.IsAttending == null);
        }

        public decimal GetTotalAmountCollected()
        {
            return _participantRepo.GetAll()
                .Where(p => p.HasPaid)
                .Sum(p => p.AmountContributed);
        }
    }
}