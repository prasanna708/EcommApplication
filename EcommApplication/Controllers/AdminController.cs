using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using EcommApplication.Models;
using System.IO;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace EcommApplication.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        ApplicationDbContext db = new ApplicationDbContext();

        public void ErrorLogDetails(Exception ex, string ActionMethodName)
        {
            string uid = Session["uid"].ToString(); 
            
            ErrorLogs logs = new ErrorLogs()
            {
                UserId = uid,
                ErrorActionMethod = ActionMethodName,
                ErrorCode = ex.HResult,
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace,
                DateAndTime = DateTime.Now
            };
            db.ErrorLogs.Add(logs);
            db.SaveChanges();
        }

        public void ActivityLogDetails(string userId,string action,string table,int? tid,string details)
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


        public void DataLogDetails(string table,int? id,string property,string oldValue, string newValue,string userId)
        {
            DataLog log = new DataLog()
            {
                TableEffected= table,
                PropertyId = id,
                PropertyEffected= property,
                OldValue = oldValue,
                NewValue = newValue,
                UserId = userId,
                ActivityDateAndTime= DateTime.Now
            };
            db.DataLogs.Add(log);
            db.SaveChanges();
        }


        public ActionResult HomePage()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View(new User());
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            try
            {
                var obj = db.Users.Find(user.UserId);
                if (obj != null)
                {
                    Session["uid"] = obj.UserId;
                    Session["Name"] = obj.Name;
                    string details = "User " + obj.Name + " logged in.";
                    ActivityLogDetails(obj.UserId, "Login" , "Users", 0 , details);
                    if (obj.Role == "User")
                    {
                        TempData["success"] = "Login Successful....";
                        return RedirectToAction("UserHomePage", "Users");
                    }
                    else if (obj.Role == "Admin")
                    {
                        TempData["success"] = "Login Successful....";
                        return RedirectToAction("AdminHomePage", "Admin");
                    }
                    else if (obj.Role == "Finance")
                    {
                        TempData["success"] = "Login Successful....";
                        return RedirectToAction("FinanceHomePage", "Finance");
                    }
                }
                else
                    TempData["error"] = "invalid credentials..";
                return View(user);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex , "Login");
                throw ex;
            }
        }

        public ActionResult AdminHomePage()
        {
            return View();
        }

        public ActionResult DisplayProducts()
        {
            try
            {
                var products = db.Products.ToList();
                return View(products);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "DisplayProducts");
                throw ex;
            }
        }

        public ActionResult DisplayProduct(int? id)
        {
            try
            {
                var product = db.Products.FirstOrDefault(p => p.ProductId == id);
                Session["productName"] = product.Name;
                return View(product);
            }
            catch ( Exception ex )
            {
                ErrorLogDetails(ex, "DisplayProduct");
                throw ex;
            }
        }

        public ActionResult AddProduct()
        {
            return View(new Product());
        }

        [HttpPost]
        public ActionResult AddProduct(Product product, HttpPostedFileBase selectedFile)
        {
            try
            {
                if (selectedFile != null)
                {
                    string physicalPath = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(physicalPath))
                    {
                        Directory.CreateDirectory(physicalPath);
                    }
                    selectedFile.SaveAs(physicalPath + selectedFile.FileName);
                    product.Photo = selectedFile.FileName;
                }
                db.Products.Add(product);
                TempData["success"] = "Product Added successfully....";
                db.SaveChanges();
                string details = "Product " + product.Name + " added.";
                string uid = Session["uid"].ToString();
                ActivityLogDetails(uid, "Insert", "Products",product.ProductId, details);
                return RedirectToAction("DisplayProducts");
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "AddProduct");
                throw ex;
            }
        }

        public ActionResult EditProduct(int? id)
        {
            try
            {
                var product = db.Products.Find(id);
                TempData["Photo"] = product.Photo;
                return View(product);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "EditProduct");
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UpdateProduct(Product product, HttpPostedFileBase selectedFile)
        {
            try
            {
                var prod = db.Products.FirstOrDefault(p => p.ProductId == product.ProductId);
                
                    string oldName = prod.Name;
                    double? oldPrice = prod.Price;
                    int? oldQuantity = prod.Quantity;
                if (selectedFile != null)
                {
                    string physicalPath = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(physicalPath))
                    {
                        Directory.CreateDirectory(physicalPath);
                    }
                    selectedFile.SaveAs(physicalPath + selectedFile.FileName);
                    product.Photo = selectedFile.FileName;
                }
                else if (TempData["Photo"] != null)
                {
                    product.Photo = TempData["Photo"].ToString();
                }
                if (prod != null)
                {
                    prod.Name = product.Name;
                    prod.Price = product.Price;
                    prod.Quantity = product.Quantity;
                    prod.Photo = product.Photo;
                }

                TempData["success"] = "Product Updated successfully....";
                db.SaveChanges();
                string details = "Product " + product.Name + " Updated.";
                string uid = Session["uid"].ToString();
                ActivityLogDetails(uid, "Update", "Products", product.ProductId, details);
                if (oldName != null && product.Name != null)
                {
                    DataLogDetails("Products", product.ProductId, "Name", oldName, product.Name, uid);
                }
                if (oldPrice != null && product.Price != null)
                {
                    DataLogDetails("Products", product.ProductId, "Price", oldPrice.ToString(), product.Price.ToString(), uid);
                }
                if (oldQuantity != null && product.Quantity != null)
                {
                    DataLogDetails("Products", product.ProductId, "Quantity", oldQuantity.ToString(), product.Quantity.ToString(), uid);
                }
                return RedirectToAction("DisplayProducts");
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "UpdateProduct");
                throw ex;
            }
        }

        public ActionResult DeleteProduct(int? id)
        {
            try
            {
                var product = db.Products.Find(id);
                db.Entry(product).State = EntityState.Deleted;
                TempData["success"] = "Product Deleted successfully....";
                db.SaveChanges();
                string details = "Product " + product.Name + " deleted.";
                string uid = Session["uid"].ToString();
                ActivityLogDetails(uid, "Delete", "Products", id, details);
                return RedirectToAction("DisplayProducts");
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "DeleteProduct");
                throw ex;
            }
        }

        public ActionResult ViewUsers()
        {
            try
            {
                var user = db.Users.ToList();
                return View(user);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "ViewUsers");
                throw ex;
            }
        }

        [NonAction]
        public ActionResult DeleteUser(string id)
        {
            try
            {
                var user = db.Users.Find(id);
                db.Users.Remove(user);
                TempData["success"] = "User deleted successfully...";
                db.SaveChanges();
                return RedirectToAction("DisplayProducts");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult ViewOrders()
        {
            try
            {
                var orders = db.PurchaseAuditLogs.ToList();
                var total = orders.Sum(o => o.TotalPrice);
                ViewBag.Total = Convert.ToInt32(total);
                return View(orders);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "ViewOrders");
                throw ex;
            }
        }

        public ActionResult TotalSales()
        {
            try
            {
                //DateTime date = Convert.ToDateTime(DateTime.Now.ToString());
                DateTime date = DateTime.Today.AddDays(1);
                var obj = db.PurchaseAuditLogs.Where(o => o.OrderDateAndTime >= DateTime.Today && o.OrderDateAndTime < date);
                var sum = obj.Sum(o => o.TotalPrice);
                ViewBag.TotalSales = Convert.ToInt32(sum);
                return View(obj);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "TotalSales");
                throw ex;
            }
        }

        public ActionResult StockLeft()
        {
            try
            {
                var order = db.Products.ToList();
                return View(order);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex, "StockLeft");
                throw ex;
            }
        }

        public ActionResult EditProfile(string id)
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
        public ActionResult EditProfile(User user)
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
                        DataLogDetails("Users", 0 , "Name", oldName ,user.Name, user.UserId);
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
                    return RedirectToAction("EditProfile");
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