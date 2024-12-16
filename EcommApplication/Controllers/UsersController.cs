using EcommApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace EcommApplication.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        ApplicationDbContext db = new ApplicationDbContext();
        /*public ActionResult UsersLogin()
        {
            return View(new User());
        }

        [HttpPost]
        public ActionResult UsersLogin(User user)
        {
            var obj = db.Users.SingleOrDefault(u => u.UserId == user.UserId && u.Pwd == user.Pwd);
            if (obj != null)
            {
                TempData["success"] = "Login Successful";
                TempData["uid"] = user.UserId;
                Session["uid"] = user.UserId;
                return RedirectToAction("UserHomePage");
            }
            else
            {
                TempData["success"] = "Login failed.....";
            }
            return View(user);
        }
        */

        public void ActivityLogDetails(string userId, string action, string table, int? tid, string details)
        {
            ActivityLog log = new ActivityLog()
            {
                UserId = userId,
                ActivityDateAndTime = DateTime.Now,
                Action = action,
                TableEffected = table,
                TableId = tid,
                Details = details
            };
            db.ActivityLogs.Add(log);
            db.SaveChanges();
        }

        public void ErrorLogDetails(Exception ex , string ActionMethodName)
        {
            ErrorLogs logs = new ErrorLogs()
            {
                UserId = Session["uid"].ToString(),
                ErrorActionMethod = ActionMethodName,
                ErrorCode = ex.HResult,
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace,
                DateAndTime = DateTime.Now
            };
            db.ErrorLogs.Add(logs);
            db.SaveChanges();
        }

        public void DataLogDetails(string table, int? id, string property, string oldValue, string newValue, string userId)
        {
            DataLog log = new DataLog()
            {
                TableEffected = table,
                PropertyId = id,
                PropertyEffected = property,
                OldValue = oldValue,
                NewValue = newValue,
                UserId = userId,
                ActivityDateAndTime = DateTime.Now
            };
            db.DataLogs.Add(log);
            db.SaveChanges();
        }

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(User user , FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string dob = form["dob"];
                    user.Dob = Convert.ToDateTime(form["dob"]);
                    db.Users.Add(user);
                    db.SaveChanges();
                    TempData["success"] = "Registration successful.Try login with your uid and password";
                    string details = $"{user.Name} Registered successfully..";
                    ActivityLogDetails(user.UserId, "Registration", "Users", 0 , details);
                    return RedirectToAction("Login", "Admin");
                }
                return View(user);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "Registration");
                throw ex;
            }
        }

        public ActionResult UserHomePage()
        {
            return View();
        }

        public ActionResult DisplayAllProducts()
        {
            try
            {
                var products = db.Products.ToList();
                if (Session["uid"] != null)
                {
                    ActivityLogDetails(Session["uid"].ToString(), "ViewProducts", "Product", 0 , "User viewed all products.");
                }
                return View(products);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex , "DisplayAllProducts");
                throw ex;
            }
        }

        public ActionResult DisplayProductDetails(int? id)
        {
            try
            {
                var product = db.Products.Find(id);
                if (Session["uid"] != null)
                {
                    ActivityLogDetails(Session["uid"].ToString(), "ViewProducts", "Product", id, "User viewed all products.");
                }
                return View(product);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "DisplayProductDetails");
                throw ex;
            }
        }

        public ActionResult AddTocart(int? id)
        {
            try
            {
                var product = db.Products.Find(id);
                Order order = new Order
                {
                    ProductId = id.Value,
                    Name = product.Name,
                    Photo = product.Photo
                };
                if (Session["uid"] != null)
                {
                    order.UserId = Session["uid"].ToString();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "AddTocart");
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult AddTocart(Product prod, string uid , Order order)
        {
            try
            {
                var product = db.Products.FirstOrDefault(p => p.ProductId == prod.ProductId);
                if (Session["uid"] != null)
                {
                    uid = Session["uid"].ToString();
                }
                if (product != null)
                {
                    order.ProductId = product.ProductId;
                    order.Name = product.Name;
                    order.OrderDateAndTime = DateTime.Now;
                    order.UserId = uid;
                    order.TotalPrice = order.Quantity * product.Price;
                    order.Photo = product.Photo;
                    db.Orders.Add(order);
                    db.SaveChanges();
                    TempData["success"] = "Product added to Cart Successfully....";
                    string details = $"{order.Name} Added to Cart.";
                    ActivityLogDetails(uid, "AddToCart", "Orders", order.OrderId, details);
                    return RedirectToAction("DisplayAllProducts");
                }
                else
                    TempData["success"] = "Product not found....";
                return View(order);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "AddTocart");
                throw ex;
            }
        }
        
        public ActionResult ViewCart(string uid)
        {
            try
            {
                if (Session["uid"] != null)
                {
                    uid = Session["uid"].ToString();
                }
                var orders = db.Orders.Where(o => o.UserId == uid).ToList();
                return View(orders);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "ViewCart");
                throw ex;
            }
        }

        public ActionResult DeleteOrders(int id)
        {
            try
            {
                var order = db.Orders.Find(id);
                db.Orders.Remove(order);
                db.SaveChanges();
                string uid = Session["uid"].ToString();
                TempData["success"] = "Order Cancelled....";
                string details = $"{order.Name} deleted from Cart.";
                ActivityLogDetails(uid, "DeleteCartItem", "Orders", id, details);
                return RedirectToAction("ViewCart");
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "DeleteOrders");
                throw ex;
            }
        }

        public ActionResult PlaceOrder(string id)
        {
            try
            {
                if (Session["uid"] != null)
                {
                    id = Session["uid"].ToString();
                }
                var order = db.Orders.Where(o => o.UserId == id).ToList();
                foreach (var item in order)
                {
                    PurchaseAuditLogs logs = new PurchaseAuditLogs()
                    {
                        OrderId = item.OrderId,
                        ProductId = item.ProductId,
                        UserId = id,
                        ProductName = item.Name,
                        OrderDateAndTime = DateTime.Now,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice,
                        Photo = item.Photo,
                    };
                    db.PurchaseAuditLogs.Add(logs);

                    var product = db.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                    if (product != null)
                    {
                        product.Quantity = product.Quantity - item.Quantity;
                        if (product.Quantity < 0)
                        {
                            product.Quantity = 0;
                        }
                    }
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    string details = "Order Placed.";
                    ActivityLogDetails(id, "PlacingOrder", "PurchaseAudit", logs.OrderNumber, details);

                }
                db.Orders.RemoveRange(order);

                db.SaveChanges();
                TempData["success"] = "Order Placed Successfully....";
                return RedirectToAction("ViewCart");
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "PlaceOrder");
                throw ex;
            }
            
        }

        public ActionResult ViewOrders(string id)
        {
            try
            {
                if (Session["uid"] != null)
                {
                    id = Session["uid"].ToString();
                }
                var purchases = db.PurchaseAuditLogs.Where(p => p.UserId == id).ToList();
                var sum = purchases.Sum(p => p.TotalPrice);
                ViewBag.TotalPrice = Convert.ToInt32(sum);
                return View(purchases);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "ViewOrders");
                throw ex;
            }
        }

        public ActionResult EditUserProfile(string id)
        {
            try
            {
                var obj = db.Users.Find(id);
                return View(obj);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "EditProfile");
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult EditUserProfile(User user)
        {
            try
            {
                var obj = db.Users.FirstOrDefault(u => u.UserId == user.UserId);
                string oldName = obj.Name;
                string oldPwd = obj.Pwd;
                long? oldNumber = obj.MobileNumber;
                string oldEmail = obj.Email;
                string oldLocation = obj.Location;
                DateTime oldDOB = obj.Dob;
                if (obj == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    obj.Name = user.Name;
                    obj.Email = user.Email;
                    obj.UserId = user.UserId;
                    obj.MobileNumber = user.MobileNumber;
                    obj.Dob = user.Dob;
                    obj.Location = user.Location;
                    obj.Pwd = user.Pwd;

                    TempData["success"] = "Profile updated successfully successfully...";
                    db.SaveChanges();
                    if (oldName != null && user.Name != null)
                    {
                        DataLogDetails("Users", 0, "Name", oldName, user.Name, user.UserId);
                    }
                    if (oldPwd != null && user.Pwd != null)
                    {
                        DataLogDetails("Users", 0, "Password", oldPwd, user.Pwd, user.UserId);
                    }
                    if (oldNumber != null && user.MobileNumber != null)
                    {
                        DataLogDetails("Users", 0, "MobileNo", oldNumber.ToString(), user.MobileNumber.ToString(), user.UserId);
                    }
                    if (oldEmail != null && user.Email != null)
                    {
                        DataLogDetails("Users", 0, "Email", oldEmail, user.Email, user.UserId);
                    }
                    if (oldDOB != null && user.Dob != null)
                    {
                        DataLogDetails("Users", 0, "DOB", oldDOB.ToString(), user.Dob.ToString(), user.UserId);
                    }
                    if (oldLocation != null && user.Location != null)
                    {
                        DataLogDetails("Users", 0, "Location", oldLocation, user.Location, user.UserId);
                    }
                    return RedirectToAction("EditUserProfile");
                }
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "EditProfile");
                throw ex;
            }
        }
    }
}