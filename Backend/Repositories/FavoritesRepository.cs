using Backend.Models;
using Backend.Data;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Linq;


namespace Backend.Repositories
{
    public class FavoritesRepository
    {
        private readonly MongoDbContext _dbContext;

        public FavoritesRepository(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddFavoriteAsync(string userId, RssItem favoriteItem)
        {
            var user = await GetUserByIdAsync(userId);
            user.FavoriteRssItems.Add(favoriteItem);
            await UpdateUserAsync(user);
        }

        public async Task RemoveFavoriteAsync(string userId, string rssItemId)
        {
            var user = await GetUserByIdAsync(userId);
            user.FavoriteRssItems.RemoveAll(item => item.Id == rssItemId);
            await UpdateUserAsync(user);
        }

        private async Task<User> GetUserByIdAsync(string userId)
        {
            return await _dbContext.Database.GetCollection<User>("Users").Find(u => u.UserId == userId).FirstOrDefaultAsync();
        }

        private async Task UpdateUserAsync(User user)
        {
            await _dbContext.Database.GetCollection<User>("Users").ReplaceOneAsync(u => u.UserId == user.UserId, user);
        }
    }
}
