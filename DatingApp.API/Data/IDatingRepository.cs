using System.Collections;
using System.Collections.Generic;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<PagedList<User>> GetUsersAsync(UserParams userParams);
        Task<User> GetUserAsync(int id);
        Task<bool> UserExistsAsync(int id);
        Task<Photo> GetPhotoAsync(int id);
        Task<Photo> GetMainPhotoForUserAsync(int userId);
        Task<Like> GetLikeAsync(int userId, int recipientId);
        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);



    }
}