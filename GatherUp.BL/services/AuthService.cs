using GatherUp.Core.DO.Users;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL.Services
{
    public class AuthService
    {
        private readonly IRepository<Person> _userRepo;

        public AuthService(IRepository<Person> userRepo)
        {
            _userRepo = userRepo;
        }

        // פונקציית עזר אחת ששולפת משתמש לפי מייל בלבד
        private Person GetUserByEmail(string email)
        {
            return _userRepo.GetAll().FirstOrDefault(u => u.Email == email);
        }

        // פעולת ההרשמה משתמשת בפונקציית העזר כדי לבדוק אם המייל תפוס
        public void AddUser(Person user)
        {
            if (GetUserByEmail(user.Email) != null)
                throw new InvalidOperationException($"משתמש עם המייל {user.Email} כבר קיים במערכת");

            _userRepo.Add(user);
        }

        // פעולת ההתחברות משתמשת באותה פונקציית עזר, ומוסיפה גם בדיקת סיסמה
        public Person AuthenticateUser(string email, int password)
        {
            Person user = GetUserByEmail(email);

            // אם המשתמש קיים והסיסמה שלו (תעודת הזהות) תואמת - נחזיר אותו
            if (user != null && user.Id == password)
            {
                return user;
            }

            // אחרת, ההתחברות נכשלה
            return null;
        }
        public void UpdateUser(Person updatedUser)
        {
            // בדיקת ולידציה 1: האם המשתמש בכלל קיים במערכת?
            Person existingUser = _userRepo.GetAll().FirstOrDefault(u => u.Id == updatedUser.Id);
            if (existingUser == null)
                throw new KeyNotFoundException($"משתמש עם מזהה {updatedUser.Id} לא נמצא במערכת.");

            // בדיקת ולידציה 2: אם הוא מנסה לשנות מייל, האם המייל החדש כבר תפוס?
            if (existingUser.Email != updatedUser.Email && GetUserByEmail(updatedUser.Email) != null)
                throw new InvalidOperationException("המייל החדש כבר בשימוש על ידי משתמש אחר.");

            // אם כל הבדיקות עברו בהצלחה, שולחים לעדכון בריפוזיטורי
            _userRepo.Update(updatedUser);
        }
    }
}