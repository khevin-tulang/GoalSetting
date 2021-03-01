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

        public ActionResult Index()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Index(UserInfoModel userData)
        {
            if (userData.UserID.Length > 0 && userData.UserPass.Length > 0) {
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
                            userData.FirstName = rdr["firstName"].ToString();
                            ViewBag.FName = userData.FirstName;
                            userData.MiddleName = rdr["middleName"].ToString();
                            userData.LastName = rdr["lastName"].ToString();
                            try { userData.Age = Convert.ToInt32(rdr["age"].ToString()); }
                            catch (Exception) { userData.Age = 0; }
                            userData.Gender = rdr["gender"].ToString();
                            userData.Address = rdr["address"].ToString();
                            rdr.Close();
                            connSelect.Close();
                            return View("Index", userData);
                        }
                    }
                    connSelect.Close();
                }
            }
            ViewBag.msg = "username or password is incorrect";
            return View("Login");
        }

        [HttpPost]
        public ActionResult Update(UserInfoModel userData)
        {
            using (SqlConnection connInsert = new SqlConnection(connection))
            {
                connInsert.Open();
                SqlTransaction trans = connInsert.BeginTransaction(IsolationLevel.ReadCommitted);
                using (SqlCommand cmdInsert = connInsert.CreateCommand())
                {

                    cmdInsert.Transaction = trans;
                    cmdInsert.CommandText = "UPDATE [localb].[dbo].[mscrudgoal] "
                                          + "SET "
                                          + "firstName = @firstName, lastName = @lastName, middlename = @middleName, "
                                          + "age = @age, gender = @gender, address = @address "
                                          + "WHERE username = @username; ";
                    cmdInsert.Parameters.AddWithValue("username", userData.UserID);
                    cmdInsert.Parameters.AddWithValue("firstName", userData.FirstName);
                    cmdInsert.Parameters.AddWithValue("lastName", userData.LastName);
                    cmdInsert.Parameters.AddWithValue("middlename", userData.MiddleName);
                    cmdInsert.Parameters.AddWithValue("gender", userData.Gender);
                    cmdInsert.Parameters.AddWithValue("age", userData.Age);
                    cmdInsert.Parameters.AddWithValue("address", userData.Address);

                    try
                    {
                        if (cmdInsert.ExecuteNonQuery() > 0)
                        {
                            trans.Commit();
                            userData = null;
                            ViewBag.msg = "Details updated succefully!!";
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
            return View("Index", userData);
        }

        public ActionResult Delete()
        {
            return View();
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
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserInfoModel userData)
        {
            if (userData.UserPass != userData.ConfPass)
            {
                ViewBag.msg = "password and confirm password doesn't match";
                return View();
            }
            Boolean isToRegister = false;
            using (SqlConnection connSelect = new SqlConnection(connection)) 
            {
                connSelect.Open();
                using (SqlCommand cmdSelect = connSelect.CreateCommand())
                {

                    cmdSelect.CommandText = "SELECT username FROM [localb].[dbo].[mscrudgoal] "
                                          + "WHERE username = @username";
                    cmdSelect.Parameters.AddWithValue("username", userData.UserID);

                    SqlDataReader rdr = cmdSelect.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        ViewBag.msg = "Sorry!, this username has been taken, please try a different one";
                    }
                    else
                    {
                        isToRegister = true;
                    }
                }
                connSelect.Close();
            }

            if (isToRegister)
            {
                using (SqlConnection connInsert = new SqlConnection(connection))
                {
                    connInsert.Open();
                    SqlTransaction trans = connInsert.BeginTransaction(IsolationLevel.ReadCommitted);
                    using (SqlCommand cmdInsert = connInsert.CreateCommand())
                    {
                        cmdInsert.Transaction = trans;
                        cmdInsert.CommandText = "INSERT INTO [localb].[dbo].[mscrudgoal] "
                                              + "([username] ,[password] ,[firstName] ,[lastName] "
                                              + ",[middlename], [gender], [age], [address]) "
                                              + "VALUES "
                                              + "(@username, @password, @firstName, @lastName"
                                              + ",@middlename, @gender, @age, @address"
                                              + ")";
                        cmdInsert.Parameters.AddWithValue("username", userData.UserID);
                        cmdInsert.Parameters.AddWithValue("password", userData.UserPass);
                        cmdInsert.Parameters.AddWithValue("firstName", userData.FirstName);
                        cmdInsert.Parameters.AddWithValue("lastName", userData.LastName);
                        cmdInsert.Parameters.AddWithValue("middlename", userData.MiddleName);
                        cmdInsert.Parameters.AddWithValue("gender", userData.Gender);
                        cmdInsert.Parameters.AddWithValue("age", userData.Age);
                        cmdInsert.Parameters.AddWithValue("address", userData.Address);

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
            }
            return View();
        }
    }
}