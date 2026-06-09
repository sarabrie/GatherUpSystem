using System;
using System.Collections.Generic;
using GatherUp.Core.Interfaces;
using GatherUp.Core.DO;
using GatherUp.Core.DO.Users;
using GatherUp.Core.DO.Finance;
using GatherUp.Core.DO.Polls;



    namespace GatherUp.Infrastructure.Data
    {
        public static class DataInitializer
        {
            public static void Initialize(
                IRepository<EventManager> managerRepo,
                IRepository<EventHost> hostRepo,
                IRepository<Participant> participantRepo,
                IRepository<VendorAllocation> vendorRepo,
                IRepository<Poll> pollRepo,
                IRepository<Event> eventRepo)
            {
                var manager = new EventManager { Name = "אילה לוי", Email = "ayala99263@gmail.com" };
                managerRepo.Add(manager);

                var host = new EventHost { Name = "שרה ברייש", Email = "moshe.host@gmail.com" };
                hostRepo.Add(host);

                var participant1 = new Participant
                {
                    Name = "ישראל ישראלי",
                    Email = "israel.test@gmail.com",
                    IsAttending = true,
                    HasPaid = true,
                    AmountContributed = 250.00m
                };
                participant1.MailingPreferences.Add(MailingPreference.Email);
                participantRepo.Add(participant1);

                var participant2 = new Participant
                {
                    Name = "רחל אברהם",
                    Email = "rachel.test@gmail.com",
                    IsAttending = null,
                    HasPaid = false,
                    AmountContributed = 0m
                };
                participant2.MailingPreferences.Add(MailingPreference.Sms);
                participantRepo.Add(participant2);

                var vendor = new VendorAllocation
                {
                    Name = "קייטרינג אסאדו הגורמה",
                    AmountOwed = 15000.00m,
                    HasReceipt = true
                };
                vendor.Receipts.Add(new ReceiptDetails { ReceiptNumber="12344", Amount = 15000.00m, Date = DateTime.Now.AddDays(-10)});
                vendorRepo.Add(vendor);


                var initialPoll = new Poll { Title = "פרטים התחלתיים", Description = "הצבעה על תאריך ומיקום מועדף לאירוע" };
                var q1 = new PollQuestion { QuestionText = "איזה מיקום מועדף עליך?" };
                q1.Options.AddRange(new[] { "תל אביב", "ירושלים", "חיפה" });
                var q2 = new PollQuestion { QuestionText = "איזה חודש הכי נוח לך?" };
                q2.Options.AddRange(new[] { "יוני", "יולי", "אוגוסט" });
                initialPoll.Questions.Add(q1);
                initialPoll.Questions.Add(q2);
                pollRepo.Add(initialPoll);

                var followupPoll = new Poll { Title = "סקר המשך - קולינריה", Description = "בחירת מנות מועדפות" };
                var q3 = new PollQuestion { QuestionText = "איזו מנה עיקרית תעדיף?" };
                q3.Options.AddRange(new[] { "בשרי", "צמחוני", "טבעוני" });
                followupPoll.Questions.Add(q3);
                pollRepo.Add(followupPoll);

                var mainEvent = new Event
                {
                    Title = "אירוע חברה שנתי GatherUp",
                    EventDate = DateTime.Now.AddMonths(2),
                    Location = "אולם גני האירועים",
                    EventManagerId = manager.Id,
                    EventHostId = host.Id
                };

                mainEvent.ParticipantIds.Add(participant1.Id);
                mainEvent.ParticipantIds.Add(participant2.Id);
                mainEvent.VendorIds.Add(vendor.Id);
                mainEvent.PollIds.Add(initialPoll.Id);
                mainEvent.PollIds.Add(followupPoll.Id);

                eventRepo.Add(mainEvent);
            }
        }
    }
