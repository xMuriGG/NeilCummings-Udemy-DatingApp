using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly DataContext _db;

        public Seed(DataContext db)
        {
            _db = db;
        }
        public void SeedUsers()
        {
            if (_db.Users.Any()){return;}

            var userData=System.IO.File.ReadAllText("Data/UserSeedData.json");
            var jsonUserData= JsonConvert.DeserializeObject<List<User>>(userData);

            foreach (var user in jsonUserData)
            {
                byte[] passwordHash, passwordSalt;
                //polje password ručno prosljeđujemo jer je u UsersSeedData.json generisano polje password koje se neće deserializirati u klasu User
                //TO DO: probati podatke deserializirati u object pa iz njega izvući password
                CreatePasswordHash(user.Username=="Muris"?"muri":"test",out passwordHash,out passwordSalt);

                user.PasswordHash=passwordHash;
                user.PasswordSalt=passwordSalt;

                _db.Users.Add(user);
            }
             _db.SaveChanges(); //nećemo staviti async zato što nema potrebe ovo samo jednom izvršavamo 
        }


        //ovo je kopija metode iz Data/AuthRepository jer ova klasa ne ide u produkciju, a tamo ne želimo staviti public ili static na klasu
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }


    }
}