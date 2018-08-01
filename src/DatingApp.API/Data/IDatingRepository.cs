namespace DatingApp.API.Data
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using DatingApp.API.Models;
    using DatingApp.API.Helpers;

    public interface IDatingRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAll();
         Task<PagedList<User>> GetUsers(UserParams UserParams);
         Task<User> GetUser(int id);
         Task<Photo> GetPhoto(int id);
         Task<Photo> GetMainPhoto(int userId);
         Task<Like> GetLike(int userId, int recipientId);
    }
}