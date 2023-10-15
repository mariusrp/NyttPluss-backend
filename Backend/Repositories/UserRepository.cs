using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public class UserRepository
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;  // Added UserManager dependency
        private readonly MongoDbContext _context;

        public UserRepository(SignInManager<User> signInManager, UserManager<User> userManager, MongoDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;  // Initialized UserManager
            _context = context;
        }

        public async Task<User> FindUserByIdAsync(string userId)
        {
            var collection = _context.Database.GetCollection<User>("Users");
            var user = await collection.Find(u => u.UserId == userId).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> LoginOrCreateUserAsync(User user)
        {
            var existingUser = await FindUserByIdAsync(user.UserId);
            if (existingUser == null)
            {
                var collection = _context.Database.GetCollection<User>("Users");
                await collection.InsertOneAsync(user);
                return user;  // Return the new user
            }
            else
            {
                return existingUser;  // Return the existing user
            }
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        {
            return await _signInManager.GetExternalLoginInfoAsync();
        }

        public async Task<User> HandleGoogleLoginAsync(ExternalLoginInfo info)
        {
            // ... handle Google login and return User ...
            return null;  // Placeholder return statement, replace with your actual logic
        }
    }
}
