using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GatherUp.Core.DO;
using GatherUp.Core.DO.Users;
using GatherUp.Core.DO.Finance;
using GatherUp.Core.DO.Polls;
using GatherUp.Core.Interfaces;
using GatherUp.Infrastructure.Data;
using GatherUp.Infrastructure.Mail;
using GatherUp.BL.Services;

namespace GatherUp.Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== GatherUp | סימולציית זרימת נתונים מקצה לקצה ===\n");

            IRepository<Event> eventRepo = new XmlRepository<Event>();
            IRepository<Participant> participantRepo = new XmlRepository<Participant>();
            IRepository<VendorAllocation> vendorRepo = new XmlRepository<VendorAllocation>();
            IRepository<ReceiptDetails> receiptRepo = new ReceiptRepository();
            IRepository<Poll> pollRepo = new XmlRepository<Poll>();

            IMailService mailService = new FileMailService();
            SimpleNotificationBridge notificationBridge = new SimpleNotificationBridge();

            EventManagerService eventManagerService = new EventManagerService(
                eventRepo, participantRepo, notificationBridge, mailService);

            FinanceService financeService = new FinanceService(
                eventRepo, participantRepo, vendorRepo, receiptRepo);

            PollService pollService = new PollService(eventRepo, pollRepo);

            EventsService eventsService = new EventsService(eventRepo, participantRepo, pollRepo, vendorRepo);

            Console.WriteLine("[מסך: יצירת אירוע] לחיצה על 'שמור אירוע'...");

            Event newEvent = new Event
            {
                Id = 1,
                Title = "חתונת כהן-לוי",
                EventDate = new DateTime(2026, 8, 15),
                Location = "אולמי נוה שלום, תל אביב",
                EventManagerId = 10,
                EventHostId = 20
            };
            eventRepo.Add(newEvent);
            Console.WriteLine($"[✓] אירוע נשמר: '{newEvent.Title}' בתאריך {newEvent.EventDate:dd/MM/yyyy}\n");

            Console.WriteLine("[מסך: ניהול משתתפים] לחיצה על 'הוסף משתתף'...");

            Participant p1 = new Participant { Id = 101, Name = "יוסי לוי", Email = "yossi@test.com", IsAttending = true, HasPaid = true, AmountContributed = 500m };
            Participant p2 = new Participant { Id = 102, Name = "דינה כהן", Email = "dina@test.com", IsAttending = true, HasPaid = false, AmountContributed = 0m };

            eventManagerService.AddParticipantToEvent(1, p1);
            eventManagerService.AddParticipantToEvent(1, p2);
            Console.WriteLine("[✓] שני משתתפים נוספו לאירוע ונשמרו ב-XML\n");

            Console.WriteLine("[מסך: עריכת אירוע] שינוי מיקום ולחיצה על 'שמור שינויים'...");

            Event updatedEvent = new Event
            {
                Id = 1,
                Title = "חתונת כהן-לוי",
                EventDate = new DateTime(2026, 8, 15),
                Location = "גן האירועים רמת גן - עודכן!",
                EventManagerId = 10,
                EventHostId = 20
            };
            eventManagerService.UpdateEventDetails(1, updatedEvent);

            Event verifyEvent = eventRepo.GetById(1);
            Console.WriteLine($"[✓] מיקום מעודכן ב-XML: '{verifyEvent.Location}'\n");

            VendorAllocation vendor = new VendorAllocation
            {
                Id = 201,
                Name = "קייטרינג דלוקס",
                AmountOwed = 8500m,
                HasReceipt = true,
                ReceiptIds = new List<int> { 301 }
            };
            financeService.AddVendorToEvent(1, vendor);
            Console.WriteLine($"[✓] ספק '{vendor.Name}' נוסף לאירוע\n");

            string dummyInvoicePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "invoice_sample.txt");
            File.WriteAllText(dummyInvoicePath, "Fake invoice content for testing.");

            ReceiptDetails receipt = new ReceiptDetails
            {
                Id = 301,
                ReceiptNumber = "INV-2026-001",
                ReceiptFilePath = dummyInvoicePath,
                Amount = 8500m,
                Date = DateTime.Now
            };
            receiptRepo.Add(receipt);

            ReceiptDetails verifyReceipt = receiptRepo.GetById(301);
            Console.WriteLine($"[✓] קבלה נשמרה. קובץ פיזי נמצא ב: {verifyReceipt?.ReceiptFilePath}\n");

            Poll poll = new Poll
            {
                Id = 401,
                Title = "בחירת תפריט",
                Description = "סקר לבחירת מנות לאירוע",
                Questions = new List<PollQuestion>
                {
                    new PollQuestion
                    {
                        QuestionId = 1,
                        QuestionText = "מה מנה עיקרית מועדפת?",
                        Options = new List<string> { "בשרי", "דגים", "טבעוני" }
                    }
                }
            };
            pollRepo.Add(poll);

            Event ev = eventRepo.GetById(1);
            ev.PollIds.Add(401);
            eventRepo.Update(ev);

            bool isActive = pollService.IsPollValidAndActive(401);
            Console.WriteLine($"[✓] הסקר פורסם ופעיל: {isActive}\n");

            eventManagerService.SendReminderToParticipants(1, "אישור הגעה לאירוע");
            Console.WriteLine("[✓] תזכורות נשלחו (נשמרו ב-mail_log.txt)\n");

            decimal balance = financeService.CalculateEventFinancialStatus(1);
            Console.WriteLine($"[✓] יתרה פיננסית לאירוע: {balance} ש\"ח\n");

            IEnumerable<Participant> allParticipants = eventManagerService.GetParticipantsForEvent(1);
            Console.WriteLine($"[✓] משתתפים באירוע 1: {allParticipants.Count()} משתתפים");
            allParticipants.ToList().ForEach(p => Console.WriteLine($"    - {p.Name} | שילם: {(p.HasPaid ? "כן" : "לא")}"));

            IEnumerable<Poll> eventPolls = pollService.GetEventPolls(1);
            Console.WriteLine($"[✓] סקרים פעילים לאירוע: {eventPolls.Count()}");

            Console.WriteLine("\n=== הסימולציה הושלמה בהצלחה ===");
            Console.ReadLine();
        }
    }
}
