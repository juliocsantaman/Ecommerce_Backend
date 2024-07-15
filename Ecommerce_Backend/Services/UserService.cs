using Ecommerce_Backend.Models;

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
    }

    public interface IUserService
    {
        Task<User> Save(User user);
    }
}
