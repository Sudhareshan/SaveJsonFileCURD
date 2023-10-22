using SaveJsonFileCURD.Models;

namespace SaveJsonFileCURD.Repository
{
    public interface IUserDataStore
    {
        List<UserWithDepartment> GetUsers();
        UserWithDepartment GetUserById(int userId);



        void CreateUser(User user);


        void UpdateUser(User user);
        void DeleteUser(int userId);
    }
}
