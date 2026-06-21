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

        public void GetEventsByUser(int userId)
        {
            IEnumerable<Event> allEvents = _eventRepo.GetAll();

            Console.WriteLine("אירועים שאתה מנהל:");
            allEvents.Where(e => e.EventManagerId == userId)
                     .ToList()
                     .ForEach(e => Console.WriteLine($"  - [{e.Id}] {e.Title} | {e.EventDate:dd/MM/yyyy}"));

            Console.WriteLine("אירועים שאתה בעל הבית:");
            allEvents.Where(e => e.EventHostId == userId)
                     .ToList()
                     .ForEach(e => Console.WriteLine($"  - [{e.Id}] {e.Title} | {e.EventDate:dd/MM/yyyy}"));

            Console.WriteLine("אירועים שאתה משתתף בהם:");
            allEvents.Where(e => e.ParticipantIds.Contains(userId))
                     .ToList()
                     .ForEach(e => Console.WriteLine($"  - [{e.Id}] {e.Title} | {e.EventDate:dd/MM/yyyy}"));
        }
    }
}
