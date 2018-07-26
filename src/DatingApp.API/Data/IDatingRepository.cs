namespace DatingApp.API.Data
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using DatingApp.API.Models;

    public interface IDatingRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAll();
         Task<IEnumerable<User>> GetUsers();
         Task<User> GetUser(int id);
    }
}