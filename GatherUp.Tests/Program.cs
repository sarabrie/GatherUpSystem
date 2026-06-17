using System;
using System.IO;
using GatherUp.Core.Interfaces;
using GatherUp.Core.DO;
using GatherUp.Core.DO.Users;
using GatherUp.Core.DO.Finance;
using GatherUp.Core.DO.Polls;
using GatherUp.BL.Services;

using GatherUp.Infrastructure.Data.Memory;
using GatherUp.Infrastructure.Data; // ודאי שזה ה-Namespace שבו נמצא ה-XmlRepository וה-ReceiptRepository שלך
namespace GatherUp.Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== תחילת הרצת בדיקות מערכת GatherUp ===");
            Console.WriteLine("---------------------------------------");

            // --- חלק א': הבדיקות הישנות מול ה-MemoryRepository (נשארות כפי שהיו) ---

            IRepository<EventManager> memManagerRepo = new MemoryRepository<EventManager>();
            IRepository<EventHost> memHostRepo = new MemoryRepository<EventHost>();
            IRepository<Participant> memParticipantRepo = new MemoryRepository<Participant>();
            IRepository<VendorAllocation> memVendorRepo = new MemoryRepository<VendorAllocation>();
            IRepository<Poll> memPollRepo = new MemoryRepository<Poll>();
            IRepository<Event> memEventRepo = new MemoryRepository<Event>();
            // רישום שירות שליחת המיילים לקובץ

            Console.WriteLine("[מערכת]: מריץ איתחול נתונים לזיכרון (Memory)...");
            DataInitializer.Initialize(memManagerRepo, memHostRepo, memParticipantRepo, memVendorRepo, memPollRepo, memEventRepo);

            // הרצת פונקציית הבדיקות של הזיכרון
            ExecuteMemoryRepositoryTests(memParticipantRepo);


            Console.WriteLine("\n--------------------------------------------------");
            Console.WriteLine("=== מעבר לבדיקות מול ה-XmlRepository ===");
            Console.WriteLine("--------------------------------------------------");


            // --- חלק ב': הדרישה החדשה - הרצת הבדיקות מול ה-XmlRepository ---

            // הגדרת הניתוב לתיקייה שבה תרצי שיווצרו כל קובצי ה-XML
            string xmlFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XmlDatabase");

            // יצירת 6 ה-XmlRepositories עבור הישויות השונות
            IRepository<EventManager> xmlManagerRepo = new XmlRepository<EventManager>();
            IRepository<EventHost> xmlHostRepo = new XmlRepository<EventHost>();
            IRepository<Participant> xmlParticipantRepo = new XmlRepository<Participant>();
            IRepository<VendorAllocation> xmlVendorRepo = new XmlRepository<VendorAllocation>();
            IRepository<Poll> xmlPollRepo = new XmlRepository<Poll>();
            IRepository<Event> xmlEventRepo = new XmlRepository<Event>();

            IRepository<ReceiptDetails> xmlReceiptRepo = new ReceiptRepository();

            // קריאה לפונקציית הבדיקה המקבילה החדשה והזרקת ה-Repositories של ה-XML
            ExecuteXmlRepositoryTests(xmlParticipantRepo, xmlPollRepo, xmlReceiptRepo, xmlFolderPath);
            Console.WriteLine("\n=======================================");
            Console.WriteLine("=== סיום הרצת כל הבדיקות בהצלחה ===");
            Console.ReadLine();
        }

        /// <summary>
        /// פונקציה מקבילה אשר מנהלת את כל ההתנהלות ובדיקת הנתונים מול ה-XmlRepository
        /// </summary>
        private static void ExecuteXmlRepositoryTests(
    IRepository<Participant> xmlParticipantRepo,
    IRepository<Poll> xmlPollRepo,
    IRepository<ReceiptDetails> xmlReceiptRepo,
    string folderPath)
        {
            Console.WriteLine($"\n[מערכת XML]: תחילת בדיקות דיסק בנתיב:\n -> {folderPath}");
            Console.WriteLine("--------------------------------------------------");

            // ==========================================================================
            // בדיקה 1: הוספת שלושה משתתפים נוספים (יוסי לוי, דוד כהן, יאיר מזרחי)
            // ==========================================================================
            Console.WriteLine("\n[פעולה 1]: מוסיף 3 משתתפים חדשים לקובץ ה-XML...");

            var p1 = new Participant { Id = 201, Name = "יוסי לוי", Email = "yossi@test.com", IsAttending = true, HasPaid = true, AmountContributed = 500m };
            var p2 = new Participant { Id = 202, Name = "דוד כהן", Email = "david@test.com", IsAttending = null, HasPaid = false, AmountContributed = 0m };
            var p3 = new Participant { Id = 203, Name = "יאיר מזרחי", Email = "yair@test.com", IsAttending = false, HasPaid = false, AmountContributed = 0m };

            xmlParticipantRepo.Add(p1);
            xmlParticipantRepo.Add(p2);
            xmlParticipantRepo.Add(p3);
            Console.WriteLine("[תוצאה]: המשתתפים נשמרו פיזית בקובץ Participant.xml.");


            // ==========================================================================
            // בדיקה 2: סקרים - הוספת שאלה לסקר ושינוי תשובה (אופציה) בסקר
            // ==========================================================================
            Console.WriteLine("\n[פעולה 2]: מריץ בדיקות על מערכת הסקרים (Polls) ב-XML...");

            // א. ניצור סקר חדש כדי שנוכל לעדכן אותו
            var testPoll = new Poll { Id = 1, Title = "סקר קולינריה חברה", Description = "בחירת אוכל לאירוע" };
            testPoll.Questions.Add(new PollQuestion { QuestionText = "האם תרצה מנה טבעונית?" }); // שאלה ראשונית
            xmlPollRepo.Add(testPoll);

            // ב. נשלוף את הסקר מהדיסק כדי לדמות עבודה אמיתית
            Poll pollFromDisk = xmlPollRepo.GetById(1);
            if (pollFromDisk != null)
            {
                // ג. הוספת שאלה חדשה לסקר הקיים
                Console.WriteLine("- מוסיף שאלה חדשה לסקר...");
                pollFromDisk.Questions.Add(new PollQuestion { QuestionText = "באיזו שעה להגיש את הקינוחים?" });

                // ד. שינוי תשובה/אופציה בתוך שאלה קיימת
                Console.WriteLine("- משנה/מוסיף אופציות תשובה בשאלה הראשונה...");
                if (pollFromDisk.Questions.Count > 0)
                {
                    pollFromDisk.Questions[0].Options.Clear();
                    pollFromDisk.Questions[0].Options.AddRange(new[] { "כן, בטח!", "לא, מעדיף בשרי", "לא משנה לי" });
                }

                // ה. שמירת השינויים חזרה לדיסק בעזרת Update
                xmlPollRepo.Update(pollFromDisk);
                Console.WriteLine("[תוצאה]: השינויים בסקר נשמרו בהצלחה בקובץ Poll.xml.");
            }


            // ==========================================================================
            // בדיקה 3: קבלות - העלאת קובץ פיזי והוספת ישות ל-XML
            // ==========================================================================
            Console.WriteLine("\n[פעולה 3]: בודק העלאת קבלה וקובץ פיזי...");

            // יצירת קובץ "דמה" זמני על שולחן העבודה כדי לבדוק את מנגנון ההעתקה שלכן
            // במקום pdf, ניצור קובץ טקסט אמיתי
            string dummyUserInvoicePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user_invoice_sample.txt");
            File.WriteAllText(dummyUserInvoicePath, "This is a dummy invoice file content for testing.");
            // בניית ישות הקבלה - בשדה ReceiptNumber אנחנו שמים את הניתוב של הקובץ המקורי שרוצים להעלות
            var newReceipt = new ReceiptDetails
            {
                Id = 555,
                ReceiptNumber = dummyUserInvoicePath, // הנתיב הפיזי של הקובץ שצריך להעתיק
                Amount = 1250.50m,
                Date = DateTime.Now
            };

            // הפעלת מתודת ה-Add שלכן (שמעתיקה את הקובץ לתיקיית ReceiptFiles ורושמת ב-XML)
            xmlReceiptRepo.Add(newReceipt);
            Console.WriteLine("[תוצאה]: הקבלה עובדה בהצלחה!");
            Console.WriteLine(" -> ודאו שנוצר קובץ חדש בתוך תיקיית: bin\\Debug\\...\\ReceiptFiles");
            Console.WriteLine(" -> ודאו שהנתיב החדש נרשם בתוך קובץ ה-XML: ReceiptDetails.xml");


            // ==========================================================================
            // הדפסת הרשימה המלאה של המשתתפים מה-XML (דרישת סעיף 2)
            // ==========================================================================
            Console.WriteLine("\n--- [הדפסה]: רשימת כל המשתתפים המעודכנת מתוך קובץ ה-XML: ---");
            var allParticipants = xmlParticipantRepo.GetAll();
            foreach (var p in allParticipants)
            {
                Console.WriteLine($"- [ID: {p.Id}] שם: {p.Name} | מייל: {p.Email} | שילם: {(p.HasPaid ? "כן" : "לא")} (סכום: {p.AmountContributed} ש\"ח)");
            }
        }

        /// <summary>
        /// פונקציית בדיקות הזיכרון המקורית (מהשלב הקודם)
        /// </summary>
        private static void ExecuteMemoryRepositoryTests(IRepository<Participant> participantRepo)
        {
            Console.WriteLine("\n[פעולה]: מוסיף 3 משתתפים חדשים לזיכרון (Memory)...");

            var p1 = new Participant { Id = 101, Name = "מאיר כהן", Email = "meir@test.com", IsAttending = true, HasPaid = true, AmountContributed = 200m };
            var p2 = new Participant { Id = 102, Name = "חנה לוי", Email = "chana@test.com", IsAttending = null, HasPaid = false, AmountContributed = 0m };
            var p3 = new Participant { Id = 103, Name = "שמעון מזרחי", Email = "shimon@test.com", IsAttending = false, HasPaid = false, AmountContributed = 0m };

            participantRepo.Add(p1);
            participantRepo.Add(p2);
            participantRepo.Add(p3);

            int searchId = 101;
            Participant fetchedParticipant = participantRepo.GetById(searchId);
            if (fetchedParticipant != null)
            {
                Console.WriteLine($"[תוצאה מהזיכרון]: נמצא משתתף! שם: {fetchedParticipant.Name}");
            }
        }
    }
}