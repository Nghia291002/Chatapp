using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Chatapp.Models;
using System.Runtime.InteropServices.WindowsRuntime;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNet.SignalR;
using Chatapp.Filters;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace Chatapp.Controllers
{
    [MyAuthenFilterz]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            
                string userName = (string)Session["UserName"];
                ViewBag.UserName = userName;
                var userID = (decimal)Session["User_ID"];
                ViewBag.UserID = userID; 
                CHAT_WEBEntities2 db = new CHAT_WEBEntities2();

                List<decimal?> groupIds = db.GROUP_ACCOUNTS.Where(u => u.USER_ID == userID).Select(u => u.GROUP_ID).ToList();
                Session["groupid"] = groupIds;

                ViewBag.groupid = groupIds;
                 // Truy vấn tên các nhóm theo danh sách groupIds
                List<string> groupnames = db.GROUPS.Where(u => groupIds.Contains(u.GROUP_ID)).Select(u => u.GROUP_NAME).ToList();

                ViewBag.group = groupnames;
                Session["group"] = groupnames;
                return View();
        }
        [HttpPost]
              public ActionResult Index(string text )
        {
            CHAT_WEBEntities2 db = new CHAT_WEBEntities2();
            if (Session["User_ID"] != null)
            {
                decimal id;

                // Kiểm tra giá trị của Session["User_ID"] có thể chuyển đổi thành decimal không
                if (decimal.TryParse(Session["User_ID"].ToString(), out id))
                {
                    MESSAGE message = new MESSAGE
                    {
                        GROUP_ACCOUNT_ID = id,
                        TEXT = text
                    };

                    // Kiểm tra db không phải là null
                    if (db != null)
                    {
                        db.MESSAGEs.Add(message);
                        db.SaveChanges();
                        chatHub chat = new chatHub();
                        chat.send(Session["UserName"].ToString(), text);
                    }
                    else
                    {
                        // Xử lý lỗi khi db là null
                        throw new InvalidOperationException("Database context is null.");
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    // Xử lý lỗi khi không thể chuyển đổi Session["User_ID"] thành decimal
                    throw new InvalidCastException("Session['User_ID'] is not a valid decimal.");
                }
            }
            else
            {
                return RedirectToAction("Login","Account");   
            }
               
        }
       

        public ActionResult Search(string search = " ")
        {

            string userName = (string)Session["UserName"];
            ViewBag.UserName = userName;
            CHAT_WEBEntities2 db = new CHAT_WEBEntities2();
                List<USER> user = db.USERs.Where(u => u.USERNAME.Contains(search)).ToList();
                ViewBag.Search = search;
                return View(user);
        }

        public ActionResult AddGroup()
        {
            string userName = (string)Session["UserName"];
            ViewBag.UserName = userName;
            return View();
        }
        [HttpPost]
        public ActionResult AddGroup(string groupname, string username)
        {

            string username1 = (string)Session["UserName"];
            CHAT_WEBEntities2 db = new CHAT_WEBEntities2();
            var check = db.GROUPS.Where(u => u.GROUP_NAME == groupname && u.USERNAME == username1).FirstOrDefault();
            if (check != null)
            {
                ViewBag.Fail = "Nhóm đã tồn tại";
                return View();
            }
            else
            {
                GROUP newgroup = new GROUP
                {
                    USERNAME = username1,
                    GROUP_NAME = groupname
                };

                // Thêm đối tượng vào cơ sở dữ liệu
                db.GROUPS.Add(newgroup);
                db.SaveChanges();
            }
            var find1 = db.GROUPS.Where(u => u.GROUP_NAME == groupname).Select(u => u.GROUP_ID).FirstOrDefault();
            var login1 = db.USERs.Where(u => u.USERNAME == username1).Select(u => u.USER_ID).FirstOrDefault();
            GROUP_ACCOUNTS add = new GROUP_ACCOUNTS
            {
                USER_ID = login1,
                GROUP_ID = find1
            };
            db.GROUP_ACCOUNTS.Add(add);
            var find = db.GROUPS.Where(u => u.GROUP_NAME == groupname).Select(u => u.GROUP_ID).FirstOrDefault();
            var login = db.USERs.Where(u => u.USERNAME == username).Select(u => u.USER_ID).FirstOrDefault();
            if (login == null && find == null)
            {
                    return View();
             }
            else
             {
                    GROUP_ACCOUNTS newhi = new GROUP_ACCOUNTS
                    {
                        USER_ID = login,
                        GROUP_ID = find
                    };

                    // Thêm đối tượng vào cơ sở dữ liệu
                    db.GROUP_ACCOUNTS.Add(newhi);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            
        }

        public ActionResult Chat(string groupname)
        {
               
            string userName = (string)Session["UserName"];
            var userID = (decimal)Session["User_ID"];
            CHAT_WEBEntities2 db = new CHAT_WEBEntities2();
            var group = db.GROUPS.FirstOrDefault(g => g.GROUP_NAME == groupname);
            Session["groupname"] = groupname;
            var id = db.GROUPS.Where(u => u.GROUP_NAME == groupname).Select(u => u.GROUP_ID).FirstOrDefault();
            List<MESSAGE> hi = db.MESSAGEs.ToList();

            ViewBag.UserName = userName;
            ViewBag.UserID = userID;
            ViewBag.groupheader = Session["groupname"];
            ViewBag.groupid = id;
            ViewBag.grouplist = Session["group"];
            ViewBag.chat = hi;
            return View(hi);
        }
        [HttpPost]
        public ActionResult SendChat(string text, string groupname)
        {
            var id = (decimal)Session["User_ID"];
            CHAT_WEBEntities2 db = new CHAT_WEBEntities2();
            var nameid = db.GROUPS.Where(u => u.GROUP_NAME == groupname).Select(u => u.GROUP_ID).FirstOrDefault();

            var find = db.GROUP_ACCOUNTS.Where(u => u.USER_ID == id && u.GROUP_ID == nameid).Select(u => u.GROUP_ACCOUNT_ID).FirstOrDefault();
            if (Session["User_ID"] != null)
            {
                    MESSAGE message = new MESSAGE
                    {
                        GROUP_ACCOUNT_ID = find,
                        TEXT = text
                    };

                    // Kiểm tra db không phải là null
                    //if (db != null)
                    //{
                        db.MESSAGEs.Add(message);
                        db.SaveChanges();
                        chatHub chat = new chatHub();
                        chat.send(Session["UserName"].ToString(), text);
                    //}
                    //else
                    //{
                    //    // Xử lý lỗi khi db là null
                    //    throw new InvalidOperationException("Database context is null.");
                    //}

                    return RedirectToAction("Chat", new {groupname = groupname});
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }
    }
}