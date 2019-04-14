using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _db;

        public DatingRepository(DataContext db)
        {
            _db = db;
        }

        public void Add<T>(T entity) where T : class
        {
            _db.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _db.Remove(entity);
        }

        public async Task<User> GetUserAsync(int id)
        {
            var user = await _db.Users.Include(i => i.Photos).FirstOrDefaultAsync(f => f.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = await _db.Users.Include(i => i.Photos).ToListAsync();
            return users;
        }

        public async Task<bool> SaveAll()
        {
            try
            {
                return await _db.SaveChangesAsync() > 0; // u ovoj funkciji samo ova linija treba ostalo je experiment
            }
            catch (DbUpdateException e)
            {
                throw new System.Exception("Greška prilikom spremanja podataka u ", e);
            }
        }
    }
}