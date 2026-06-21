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

        public bool UserExists(string email)
        {
            return _userRepo.GetAll().Any(u => u.Email == email);
        }

        public void AddUser(Person user)
        {
            if (UserExists(user.Email))
                throw new InvalidOperationException($"משתמש עם המייל {user.Email} כבר קיים במערכת");

            _userRepo.Add(user);
        }
    }
}
