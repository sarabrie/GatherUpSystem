using System;
using GatherUp.Core.Interfaces; // שימוש באינטרפייס הגנרי
using GatherUp.Core.DO;        // שימוש בישויות שלך בהתאם ל-Namespaces הקיימים
using GatherUp.Core.DO.Users;
using GatherUp.Core.DO.Finance;
using GatherUp.Core.DO.Polls;
using GatherUp.Infrastructure; // גישה למחלקת ה-DataInitializer שלך
using GatherUp.Infrastructure.Data.Memory;
using GatherUp.Infrastructure.Data.GatherUp.Infrastructure.Data;
namespace GatherUp.Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== תחילת הרצת בדיקות מערכת GatherUp ===");
            Console.WriteLine("---------------------------------------");

            // 1. יצירת 6 ה-Repositories בדיוק כפי שנדרש בחתימת הפונקציה שלך
            IRepository<EventManager> managerRepo = new MemoryRepository<EventManager>();
            IRepository<EventHost> hostRepo = new MemoryRepository<EventHost>();
            IRepository<Participant> participantRepo = new MemoryRepository<Participant>();
            IRepository<VendorAllocation> vendorRepo = new MemoryRepository<VendorAllocation>();
            IRepository<Poll> pollRepo = new MemoryRepository<Poll>();
            IRepository<Event> eventRepo = new MemoryRepository<Event>();

            // 2. קריאה למחלקת האיתחול הקיימת אצלך והזרקת כל ה-Repositories בסדר הנכון
            Console.WriteLine("[מערכת]: מריץ איתחול נתונים מתוך DataInitializer...");
            DataInitializer.Initialize(managerRepo, hostRepo, participantRepo, vendorRepo, pollRepo, eventRepo);
            Console.WriteLine("[מערכת]: האיתחול הסתיים בהצלחה.\n");


            // 4. דרישת שלב 5: הוספת 3 משתתפים חדשים למערכת
            Console.WriteLine("\n[פעולה]: מוסיף 3 משתתפים חדשים לפי דרישות שלב 5...");

            var p1 = new Participant { Id = 101, Name = "מאיר כהן", Email = "meir@test.com", IsAttending = true, HasPaid = true, AmountContributed = 200m };
            var p2 = new Participant { Id = 102, Name = "חנה לוי", Email = "chana@test.com", IsAttending = null, HasPaid = false, AmountContributed = 0m };
            var p3 = new Participant { Id = 103, Name = "שמעון מזרחי", Email = "shimon@test.com", IsAttending = false, HasPaid = false, AmountContributed = 0m };

            participantRepo.Add(p1);
            participantRepo.Add(p2);
            participantRepo.Add(p3);

            // 5. דרישת שלב 5: שלפו את אחד המשתתפים לפי ה-Id שלו
            int searchId = 101;
            Console.WriteLine($"\n[פעולה]: שולף משתתף עם מזהה (ID): {searchId}...");
            Participant fetchedParticipant = participantRepo.GetById(searchId);

            if (fetchedParticipant != null)
            {
                Console.WriteLine($"[תוצאה]: נמצא משתתף! שם: {fetchedParticipant.Name}, מייל: {fetchedParticipant.Email}");
            }
            else
            {
                Console.WriteLine($"[שגיאה]: לא נמצא משתתף עם ID {searchId}");
            }

            // 6. דרישת שלב 5: הדפסת מסך של רשימת כל המשתתפים המעודכנת
            Console.WriteLine("\n--- רשימת כל המשתתפים במערכת כעת: ---");
            PrintAllParticipants(participantRepo);

            Console.WriteLine("\n=======================================");
            Console.WriteLine("=== סיום הרצת הבדיקה בהצלחה ===");
            Console.ReadLine(); // משאיר את חלון ה-Console פתוח
        }

        private static void PrintAllParticipants(IRepository<Participant> repo)
        {
            foreach (var participant in repo.GetAll())
            {
                Console.WriteLine($"- [ID: {participant.Id}] שם: {participant.Name} | מייל: {participant.Email} | סטטוס הגעה: {(participant.IsAttending == true ? "מאשר" : participant.IsAttending == false ? "לא מאשר" : "טרם השיב")}");
            }
        }
    }
}