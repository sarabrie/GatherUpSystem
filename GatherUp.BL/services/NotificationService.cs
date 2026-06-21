using System;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL.Services
{
    public class NotificationService
    {
        private readonly IMailNotificationBridge _mailBridge;

        // הזרקת הגשר בבנאי
        public NotificationService(IMailNotificationBridge mailBridge)
        {
            _mailBridge = mailBridge;
        }

        // הנה המתודה המדויקת ש-Amazon Q אומרת שחסרה לך בפרויקט!
        public void SendNotification(int eventId, string subject, string content, string targetAudience)
        {
            // שרשור הנושא והתוכן למחרוזת אחת עבור ה-Bridge
            string fullMessageDetails = $"Subject: {subject} | Content: {content}";

            if (targetAudience == "כל המשתתפים")
            {
                // הפעלת אירוע גלובלי של האירוע
                _mailBridge.TriggerEventAction(eventId, fullMessageDetails);
            }
            else if (targetAudience == "רק בעל האירוע")
            {
                // הפעלת אירוע ספציפי מול בעל האירוע
                _mailBridge.TriggerParticipantAction(eventId, fullMessageDetails);
            }
        }
    }
}