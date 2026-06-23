using System;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL.Services
{
    public class NotificationService
    {
        private readonly IMailNotificationBridge _mailBridge;

        public NotificationService(IMailNotificationBridge mailBridge)
        {
            _mailBridge = mailBridge;
        }

        public void SendNotification(int eventId, string subject, string content, string targetAudience)
        {
            string fullMessageDetails = $"Subject: {subject} | Content: {content}";

            if (targetAudience == "כל המשתתפים")
            {
                _mailBridge.TriggerEventAction(eventId, fullMessageDetails);
            }
            else if (targetAudience == "רק בעל האירוע")
            {
                _mailBridge.TriggerParticipantAction(eventId, fullMessageDetails);
            }
        }
    }
}