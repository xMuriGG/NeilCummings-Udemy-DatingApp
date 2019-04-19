using System;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _db;

        public AuthRepository(DataContext db)
        {
            _db = db;
        }
        public async Task<User> Login(string username, string password)
        {
            username=username.ToLower();
            var user = await _db.Users.Include(i=>i.Photos).Where(w => w.Username.ToLower() == username).FirstOrDefaultAsync();
            if (user == null) { return null; }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) { return null; }

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                byte[] hashForCheck = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return hashForCheck.SequenceEqual(passwordHash);
            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            username=username.ToLower();
            return await _db.Users.AnyAsync(a=>a.Username.ToLower()==username);  
        }
    }
}