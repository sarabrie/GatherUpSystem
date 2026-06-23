using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.Interfaces;
using GatherUp.Core.DO;
using GatherUp.Core.DO.Users;
using GatherUp.Core.DO.Polls;

using GatherUp.Core.Enums;

namespace GatherUp.BL.Services
{
    public class EventManagerService
    {
        private readonly IRepository<Event> _eventRepo;
        private readonly IRepository<Participant> _participantRepo;
        private readonly IRepository<Person> _personRepo;
        private readonly IMailService _mailService;

        public EventManagerService(
            IRepository<Event> eventRepo,
            IRepository<Participant> participantRepo,
            IRepository<Person> personRepo,
            IMailNotificationBridge notificationBridge, 
            IMailService mailService)
        {
            _eventRepo = eventRepo;
            _participantRepo = participantRepo;
            _personRepo = personRepo;
            _mailService = mailService;

            notificationBridge.OnParticipantAction += HandleParticipantAction;
            notificationBridge.OnEventAction += HandleEventAction;
            notificationBridge.OnNewPoll += HandleNewPoll;
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

            Person registeredUser = _personRepo.GetAll().FirstOrDefault(p => p.Email == participant.Email);
            if (registeredUser == null)
                throw new InvalidOperationException($"משתמש עם מייל {participant.Email} אינו רשום במערכת.");

            Event ev = _eventRepo.GetById(eventId);
            if (ev == null) throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");

            if (ev.ParticipantIds != null && ev.ParticipantIds.Contains(registeredUser.Id))
                throw new InvalidOperationException("המשתמש כבר משתתף באירוע זה.");

            participant.Id = registeredUser.Id;
            participant.Name = registeredUser.Name;

            if (!_participantRepo.GetAll().Any(p => p.Id == participant.Id))
                _participantRepo.Add(participant);

            if (ev.ParticipantIds == null) ev.ParticipantIds = new List<int>();
            ev.ParticipantIds.Add(participant.Id);
            _eventRepo.Update(ev);
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

        public void UpdateParticipantAttendance(int eventId, int participantId, bool isAttending)
        {
            Event ev = _eventRepo.GetById(eventId);
            if (ev == null) throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");
            if (ev.ParticipantIds == null || !ev.ParticipantIds.Contains(participantId))
                throw new InvalidOperationException("המשתמש אינו משתתף באירוע זה.");

            Participant participant = _participantRepo.GetById(participantId);
            if (participant == null) throw new KeyNotFoundException($"משתתף {participantId} לא נמצא.");

            participant.IsAttending = isAttending;
            _participantRepo.Update(participant);
        }

        public void UpdateParticipantNotifications(int eventId, int participantId, int notificationSettings)
        {
            Event ev = _eventRepo.GetById(eventId);
            if (ev == null) throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");
            if (ev.ParticipantIds == null || !ev.ParticipantIds.Contains(participantId))
                throw new InvalidOperationException("המשתמש אינו משתתף באירוע זה.");

            Participant participant = _participantRepo.GetById(participantId);
            if (participant == null) throw new KeyNotFoundException($"משתתף {participantId} לא נמצא.");

            participant.NotificationSettings = (NotificationPreferences)notificationSettings;
            _participantRepo.Update(participant);
        }

        public void UpdateParticipantPayment(int eventId, int participantId, bool hasPaid)
        {
            Event ev = _eventRepo.GetById(eventId);
            if (ev == null) throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");
            if (ev.ParticipantIds == null || !ev.ParticipantIds.Contains(participantId))
                throw new InvalidOperationException("המשתמש אינו משתתף באירוע זה.");

            Participant participant = _participantRepo.GetById(participantId);
            if (participant == null) throw new KeyNotFoundException($"משתתף {participantId} לא נמצא.");

            participant.HasPaid = hasPaid;
            _participantRepo.Update(participant);
        }

        public void SendReminderToParticipants(int eventId, string reminderType)
        {
            GetParticipantsForEvent(eventId)
                .Where(p => !string.IsNullOrEmpty(p.Email))
                .ToList()
                .ForEach(p => _mailService.Send(p.Email, $"Reminder: {reminderType}", "Please take action regarding the event."));
        }

        private void HandleNewPoll(int eventId, string pollTitle)
        {
            _participantRepo.GetAll()
                .Where(p => !string.IsNullOrEmpty(p.Email)
                    && p.NotificationSettings.HasFlag(NotificationPreferences.NewPolls))
                .ToList()
                .ForEach(p => _mailService.Send(p.Email, $"סקר חדש: {pollTitle}", $"נפתח סקר חדש באירוע {eventId}: {pollTitle}"));
        }

        private void HandleParticipantAction(int eventId, string actionType)
        {
            _participantRepo.GetAll()
                .Where(p => !string.IsNullOrEmpty(p.Email)
                    && p.NotificationSettings.HasFlag(NotificationPreferences.AdminMessages))
                .ToList()
                .ForEach(p => _mailService.Send(p.Email, $"עדכון מנהל: {actionType}", $"בוצעה פעולה באירוע {eventId}: {actionType}"));
        }

        private void HandleEventAction(int eventId, string actionType)
        {
            _participantRepo.GetAll()
                .Where(p => !string.IsNullOrEmpty(p.Email)
                    && p.NotificationSettings.HasFlag(NotificationPreferences.EventChanges))
                .ToList()
                .ForEach(p => _mailService.Send(p.Email, $"עדכון אירוע: {actionType}", $"חל שינוי באירוע {eventId}: {actionType}"));
        }
    }
}   