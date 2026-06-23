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

        public void AddUser(Person user)
        {
            if (GetUserByEmail(user.Email) != null)
                throw new InvalidOperationException($"משתמש עם המייל {user.Email} כבר קיים במערכת");

            _userRepo.Add(user);
        }

        public Person AuthenticateUser(string email, int password)
        {
            Person user = GetUserByEmail(email);

            if (user != null && user.Id == password)
            {
                return user;
            }

            return null;
        }
        public void UpdateUser(Person updatedUser)
        {
            Person existingUser = _userRepo.GetAll().FirstOrDefault(u => u.Id == updatedUser.Id);
            if (existingUser == null)
                throw new KeyNotFoundException($"משתמש עם מזהה {updatedUser.Id} לא נמצא במערכת.");

            if (existingUser.Email != updatedUser.Email && GetUserByEmail(updatedUser.Email) != null)
                throw new InvalidOperationException("המייל החדש כבר בשימוש על ידי משתמש אחר.");

            _userRepo.Update(updatedUser);
        }
    }
}