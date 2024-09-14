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
using Chatapp.ViewModel;

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
        public ActionResult Index(string text, string groupname)
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
                        chat.send(Session["UserName"].ToString(), text, groupname);
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
                return RedirectToAction("Login", "Account");
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
            CHAT_WEBEntities2 db = new CHAT_WEBEntities2();
            var userID = (decimal)Session["User_ID"];
            List<USER> ids = db.USERs.Where(u => !(u.USER_ID == userID)).ToList();
            ViewBag.namefriends = ids;
            return View();
        }
        [HttpPost]
        public ActionResult AddGroup(string groupname,List<decimal?> ids)
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
            if (find == null)
            {
                return View();
            }
            else
            {
                foreach (var item in ids)
                {
                    GROUP_ACCOUNTS add_user = new GROUP_ACCOUNTS
                    {
                        USER_ID = item,
                        GROUP_ID = find
                    };
                    db.GROUP_ACCOUNTS.Add(add_user);  
                }
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
            //Session["groupname"] = groupname;
            var id = db.GROUPS.Where(u => u.GROUP_NAME == groupname).Select(u => u.GROUP_ID).FirstOrDefault();
            List<MESSAGE> hi = db.MESSAGEs.Where(u=>u.GROUP_ACCOUNTS.GROUP_ID == id).ToList();

            var users = db.GROUP_ACCOUNTS.Where(u => u.GROUP_ID == id).Select(u => u.USER_ID).ToList();

            var user_admin = db.GROUPS.Where(u => u.GROUP_ID == id).Select(u => u.USERNAME).FirstOrDefault();

            var user_admin_id = db.USERs.Where(u => u.USERNAME == user_admin).Select(u => u.USER_ID).FirstOrDefault();

            List<decimal?> user_id_delete = db.GROUP_ACCOUNTS.Where(u => !(u.USER_ID == user_admin_id) && u.GROUP_ID == id).Select(u=>u.USER_ID).ToList();

            List<USER> id_delete = db.USERs.Where(u => user_id_delete.Contains(u.USER_ID)).ToList();

            List<USER> ids = db.USERs.Where(u => !(users.Contains(u.USER_ID))).ToList();

            decimal sumUserIds = (decimal)users.Count();
            ViewBag.sum = sumUserIds;
            ViewBag.namefriends = ids;
            ViewBag.User_id_delete = id_delete;
            ViewBag.UserName = userName;
            ViewBag.UserID = userID;
            ViewBag.User_admin = user_admin;
            ViewBag.groupheader = groupname;
            ViewBag.groupid = id;
            ViewBag.grouplist = Session["group"];
            ViewBag.chat = hi;
            return View(hi);

        }
        [HttpPost]
        public ActionResult SendChat(string text, string groupname)
        {
            var ids = (decimal)Session["User_ID"];
            CHAT_WEBEntities2 db = new CHAT_WEBEntities2();
            var nameid = db.GROUPS.Where(u => u.GROUP_NAME == groupname).Select(u => u.GROUP_ID).FirstOrDefault();

            var find = db.GROUP_ACCOUNTS.Where(u => u.USER_ID == ids && u.GROUP_ID == nameid).Select(u => u.GROUP_ACCOUNT_ID).FirstOrDefault();

            if (Session["User_ID"] != null)
            {
                //    if (id != null)
                //    {
                //        GROUP_ACCOUNTS add = new GROUP_ACCOUNTS
                //        {
                //            USER_ID = id,
                //            GROUP_ID = nameid
                //        };
                //        db.GROUP_ACCOUNTS.Add(add);
                //        db.SaveChanges();

                //        return RedirectToAction("Chat", new { groupname = groupname });
                //    }
                MESSAGE message = new MESSAGE
                {
                    GROUP_ACCOUNT_ID = find,
                    TEXT = text
                };
                db.MESSAGEs.Add(message);
                db.SaveChanges();
                chatHub chat = new chatHub();
                chat.send(Session["UserName"].ToString(), text, groupname);

                // Tính tổng số lượng người dùng

                return RedirectToAction("Chat", new { groupname = groupname });

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult AddUsers(string groupname, List<decimal?> ids)
        {
            var id = (decimal)Session["User_ID"];
            CHAT_WEBEntities2 db = new CHAT_WEBEntities2();
            var nameid = db.GROUPS.Where(u => u.GROUP_NAME == groupname).Select(u => u.GROUP_ID).FirstOrDefault();

            var find = db.GROUP_ACCOUNTS.Where(u => u.USER_ID == id && u.GROUP_ID == nameid).Select(u => u.GROUP_ACCOUNT_ID).FirstOrDefault();

            if (Session["User_ID"] != null)
            {
                if (ids != null)
                {
                    foreach (var item in ids) 
                    {
                        GROUP_ACCOUNTS add = new GROUP_ACCOUNTS
                        {
                            USER_ID = item,
                            GROUP_ID = nameid
                        };
                        db.GROUP_ACCOUNTS.Add(add);
                        db.SaveChanges();
                    }
                  
                    return RedirectToAction("Chat", new { groupname = groupname });
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        public ActionResult DeleteUsers(string groupname, List<decimal?> ids_delete )
        {
            var id = (decimal)Session["User_ID"];
            CHAT_WEBEntities2 db = new CHAT_WEBEntities2();
            var nameid = db.GROUPS.Where(u => u.GROUP_NAME == groupname).Select(u => u.GROUP_ID).FirstOrDefault();

          

            if (Session["User_ID"] != null)
            {
                if (ids_delete != null)
                {
                    foreach (var item in ids_delete)
                    {
                        var find = db.GROUP_ACCOUNTS.Where(u => ids_delete.Contains(u.USER_ID) && u.GROUP_ID == nameid).Select(u => u.GROUP_ACCOUNT_ID).ToList();
                        List<MESSAGE> delete_message = db.MESSAGEs.Where(u => find.Contains((decimal)u.GROUP_ACCOUNT_ID)).ToList();
                        foreach (MESSAGE del in delete_message) {
                            db.MESSAGEs.Remove(del);
                            db.SaveChanges();
                        }
                        GROUP_ACCOUNTS delete = db.GROUP_ACCOUNTS.Where(u => u.USER_ID == @item && u.GROUP_ID == nameid).FirstOrDefault();
                        db.GROUP_ACCOUNTS.Remove(delete);
                        db.SaveChanges();
                    }

                    return RedirectToAction("Chat", new { groupname = groupname });
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

    }
}