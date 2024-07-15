using Ecommerce_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Backend.Services
{
    public class UserService : IUserService
    {
        UserContext _context;

        public UserService(UserContext context)
        {
            _context = context;
        }

        public async Task<User> Save(User user)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserByEmailAndPassword(string email, string password)
        {
            return await _context.Users.Where(u => u.Email == email && u.Password == password).FirstOrDefaultAsync();
        }
    }

    public interface IUserService
    {
        Task<User> Save(User user);
        Task<User> GetUserByEmailAndPassword(string email, string password);
    }
}
