using LapkaBackend.Domain.Entities;
using LapkaBackend.Infrastructure.Data;
using LapkaBackend.Infrastructure.Interfaces;
using Microsoft.VisualBasic;

namespace LapkaBackend.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _dbContext;

        public AuthService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }


        #region RegisterUser
        public User RegisterUser(Auth auth)
        {

            if (auth.Password != auth.ConfirmPassword)
            {
                return null;
            }

            var newUser = new User()
            {
                FirstName = auth.FirstName,
                LastName = auth.LastName,
                Email = auth.Email,
                Password = auth.Password,
                CreatedAt = DateTime.Now
            };

            _dbContext.Users.AddAsync(newUser);
            _dbContext.SaveChanges();

            return newUser;
        }
        #endregion

        #region LoginUser
        public string LoginUser(User user)
        {
            var result = _dbContext.Users.FirstOrDefault(x => x.Email == user.Email);

            if(result == null)
            {
                return "Taki użytkownik nie istnieje";
            }

            if(result.Password != user.Password)
            {
                return "Hasła się nie zgadzają";
            }
            //tu cała logika do JWT

            return "Logowanie się powiodło"; // tu ma być zwracany JWT
        }
        #endregion
    }
}
