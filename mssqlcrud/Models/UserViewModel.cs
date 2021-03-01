using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSQLCRUD.Models
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            userInfo = new UserInfoModel();
        }

        public UserInfoModel userInfo { get; set; }
    }


    public class UserInfoModel
    {
        public string UserID { get; set; }

        public string UserPass { get; set; }
        public string ConfPass { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }
        
        public int Age { get; set; }
        
        public string Gender { get; set; }
        
        public string Address { get; set; }
    }
}