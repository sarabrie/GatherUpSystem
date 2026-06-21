using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.Interfaces;
using GatherUp.Core.DO;
using GatherUp.Core.DO.Users;
using GatherUp.Core.DO.Polls;

namespace GatherUp.BL.Services
{
    public class EventManagerService
    {
        private readonly IRepository<Event> _eventRepo;
        private readonly IRepository<Participant> _participantRepo;
        private readonly IMailService _mailService;

        public EventManagerService(
            IRepository<Event> eventRepo,
            IRepository<Participant> participantRepo,
            IMailNotificationBridge notificationBridge, 
            IMailService mailService)
        {
            _eventRepo = eventRepo;
            _participantRepo = participantRepo;
            _mailService = mailService;

            notificationBridge.OnParticipantAction += HandleParticipantAction;
            notificationBridge.OnEventAction += HandleEventAction;
        }

        public IEnumerable<Participant> GetParticipantsForEvent(int eventId)
        {
            Event ev = _eventRepo.GetById(eventId);
            if (ev == null || ev.ParticipantIds == null)
                return Enumerable.Empty<Participant>();

            return _participantRepo.GetAll().Where(p => ev.ParticipantIds.Contains(p.Id));
        }

        public void AddParticipantToEvent(int eventId, Participant participant)
        {
            if (participant == null) return;

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

        public void UpdateEventDetails(int eventId, Event updatedEvent)
        {
            if (updatedEvent == null) return;

            Event existingEvent = _eventRepo.GetById(eventId);
            if (existingEvent != null)
            {
                existingEvent.Title = updatedEvent.Title;
                existingEvent.EventDate = updatedEvent.EventDate;
                existingEvent.Location = updatedEvent.Location;

                _eventRepo.Update(existingEvent);
            }
        }

        public void SendReminderToParticipants(int eventId, string reminderType)
        {
            GetParticipantsForEvent(eventId)
                .Where(p => !string.IsNullOrEmpty(p.Email))
                .ToList()
                .ForEach(p => _mailService.Send(p.Email, $"Reminder: {reminderType}", "Please take action regarding the event."));
        }

        private void HandleParticipantAction(int eventId, string actionType)
        {
            _participantRepo.GetAll()
                .Where(p => !string.IsNullOrEmpty(p.Email))
                .ToList()
                .ForEach(p => _mailService.Send(p.Email, $"עדכון מנהל: {actionType}", $"בוצעה פעולה באירוע {eventId}: {actionType}"));
        }

        private void HandleEventAction(int eventId, string actionType)
        {
            _participantRepo.GetAll()
                .Where(p => !string.IsNullOrEmpty(p.Email))
                .ToList()
                .ForEach(p => _mailService.Send(p.Email, $"עדכון אירוע: {actionType}", $"חל שינוי באירוע {eventId}: {actionType}"));
        }
    }
}   