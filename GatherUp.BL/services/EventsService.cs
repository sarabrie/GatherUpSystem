using GatherUp.Core.DO;
using GatherUp.Core.DO.Finance;
using GatherUp.Core.DO.Polls;
using GatherUp.Core.DO.Users;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL.Services
{
    public class EventsService
    {
        private readonly IRepository<Event> _eventRepo;
        private readonly IRepository<Participant> _participantRepo;
        private readonly IRepository<Poll> _pollRepo;
        private readonly IRepository<VendorAllocation> _vendorRepo;

        public EventsService(
            IRepository<Event> eventRepo,
            IRepository<Participant> participantRepo,
            IRepository<Poll> pollRepo,
            IRepository<VendorAllocation> vendorRepo)
        {
            _eventRepo = eventRepo;
            _participantRepo = participantRepo;
            _pollRepo = pollRepo;
            _vendorRepo = vendorRepo;
        }

        public void AddEvent(Event newEvent, List<Participant> participants = null, List<Poll> polls = null)
        {
            participants?.ForEach(p => { _participantRepo.Add(p); newEvent.ParticipantIds.Add(p.Id); });
            polls?.ForEach(p => { _pollRepo.Add(p); newEvent.PollIds.Add(p.Id); });
            _eventRepo.Add(newEvent);
        }

public Event GetEventDetails(int eventId)
        {
            Event ev = _eventRepo.GetById(eventId);
            if (ev == null)
                throw new KeyNotFoundException($"אירוע עם מזהה {eventId} לא נמצא");

            return ev;
        }

        public object GetEventsByUser(int userId)
        {
            IEnumerable<Event> allEvents = _eventRepo.GetAll();

            List<Event> managerEvents = allEvents.Where(e => e.EventManagerId == userId).ToList();
            List<Event> hostEvents = allEvents.Where(e => e.EventHostId == userId).ToList();

            List<Event> participantEvents = allEvents.Where(e => e.ParticipantIds != null && e.ParticipantIds.Contains(userId)).ToList();

            return new
            {
                Managing = managerEvents,
                Hosting = hostEvents,
                Participating = participantEvents
            };
        }
    }
}
