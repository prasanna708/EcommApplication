using EcommApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommApplication.Controllers
{
    public class FinanceController : Controller
    {
        // GET: Finance
        ApplicationDbContext db = new ApplicationDbContext();


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

        public void ErrorLogDetails(Exception ex, string ActionMethodName)
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
        public ActionResult FinanceHomePage()
        {
            return View();
        }

        public ActionResult ViewSales(SalesViewModel sales)
        {
            try
            {
                var filteredSales = db.PurchaseAuditLogs.AsQueryable();
                if (sales.StartDate != null)
                {
                    var startDate = sales.StartDate;
                    filteredSales = filteredSales.Where(f => f.OrderDateAndTime >= startDate);
                }
                if (sales.EndDate != null)
                {
                    var endDate = sales.EndDate;
                    filteredSales = filteredSales.Where(f => f.OrderDateAndTime <= endDate);
                }
                sales.FilteredSales = filteredSales.ToList();
                return View(sales);
            }
            catch (Exception ex)
            {
                ErrorLogDetails(ex , "ViewSales");
                throw ex;
            }
        }

        public ActionResult EditFinanceProfile(string id)
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
        public ActionResult EditFinanceProfile(User user)
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
                    return RedirectToAction("EditFinanceProfile");
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