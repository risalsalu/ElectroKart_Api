using ElectroKart_Api.Data;
using ElectroKart_Api.DTOs;
using ElectroKart_Api.Models;

namespace ElectroKart_Api.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _Context;
        public UserService(AppDbContext Context)
        {
            _Context = Context;
        }
        public User Register(RegisterDTO Dto)
        {
            var user = new User
            {
                Username = Dto.Username,
                Email = Dto.Email,
                Password = Dto.Password
            };
            _Context.Users.Add(user);
            _Context.SaveChanges();
            return user;


        }
        public User? Login(LoginDTO Dto)
        {
            return _Context.Users.FirstOrDefault(u => u.Email == Dto.Email && u.Password == Dto.Password);
        }
        public List<User> GetAllUsers()
        {
            return _Context.Users.ToList();
        }
    }
}
