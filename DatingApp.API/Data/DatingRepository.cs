using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            user.Photos = user.Photos?.OrderByDescending(ob => ob.IsMain).ToList();
            return user;
        }

        public Task<Photo> GetPhoto(int id)
        {
            var photo = _db.Photos.FirstOrDefaultAsync(f => f.Id == id);
            return photo;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            var photo = await _db.Photos.Where(w => w.UserId == userId).FirstOrDefaultAsync(w => w.IsMain);
            return photo;
        }

        public async Task<PagedList<User>> GetUsersAsync(UserParams userParams)
        {
            var query = _db.Users.Include(i => i.Photos).AsQueryable();
            query = query.Where(w => (w.Gender == userParams.Gender || userParams.Gender == null) && w.Id != userParams.UserId);
            DateTime? minDob = null;
            DateTime? maxDob = null;

            if (userParams.MaxAge is int max)
            {
                minDob = DateTime.Today.AddYears(-max - 1);
            }
            if (userParams.MinAge is int min)
            {
                maxDob = DateTime.Today.AddYears(-min);
            }
            query = query.Where(w => (w.DateOfBirth >= minDob || minDob == null) && (w.DateOfBirth <= maxDob || maxDob == null));
            // Još neki od slučajeva sa pattern matchingom
            //var minDob2 = userParams.MaxAge !=null ? DateTime.Today.AddYears((int)-userParams.MaxAge - 1) : (DateTime?)null;
            //var minDob3 = userParams.MaxAge is int maxx
            //    ? DateTime.Today.AddYears((int) -userParams.MaxAge - 1)
            //    : (DateTime?) null;

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        query = query.OrderBy(o => o.Created);break;
                    case "createdDesc":
                        query = query.OrderByDescending(o => o.Created);
                        break;
                    case "age":
                        query = query.OrderBy(o => o.DateOfBirth);
                        break;
                    case "ageDesc":
                        query = query.OrderByDescending(o => o.DateOfBirth);
                        break;
                    default: query = query.OrderByDescending(o => o.LastActive);
                        break;
                    
                }
            }

            var pagedList = await PagedList<User>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);

            return pagedList;
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