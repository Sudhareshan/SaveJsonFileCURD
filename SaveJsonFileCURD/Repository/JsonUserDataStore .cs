using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaveJsonFileCURD.Models;
using System.Linq;
using System.Net;
using System.Web.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SaveJsonFileCURD.Repository
{
    public class JsonUserDataStore : IUserDataStore
    {
        private List<User> _users;
        private List<Department> _departments;
        private List<UserWithDepartment> _userWithDepartments;
        private readonly string dataFilePath = "userData.json";
        private readonly IConfiguration _configuration;
        private JObject jsonObject;
        

        public JsonUserDataStore(IConfiguration configuration)
        {
            _configuration = configuration;
            LoadDataFromFile();
            _userWithDepartments = GetUsers();
        }

        private void LoadDataFromFile()
        {
            if (File.Exists(dataFilePath))
            {
                var json = File.ReadAllText(dataFilePath);

                jsonObject = JObject.Parse(json);
                JArray usersArray = (JArray)jsonObject["Users"];

                JArray departmentArray = (JArray)jsonObject["Departments"];
                _departments = departmentArray.ToObject<List<Department>>();
                //List<User> users = usersArray.ToObject<List<User>>();

                _users = usersArray.ToObject<List<User>>();

                //  _users = JsonConvert.DeserializeObject<List<User>>(json);
            }
            else
            {
                _users = new List<User>();
                _departments = new List<Department>();
            }
        }

        private void LoadDepartment()
        {
            if (File.Exists(dataFilePath))
            {
                var json=File.ReadAllText(dataFilePath);
                jsonObject = JObject.Parse(json);
                JArray departmentArray = (JArray)jsonObject["Departments"];
                _departments = departmentArray.ToObject<List<Department>>();
            }
        }

        private void SaveDataToFile()
        {
          //  var json = JsonConvert.SerializeObject(_users, Formatting.Indented);
            File.WriteAllText(dataFilePath, jsonObject.ToString());
        }




        public void CreateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            else
            {
                var userDepartmentId = user.DepartmentId;
                var departmentIds = _departments.Select(dept => dept.DepartmentId).ToList();

                if (departmentIds.Contains(userDepartmentId))
                {
                    string defaultPasscode = _configuration.GetSection("DefaultUserPassword").GetSection("Password").Value;
                    string val = _configuration.GetValue<string>("DefaultUserPassword:Password");
                    user.Password = defaultPasscode;

                    JArray usersArray = (JArray)jsonObject["Users"];

                    usersArray.Add(JObject.FromObject(user));

                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(user, Newtonsoft.Json.Formatting.Indented);
                    _users.Add(user);
                    SaveDataToFile();
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                

            }




        }

      



        public List<UserWithDepartment> GetUsers()
        {
            var userslist = _users;
            var departmentlist = _departments;

            var result = userslist
                .Join(departmentlist,
                    user => user.DepartmentId,
                    department => department.DepartmentId,
                    (user, department) => new UserWithDepartment
                    {
                        UserId = user.UserId,
                        Name = user.Name,
                        DepartmentName = department.DepartmentName
                    })
                .ToList();

            return result;
        }



        public  UserWithDepartment GetUserById(int userId)
        {
            var getUserById = _userWithDepartments.Find(x => x.UserId == userId);
           // var getUserById = _userWithDepartments.Where(x => x.UserId == userId).ToList();

            if (getUserById != null)
            {
                return getUserById;
            }
            else
            {
                // Handle the case when the user is not found, e.g., return a specific response or throw an exception.
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        public void DeleteUser(int userId)
        {
           
           

                JArray usersArray = (JArray)jsonObject["Users"];

                //usersArray.Remove(JObject.FromObject(userToRemove));
                JToken userToDelete = usersArray.FirstOrDefault(u => (int)u["UserId"] == userId);

                if (userToDelete != null)
                {
                    userToDelete.Remove();
                   // string updatedJson = jsonObject.ToString(Formatting.Indented);

                    SaveDataToFile();
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

        }

        public void UpdateUser(User user)
        {
            JArray usersArray = (JArray)jsonObject["Users"];
            JToken userToUpdate = usersArray.FirstOrDefault(u => (int)u["UserId"] == user.UserId);
            if (userToUpdate != null)
            {

                
                var departmentIds = _departments.Select(dept => dept.DepartmentId).ToList();
                if(departmentIds.Contains(user.DepartmentId))
                {
                    userToUpdate["DepartmentId"] = user.DepartmentId;
                    userToUpdate["Name"] = user.Name;
                    userToUpdate["Password"] = user.Password;

                    SaveDataToFile();
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
               

            }
        }
    }
}
