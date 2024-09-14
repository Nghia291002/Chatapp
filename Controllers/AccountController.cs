using Chatapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chatapp.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password, string username1, string password1, string confirmpassword)
        {
            CHAT_WEBEntities2 db = new CHAT_WEBEntities2();
            // Lấy mật khẩu đã băm từ cơ sở dữ liệu dựa trên username
            var passwordHash = db.USERs
                                  .Where(u => u.USERNAME == username)
                                  .Select(u => u.PASSWORD)
                                  .FirstOrDefault(); // Trả về mật khẩu đã băm
            if (passwordHash == null)
            {
                ViewBag.Fail = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View();
            }
            else
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
                var login = db.USERs.Where(u => u.USERNAME == username && isPasswordValid.ToString() == "True").Select(u => u.USER_ID).FirstOrDefault();



                if (login != default(decimal))
                {
                    Session["User_ID"] = login;
                    string loginID = login.ToString();
                    Session["UserID"] = loginID;
                    Session["UserName"] = username;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Fail = "Tên đăng nhập hoặc mật khẩu không đúng";
                    return View();
                }
                if (password1 != confirmpassword)
                {
                    ViewBag.Fail = "Xác nhận mật khẩu không đúng";
                    return View();
                }
                else
                {
                    USER check = db.USERs.Where(row => row.USERNAME == username1).FirstOrDefault();
                    if (check != null)
                    {
                        check.USERNAME = username1;
                        check.PASSWORD = password1;
                    }
                    else
                    {
                        ViewBag.Fails = "Vui lòng kiểm tra lại tên đăng nhập ";
                    }
                    db.SaveChanges();
                    return View(check);
                }
            }
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(string username, string password, string confirmpassword)
        {
            if (password != confirmpassword)
            {
                ViewBag.Fail = "Xác nhận mật khẩu không đúng";
                return View();
            }
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 8);
            CHAT_WEBEntities2 db = new CHAT_WEBEntities2();
            USER login = db.USERs.Where(u => u.USERNAME == username).FirstOrDefault();
            if (login != null)
            {
                ViewBag.Fail = "Tên đăng nhập đã tồn tại";
                return View();
            }
            else
            {
                USER newUser = new USER
                {
                    USERNAME = username,
                    PASSWORD = passwordHash
                };

                // Thêm đối tượng vào cơ sở dữ liệu
                db.USERs.Add(newUser);
                 db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Forgot()
        {
            string userName = (string)Session["UserName"];
            var userID = (decimal)Session["User_ID"];
            return View();
        }
        [HttpPost]
        public ActionResult Forgot(string password, string confirmpassword)
        {
            string username = (string)Session["UserName"];
            if (password != confirmpassword)
            {
                ViewBag.Fail = "Xác nhận mật khẩu không đúng";
                return View();
            }
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password,8);
            CHAT_WEBEntities2 db = new CHAT_WEBEntities2();
            USER login = db.USERs.Where(u => u.USERNAME == username).FirstOrDefault();
            if (login != null)
            {
                // Cập nhật mật khẩu của người dùng
                login.PASSWORD = passwordHash;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
                ViewBag.done = "Thay đổi mật khẩu thành công";
            }
            //return RedirectToAction("Index", "Home");
            return View();

        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}