using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using MSSQLCRUD.Models;

namespace MSSQLCRUD.Controllers
{
    public class HomeController : Controller
    {
        private string connection = "Data Source=LAPTOP-NHNHOTRR; Initial Catalog=localb; User ID=sa;Password=password;";

        public ActionResult Login()
        {
            return View("Login");
        }
        public ActionResult Logout()
        {
            try
            {
                HttpContext.Session["eid"] = null;
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message.ToString();
            }
            return View("Login");
        }

        public ActionResult Index()
        {
            try
            {
                if (HttpContext.Session["eid"] != null)
                {
                    UserProfileInfoModel userprofile = getProfile(Session["eid"].ToString());
                    Session["fname"] = userprofile.FirstName;
                    Session["lname"] = userprofile.LastName;
                    UserListsModel userlist = getUsers();
                    return View("Index", userlist);
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message.ToString();
            }
            return View("Login");
        }

        [HttpPost]
        public ActionResult Index(UserInfoModel userData)
        {
            if (userData.UserID.Length > 0 && userData.UserPass.Length > 0)
            {
                using (SqlConnection connSelect = new SqlConnection(connection))
                {
                    connSelect.Open();
                    using (SqlCommand cmdSelect = connSelect.CreateCommand())
                    {
                        cmdSelect.CommandText = "SELECT * FROM [localb].[dbo].[mscrudgoal] "
                                              + "WHERE username = @username AND password = @password;";
                        cmdSelect.Parameters.AddWithValue("username", userData.UserID);
                        cmdSelect.Parameters.AddWithValue("password", userData.UserPass);

                        SqlDataReader rdr = cmdSelect.ExecuteReader();

                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            UserProfileInfoModel userprofile = getProfile(rdr["employeeID"].ToString());
                            rdr.Close();
                            connSelect.Close();
                            HttpContext.Session["eid"] = userprofile.EmployeeID;
                            if (userprofile.role)
                                return RedirectToAction("Index");
                            else
                                return RedirectToAction("employeeview", userprofile);
                        }
                        else
                        {
                            ViewBag.msg = "username or password is incorrect";
                        }
                        rdr.Close();
                    }
                    connSelect.Close();
                }
            }
            return View("Login");
        }

        public ActionResult Update(string employeeID)
        {
            UserProfileInfoModel usrprof = new UserProfileInfoModel();
            usrprof = getProfile(employeeID);
            return View(usrprof);
        }
        [HttpPost]
        public ActionResult Update(UserProfileInfoModel userData)
        {
            using (SqlConnection connInsert = new SqlConnection(connection))
            {
                connInsert.Open();
                using (SqlCommand cmdInsert = connInsert.CreateCommand())
                {
                    cmdInsert.CommandText = "UPDATE [localb].[dbo].[mscrudgoalProfile] "
                                          + "SET "
                                          + "firstname = @firstName, lastname = @lastName, middlename = @middleName, "
                                          + "age = @age, address = @address, position = @position, maritalstatus = @maritalstatus "
                                          + "WHERE employeeID = @eid; ";
                    cmdInsert.Parameters.AddWithValue("firstName", userData.FirstName);
                    cmdInsert.Parameters.AddWithValue("lastName", userData.LastName);
                    cmdInsert.Parameters.AddWithValue("middlename", userData.MiddleName);
                    cmdInsert.Parameters.AddWithValue("age", userData.Age);
                    cmdInsert.Parameters.AddWithValue("address", userData.Address);
                    cmdInsert.Parameters.AddWithValue("position", userData.position);
                    cmdInsert.Parameters.AddWithValue("maritalstatus", userData.maritalStatus);
                    cmdInsert.Parameters.AddWithValue("eid", userData.EmployeeID);

                    try
                    {
                        if (cmdInsert.ExecuteNonQuery() > 0)
                        {
                            userData = null;
                            ViewBag.msg = "Details updated succefully!!";
                        }
                        else
                        {
                            ViewBag.msg = "Sorry!, An error has occured please contact admin";
                        }
                    }
                    catch (SqlException err)
                    {
                        ViewBag.msg = err.Message;
                    }
                }
                connInsert.Close();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(String employeeid)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM mscrudgoal WHERE employeeID = " + employeeid + "; "
                                    + "DELETE FROM mscrudgoalProfile WHERE employeeID = " + employeeid;
                    cmd.ExecuteReader();
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Delete(UserInfoModel userData)
        {
            Boolean isToDelete = false;
            using (SqlConnection connSelect = new SqlConnection(connection))
            {
                connSelect.Open();
                using (SqlCommand cmdSelect = connSelect.CreateCommand())
                {

                    cmdSelect.CommandText = "SELECT username FROM [localb].[dbo].[mscrudgoal] "
                                          + "WHERE username = @username AND password = @password;";
                    cmdSelect.Parameters.AddWithValue("username", userData.UserID);
                    cmdSelect.Parameters.AddWithValue("password", userData.UserPass);

                    SqlDataReader rdr = cmdSelect.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        isToDelete = true;

                    }
                    else
                    {
                        ViewBag.msg = "Username or password is incorrect";
                        return View();
                    }
                }
                connSelect.Close();
            }

            if (isToDelete)
            {
                using (SqlConnection connInsert = new SqlConnection(connection))
                {
                    connInsert.Open();
                    SqlTransaction trans = connInsert.BeginTransaction(IsolationLevel.ReadCommitted);
                    using (SqlCommand cmdInsert = connInsert.CreateCommand())
                    {
                        cmdInsert.Transaction = trans;
                        cmdInsert.CommandText = "DELETE FROM [localb].[dbo].[mscrudgoal] "
                                              + "WHERE username = @username AND password = @password";
                        cmdInsert.Parameters.AddWithValue("username", userData.UserID);
                        cmdInsert.Parameters.AddWithValue("password", userData.UserPass);

                        try
                        {
                            if (cmdInsert.ExecuteNonQuery() > 0)
                            {
                                trans.Commit();
                                userData = null;
                                ViewBag.msg = "Account successfully deleted";
                            }
                            else
                            {
                                trans.Rollback();
                                ViewBag.msg = "Sorry!, An error has occured please contact admin";
                            }
                        }
                        catch (SqlException err)
                        {
                            ViewBag.msg = err.Message;
                        }
                    }
                    connInsert.Close();
                }
            }
            return View("Login");
        }

        public ActionResult Register()
        {
            try
            {
                if (HttpContext.Session["eid"] != null)
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message.ToString();
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult Register(UserProfileInfoModel userData)
        {
            String eid = RegisterLogin(userData);

            using (SqlConnection connInsert = new SqlConnection(connection))
            {
                connInsert.Open();
                SqlTransaction trans = connInsert.BeginTransaction(IsolationLevel.ReadCommitted);
                using (SqlCommand cmdInsert = connInsert.CreateCommand())
                {
                    cmdInsert.Transaction = trans;
                    cmdInsert.CommandText = " INSERT INTO [dbo].[mscrudgoalProfile]"
                                          + " ([firstname],[middlename],[lastname],[birthdate],[employeeID] "
                                          + " ,[position],[maritalstatus],[address],[age]) "
                                          + " VALUES "
                                          + "(@firstname, @middlename, @lastname, @birthdate,'" + eid
                                          + "',@position, @maritalstatus, @address, @age )";
                    cmdInsert.Parameters.AddWithValue("firstname", userData.FirstName);
                    cmdInsert.Parameters.AddWithValue("middlename", userData.MiddleName);
                    cmdInsert.Parameters.AddWithValue("lastname", userData.LastName);
                    cmdInsert.Parameters.AddWithValue("birthdate", userData.birthdate);
                    cmdInsert.Parameters.AddWithValue("position", userData.position);
                    cmdInsert.Parameters.AddWithValue("maritalstatus", userData.maritalStatus);
                    cmdInsert.Parameters.AddWithValue("address", userData.Address);
                    cmdInsert.Parameters.AddWithValue("age", userData.Age);

                    try
                    {
                        if (cmdInsert.ExecuteNonQuery() > 0)
                        {
                            trans.Commit();
                            userData = null;
                            ViewBag.msg = "Registered Succefully! You can now log in!";
                        }
                        else
                        {
                            trans.Rollback();
                            ViewBag.msg = "Sorry!, An error has occured please contact admin";
                        }
                    }
                    catch (SqlException err)
                    {
                        ViewBag.msg = err.Message;
                    }
                }
                connInsert.Close();
            }
            
            return RedirectToAction("Index");
        }

        public ActionResult ViewProfile(String employeeid)
        {
            if (employeeid == null || employeeid == "")
                return View("Login");
            else
            {
                UserProfileInfoModel usrprof = new UserProfileInfoModel();
                usrprof = getProfile(employeeid);
                return View(usrprof);
            }
        }
        public UserProfileInfoModel getProfile(string employeeID)
        {
            UserProfileInfoModel userprofile = new UserProfileInfoModel();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM [localb].[dbo].[mscrudgoalProfile] A WITH(NOLOCK) "
                                    + "JOIN [localb].[dbo].[mscrudgoal] B ON A.employeeID = B.employeeID "
                                    + "WHERE B.employeeID = " + employeeID;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        userprofile.FirstName = rdr["firstname"].ToString();
                        userprofile.MiddleName = rdr["middlename"].ToString();
                        userprofile.LastName = rdr["lastname"].ToString();
                        userprofile.birthdate = Convert.ToDateTime(rdr["birthdate"].ToString()).ToString("MMMM dd, yyyy");
                        try { userprofile.Age = Convert.ToInt32(rdr["age"].ToString()); }
                        catch (Exception) { userprofile.Age = 0; }
                        userprofile.position = rdr["position"].ToString();
                        userprofile.maritalStatus = rdr["maritalstatus"].ToString();
                        userprofile.Address = rdr["address"].ToString();
                        userprofile.RegisteredDate = Convert.ToDateTime(rdr["registDate"].ToString()).ToString("MMMM dd, yyyy");
                        userprofile.EmployeeID = rdr["employeeID"].ToString();
                        userprofile.role = (rdr["role"].ToString() == "1" ? true : false);
                        userprofile.RegisteredDate = Convert.ToDateTime(rdr["registDate"].ToString()).ToString("MMMM dd, yyyy");
                        rdr.Close();
                    }
                    else
                    {
                        userprofile.FirstName = "null";
                    }
                }
                return userprofile;
            }
        }
        public UserListsModel getUsers()
        {
            UserListsModel userlist = new UserListsModel();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM [localb].[dbo].[mscrudgoalProfile] A WITH(NOLOCK) "
                                    + "JOIN [localb].[dbo].[mscrudgoal] B ON A.employeeID = B.employeeID ";
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            User users = new User();
                            users.employeeID = rdr["employeeID"].ToString();
                            users.fullname = rdr["firstname"].ToString() + ' ' + rdr["middlename"].ToString() + ' ' + rdr["lastname"].ToString();
                            users.registrationDate = Convert.ToDateTime(rdr["registDate"].ToString()).ToString("MMMM dd, yyyy");
                            userlist.Lists.Add(users);
                        }
                    }
                    else
                    {
                        ViewBag.msg = "No Employee's Registered";
                    }
                }
            
            }
            return userlist;
        }
        public String CleanString(string stringdata)
        {
            return stringdata.Replace(" ", "");
        }
        public String RegisterLogin(UserProfileInfoModel userinfo)
        {
            string eid;
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT CONCAT(YEAR(GETDATE()), FORMAT(ISNULL((SELECT MAX(ID) FROM mscrudgoal), 0) + 1, '00000')) AS EmployeeID;";
                    SqlDataReader rdr = cmd.ExecuteReader();    
                    rdr.Read();
                    eid = rdr["EmployeeID"].ToString();
                    rdr.Close();
                    cmd.CommandText = " INSERT INTO mscrudgoal ( username, [password], registDate, employeeID, [role] ) "
                                    + " VALUES ( @username, @password, "
                                    + " GETDATE(), "
                                    + " CONCAT(YEAR(GETDATE()), FORMAT(ISNULL((SELECT MAX(ID) FROM mscrudgoal), 0) + 1, '00000')), "
                                    + " 0 )";
                    cmd.Parameters.AddWithValue("username", userinfo.LastName.Substring(0,4) + eid);
                    cmd.Parameters.AddWithValue("password", userinfo.LastName.Substring(0, 4) + eid);
                    cmd.ExecuteNonQuery();
                }
            }

            return eid;
        }
        public ActionResult employeeView(UserProfileInfoModel userprofile)
        {
            return View(userprofile);
        }
        public ActionResult employeeUpdateProfile(String EmployeeID)
        {
            UserProfileInfoModel usrprof = new UserProfileInfoModel();
            usrprof = getProfile(EmployeeID);
            return View(usrprof);
        }

        [HttpPost]
        public ActionResult employeeUpdateProfile(UserProfileInfoModel userData)
        {
            using (SqlConnection connInsert = new SqlConnection(connection))
            {
                connInsert.Open();
                using (SqlCommand cmdInsert = connInsert.CreateCommand())
                {
                    cmdInsert.CommandText = "UPDATE [localb].[dbo].[mscrudgoalProfile] "
                                          + "SET "
                                          + "firstname = @firstName, lastname = @lastName, middlename = @middleName, "
                                          + "age = @age, address = @address, position = @position, maritalstatus = @maritalstatus "
                                          + "WHERE employeeID = @eid; ";
                    cmdInsert.Parameters.AddWithValue("firstName", userData.FirstName);
                    cmdInsert.Parameters.AddWithValue("lastName", userData.LastName);
                    cmdInsert.Parameters.AddWithValue("middlename", userData.MiddleName);
                    cmdInsert.Parameters.AddWithValue("age", userData.Age);
                    cmdInsert.Parameters.AddWithValue("address", userData.Address);
                    cmdInsert.Parameters.AddWithValue("position", userData.position);
                    cmdInsert.Parameters.AddWithValue("maritalstatus", userData.maritalStatus);
                    cmdInsert.Parameters.AddWithValue("eid", userData.EmployeeID);

                    try
                    {
                        if (cmdInsert.ExecuteNonQuery() > 0)
                        {
                            ViewBag.msg = "Details updated succefully!!";
                        }
                        else
                        {
                            ViewBag.msg = "Sorry!, An error has occured please contact admin";
                        }
                    }
                    catch (SqlException err)
                    {
                        ViewBag.msg = err.Message;
                    }
                }
                connInsert.Close();
            }
            return RedirectToAction("employeeView", userData);
        }
    }
}