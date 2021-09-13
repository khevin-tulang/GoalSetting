using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSQLCRUD.Models
{
    public class UserViewModel
    {
        //    public UserInfoModel userInfo { get; set; }
        //    public UserProfileInfoModel userProfileInfo { get; set; }
        //public UserViewModel()
        //{
        //    userInfo = new UserInfoModel();
        //    userProfileInfo = new UserProfileInfoModel();
        //}
    }


    public class UserInfoModel
    {
        public string UserID { get; set; }
        public string UserPass { get; set; }
        public string ConfPass { get; set; }
        public string RegisteredDate { get; set; }
        public string EmployeeID { get; set; }
        public string role { get; set; }
    }

    public class UserProfileInfoModel 
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string birthdate { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string maritalStatus { get; set; }
        public string Address { get; set; }
        public string position { get; set; }
        public string EmployeeID { get; set; }
        public string RegisteredDate { get; set; }
        public Boolean role { get; set; }
    }
    public class UserListsModel
    {
        public UserListsModel()
        {
            List<User> UserList = new List<User>();
            this.Lists = UserList;
        }
        public List<User> Lists { get; set; }
    }
    public class User
    {
        public string fullname { get; set; }
        public string employeeID { get; set; }
        public string registrationDate { get; set; }
    }
}