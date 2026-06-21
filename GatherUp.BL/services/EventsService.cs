using GatherUp.Core.DO;
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

        public EventsService(
            IRepository<Event> eventRepo,
            IRepository<Participant> participantRepo,
            IRepository<Poll> pollRepo)
        {
            _eventRepo = eventRepo;
            _participantRepo = participantRepo;
            _pollRepo = pollRepo;
        }

        public void AddEvent(Event newEvent, List<Participant> participants = null, List<Poll> polls = null)
        {
            if (participants != null)
            {
                foreach (Participant participant in participants)
                {
                    _participantRepo.Add(participant);
                    newEvent.ParticipantIds.Add(participant.Id);
                }
            }

            if (polls != null)
            {
                foreach (Poll poll in polls)
                {
                    _pollRepo.Add(poll);
                    newEvent.PollIds.Add(poll.Id);
                }
            }

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

            IEnumerable<Event> managedEvents = allEvents.Where(e => e.EventManagerId == userId);
            IEnumerable<Event> hostedEvents = allEvents.Where(e => e.EventHostId == userId);
            IEnumerable<Event> participatingEvents = allEvents.Where(e => e.ParticipantIds.Contains(userId));

            Console.WriteLine("אירועים שאתה מנהל:");
            foreach (Event ev in managedEvents)
                Console.WriteLine($"  - [{ev.Id}] {ev.Title} | {ev.EventDate:dd/MM/yyyy}");

            Console.WriteLine("אירועים שאתה בעל הבית:");
            foreach (Event ev in hostedEvents)
                Console.WriteLine($"  - [{ev.Id}] {ev.Title} | {ev.EventDate:dd/MM/yyyy}");

            Console.WriteLine("אירועים שאתה משתתף בהם:");
            foreach (Event ev in participatingEvents)
                Console.WriteLine($"  - [{ev.Id}] {ev.Title} | {ev.EventDate:dd/MM/yyyy}");
        }
    }
}
