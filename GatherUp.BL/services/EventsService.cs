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
            // 1. שמור משתתפים (אם לא קיימים כבר)
            if (participants != null)
                foreach (var p in participants)
                    if (!_participantRepo.GetAll().Any(x => x.Id == p.Id))
                        _participantRepo.Add(p);

            // 2. שמור סקרים
            if (polls != null)
                foreach (var poll in polls)
                    _pollRepo.Add(poll);

            // 3. הגדר IDs על ה-newEvent לפני השמירה
            if (participants != null)
                foreach (var p in participants)
                    if (!newEvent.ParticipantIds.Contains(p.Id))
                        newEvent.ParticipantIds.Add(p.Id);

            if (polls != null)
                foreach (var poll in polls)
                    if (!newEvent.PollIds.Contains(poll.Id))
                        newEvent.PollIds.Add(poll.Id);

            // 4. שמור את האירוע עם כל ה-IDs כבר בתוכו
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
