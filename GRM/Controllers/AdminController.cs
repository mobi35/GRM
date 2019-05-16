using GRM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using System.Diagnostics;
using System.Data.Entity.Validation;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace GRM.Controllers
{
    public class AdminController : Controller
    {
        GRMDB GRM = new GRMDB();
        // GET: Admin

        
        public ActionResult Index(int? year, int? months, string branchManager)
        {
            string user = Session["userName"].ToString();

            var allClients = GRM.tbl_Users.Where(a => a.branchManager == user).ToList();
                ViewBag.MyClients = allClients;
          

            var branches = GRM.tbl_Users.Where(a => a.position == "branchmanager").ToList();
            ViewBag.branches = branches;

            ViewBag.RatingList = GRM.tbl_Rating.ToList();

         
            var searchBM = GRM.tbl_Users.FirstOrDefault(a => a.branchManager == user);

            var searchRating = GRM.tbl_Rating.FirstOrDefault(a => a.client == searchBM.userName);
            ViewBag.Ratings = searchRating;

            var searchService = GRM.tbl_Rating.Select(a => a.services).ToList();
            var searchQuality = GRM.tbl_Rating.Select(a => a.quality).ToList();
            var searchPr = GRM.tbl_Rating.Select(a => a.price_improvement).ToList();
             double serviceTotal = 0;
            foreach (var a in searchService)
            {
                serviceTotal += (double)a;
            }

            double totally = serviceTotal / (double) searchService.Count();
            ViewBag.TotalService = Math.Round(totally, 1);

            double qualityTotal = 0;
            foreach (var a in searchQuality)
            {
                qualityTotal += (double)a;
            }

            double qualityTotally = qualityTotal / (double)searchQuality.Count();
            ViewBag.TotalQuality = Math.Round(qualityTotally, 1);


            double improvementTotal = 0;
            foreach (var a in searchPr)
            {
                improvementTotal += (double)a;
            }

            double improvementTotals = improvementTotal / (double)searchPr.Count();
            ViewBag.TotalImprovement = Math.Round(improvementTotals, 1);


            ViewBag.TotalSatisfaction = Math.Round( (Math.Round(improvementTotals, 1) + Math.Round(qualityTotally, 1) + Math.Round(totally, 1) ) / 3,1);

            if(branchManager == null || branchManager == "") {  

            var selectTry = (from c in GRM.tbl_OrderList
                             group c by new { c.prodName } into grouping
                             
                             select new
                             {

                                 ProductName = grouping.Key.prodName,
                                 Value = grouping.Sum(a => a.quantity),
                                 Count = grouping.Count()

                             }).ToList();
            List<int?> list = new List<int?>();
            List<string> label = new List<string>();
            foreach (var a in selectTry.OrderByDescending(a => a.Value).Take(6)) {
                
                list.Add(a.Value);
                label.Add(a.ProductName);
            }
            ViewBag.labelBestProducts = label;
            ViewBag.listBestProducts = list;

            }else
            {
              
               List< BestSellers> bestList = new List<BestSellers>();
                var getProduct = GRM.tbl_Products.Where(a => a.packageBy == branchManager).ToList() ;
                var getList = GRM.tbl_OrderList.ToList();
                foreach (var prod in getProduct)
                {
                    BestSellers best = new BestSellers();
                    best.numberOfSales = 0;
                    foreach (var list in getList)
                    {
                        if(prod.productId == list.packageId)
                            best.numberOfSales += list.quantity;
                        Console.WriteLine(list.quantity);
                    }
                  
                   best.product =  prod.productName;
                    bestList.Add(best);
                   
                  
                }

           
           

                ViewBag.labelBestProducts = bestList.OrderByDescending(a => a.numberOfSales).Select(a => a.product);
                ViewBag.listBestProducts = bestList.OrderByDescending(a => a.numberOfSales).Select(a => a.numberOfSales);

            }
            ViewBag.year = year;
            ViewBag.months = months;
            ViewBag.branch = branchManager;


            List<string> newLabel = new List<string>();
            var selectUser = GRM.tbl_Users.Where(a =>  a.branchManager != null ).GroupBy(a => a.branchManager).ToList();
            foreach (var x in selectUser)
            {
                newLabel.Add(x.FirstOrDefault().branchManager);
            }
            ViewBag.label = newLabel;
            List<Double?> data = new List<Double?>();
            double? total = 0;
         
            foreach (var item in newLabel) {
                var selectClient = GRM.tbl_Users.Where(a => a.branchManager == item ).FirstOrDefault();
                var zx = GRM.tbl_OrderHistory.Where(a => a.branch == selectClient.branchManager && a.isApproved == 1).ToList();
               
                foreach (var compute in zx) {
                    total += compute.totalPrice;
                   

                }
                Debug.WriteLine(total);
              
                data.Add(total);
                total = 0;
               
            }

            if (year == null) {
                year = DateTime.Now.Year;
            }
            if(months == null) {
                months = DateTime.Now.Month;
            }

            //DAILYY
            var dayDash = GRM.tbl_Feedbacks.ToList();
            List<int> inquiryPerDay = new List<int>();
            List<int> inquiryPerDayLabel = new List<int>();
            int countDay = 0;
            int y = Convert.ToInt32(year);
            int m = Convert.ToInt32(months);
            int daysInMonth = DateTime.DaysInMonth(y, m);
            
            for (int i = 1; i <= daysInMonth; i++)
            {
                foreach (var das in dayDash)
                {
                    if (das.dateS.Value.Day == i && das.dateS.Value.Year == year && das.dateS.Value.Month == months)
                    {
                        countDay++;
                    }
                }
                inquiryPerDayLabel.Add(i);
                inquiryPerDay.Add(countDay);
                countDay = 0;
            }
            ViewBag.inquiryPerDayLabel = inquiryPerDayLabel;
            ViewBag.inquiryPerDay = inquiryPerDay;
            //DAILYY

            //Inquiry Dashboard
            // Months
            var dash = GRM.tbl_Feedbacks.ToList();
            List<int> inquiryPerMonth = new List<int>();
            for (int i = 1; i <= 12; i++)
            {
                int count = 0;
                foreach (var s in dash)
                {

                 if (s.dateS.Value.Month == i && s.dateS.Value.Year == year)
                {
                  count++;
                }

                }
                inquiryPerMonth.Add(count);
            }
            ViewBag.InquiryData = inquiryPerMonth;


            //Months

            //YEARLYY
            var feed = GRM.tbl_Feedbacks.ToList();
            List<int> inquiryPerYear = new List<int>();
            int before = 0;
          
            foreach (var a in feed)
            {
                if(a.dateS.Value.Year != before) { 
                inquiryPerYear.Add(a.dateS.Value.Year);
                }
                before = a.dateS.Value.Year;
            }
            ViewBag.yearly = inquiryPerYear;
            List<int> yearlyData = new List<int>();
            foreach (var b in inquiryPerYear)
            {
                int s = 0;
                foreach (var c in feed)
                   
                {
                    if(c.dateS.Value.Year == b)
                    {
                        s++;
                    }

                }
                yearlyData.Add(s);
            }

            ViewBag.yearlyData = yearlyData;

            //YEARLYY


            //QUARTERLY

         
            var quarterly = GRM.tbl_Feedbacks.ToList();
            List<int> inquiryPerQuarter = new List<int>();
            int quarterCount = 0;
            int countQuarter = 0;
            for (int i = 1; i <= 3; i++)
            {
                foreach (var s in quarterly)
                {
                    if (s.dateS.Value.Month == i && s.dateS.Value.Year == year)
                    {
                        countQuarter++;
                    }
                }
            }
           
            inquiryPerQuarter.Add(countQuarter);
            countQuarter = 0;
            for (int i = 4; i <= 6; i++)
            {
                foreach (var s in quarterly)
                {
                    if (s.dateS.Value.Month == i && s.dateS.Value.Year == year)
                    {
                        countQuarter++;
                    }
                }
            }
         
            inquiryPerQuarter.Add(countQuarter);
            countQuarter = 0;
            for (int i = 7; i <= 9; i++)
            {
                foreach (var s in quarterly)
                {
                    if (s.dateS.Value.Month == i && s.dateS.Value.Year == year)
                    {
                        countQuarter++;
                    }
                }
            }
          
            inquiryPerQuarter.Add(countQuarter);
            countQuarter = 0;
            for (int i = 10; i <= 12; i++)
            {
                foreach (var s in quarterly)
                {
                    if (s.dateS.Value.Month == i && s.dateS.Value.Year == year)
                    {
                        countQuarter++;
                    }
                }
            }

            inquiryPerQuarter.Add(countQuarter);


            ViewBag.inquiryPerQuarter = inquiryPerQuarter;
            //QUARTERLY





            // Months LINE
        
            if(branchManager == null || branchManager == "") { 
     
          var lineMonthly = GRM.tbl_OrderHistory.Where(a => a.isApproved == 1 ).ToList();
       


            List<double?> grossSalesMonthly = new List<double?>();
            for (int i = 1; i <= 12; i++)
            {
                double? count = 0;
                foreach (var s in lineMonthly)
                {

                    if (s.dateRegistered.Value.Month == i && s.dateRegistered.Value.Year == year)
                    {
                        count += s.totalPrice;
                    }

                }
                grossSalesMonthly.Add(count);
            }
            ViewBag.grossSalesMonthly = grossSalesMonthly;
            }else
            {

                var lineMonthly = GRM.tbl_OrderHistory.Where(a => a.isApproved == 1 &&  a.branch == branchManager).ToList();



                List<double?> grossSalesMonthly = new List<double?>();
                for (int i = 1; i <= 12; i++)
                {
                    double? count = 0;
                    foreach (var s in lineMonthly)
                    {

                        if (s.dateRegistered.Value.Month == i && s.dateRegistered.Value.Year == year)
                        {
                            count += s.totalPrice;
                        }

                    }
                    grossSalesMonthly.Add(count);
                }
                ViewBag.grossSalesMonthly = grossSalesMonthly;
            }

            //Months




            if (branchManager == null || branchManager == "")
            {
                var yearlyLine = GRM.tbl_OrderHistory.Where(a => a.isApproved == 1).ToList();
                List<int> salesLineYear = new List<int>();
                int beforeLines = 0;

                foreach (var a in yearlyLine)
                {
                    if (a.dateRegistered.Value.Year != beforeLines)
                    {
                        salesLineYear.Add(a.dateRegistered.Value.Year);
                    }
                    beforeLines = a.dateRegistered.Value.Year;
                }
                ViewBag.yearlyLine = salesLineYear;
                List<Double?> yearlyDataLines = new List<Double?>();
                foreach (var b in salesLineYear)
                {
                    double? s = 0;
                    foreach (var c in yearlyLine)

                    {
                        if (c.dateRegistered.Value.Year == b)
                        {
                            s += c.totalPrice;
                        }

                    }
                    yearlyDataLines.Add(s);
                }

                ViewBag.yearlyDataLine = yearlyDataLines;


            }
            else
            {
                var yearlyLine = GRM.tbl_OrderHistory.Where(a => a.isApproved == 1 && a.branch == branchManager).ToList();
                List<int> salesLineYear = new List<int>();
                int beforeLines = 0;

                foreach (var a in yearlyLine)
                {
                    if (a.dateRegistered.Value.Year != beforeLines)
                    {
                        salesLineYear.Add(a.dateRegistered.Value.Year);
                    }
                    beforeLines = a.dateRegistered.Value.Year;
                }
                ViewBag.yearlyLine = salesLineYear;
                List<Double?> yearlyDataLines = new List<Double?>();
                foreach (var b in salesLineYear)
                {
                    double? s = 0;
                    foreach (var c in yearlyLine)

                    {
                        if (c.dateRegistered.Value.Year == b)
                        {
                            s += c.totalPrice;
                        }

                    }
                    yearlyDataLines.Add(s);
                }

                ViewBag.yearlyDataLine = yearlyDataLines;


            }

            //YEARLYY LINE


            //YEARLYY LINE


            if (branchManager == null || branchManager == "")
            {
                var quarterlyLines = GRM.tbl_OrderHistory.Where(a => a.isApproved == 1).ToList();
                List<double?> inquiryPerQuarterLines = new List<double?>();
                int quarterCountLines = 0;
                double? countQuarterLines = 0;
                for (int i = 1; i <= 3; i++)
                {
                    foreach (var s in quarterlyLines)
                    {
                        if (s.dateRegistered.Value.Month == i && s.dateRegistered.Value.Year == year)
                        {
                            countQuarterLines += s.totalPrice;
                        }
                    }
                }

                inquiryPerQuarterLines.Add(countQuarterLines);
                countQuarterLines = 0;
                for (int i = 4; i <= 6; i++)
                {
                    foreach (var s in quarterlyLines)
                    {
                        if (s.dateRegistered.Value.Month == i && s.dateRegistered.Value.Year == year)
                        {
                            countQuarterLines += s.totalPrice;
                        }
                    }
                }

                inquiryPerQuarterLines.Add(countQuarterLines);
                countQuarterLines = 0;
                for (int i = 7; i <= 9; i++)
                {
                    foreach (var s in quarterlyLines)
                    {
                        if (s.dateRegistered.Value.Month == i && s.dateRegistered.Value.Year == year)
                        {
                            countQuarterLines += s.totalPrice;
                        }
                    }
                }

                inquiryPerQuarterLines.Add(countQuarterLines);
                countQuarterLines = 0;
                for (int i = 10; i <= 12; i++)
                {
                    foreach (var s in quarterlyLines)
                    {
                        if (s.dateRegistered.Value.Month == i && s.dateRegistered.Value.Year == year)
                        {
                            countQuarterLines += s.totalPrice;
                        }
                    }
                }

                inquiryPerQuarterLines.Add(countQuarterLines);


                ViewBag.inquiryPerQuarterLines = inquiryPerQuarterLines;
            }
            else
            {
                var quarterlyLines = GRM.tbl_OrderHistory.Where(a => a.isApproved == 1 && a.branch == branchManager).ToList();
                List<double?> inquiryPerQuarterLines = new List<double?>();
                int quarterCountLines = 0;
                double? countQuarterLines = 0;
                for (int i = 1; i <= 3; i++)
                {
                    foreach (var s in quarterlyLines)
                    {
                        if (s.dateRegistered.Value.Month == i && s.dateRegistered.Value.Year == year)
                        {
                            countQuarterLines += s.totalPrice;
                        }
                    }
                }

                inquiryPerQuarterLines.Add(countQuarterLines);
                countQuarterLines = 0;
                for (int i = 4; i <= 6; i++)
                {
                    foreach (var s in quarterlyLines)
                    {
                        if (s.dateRegistered.Value.Month == i && s.dateRegistered.Value.Year == year)
                        {
                            countQuarterLines += s.totalPrice;
                        }
                    }
                }

                inquiryPerQuarterLines.Add(countQuarterLines);
                countQuarterLines = 0;
                for (int i = 7; i <= 9; i++)
                {
                    foreach (var s in quarterlyLines)
                    {
                        if (s.dateRegistered.Value.Month == i && s.dateRegistered.Value.Year == year)
                        {
                            countQuarterLines += s.totalPrice;
                        }
                    }
                }

                inquiryPerQuarterLines.Add(countQuarterLines);
                countQuarterLines = 0;
                for (int i = 10; i <= 12; i++)
                {
                    foreach (var s in quarterlyLines)
                    {
                        if (s.dateRegistered.Value.Month == i && s.dateRegistered.Value.Year == year)
                        {
                            countQuarterLines += s.totalPrice;
                        }
                    }
                }

                inquiryPerQuarterLines.Add(countQuarterLines);


                ViewBag.inquiryPerQuarterLines = inquiryPerQuarterLines;

            }

            //QUARTERLY LINES

            //QUARTERLY

            if (branchManager == null || branchManager == "")
            {

                var dayDashLines = GRM.tbl_OrderHistory.Where(a => a.isApproved == 1).ToList();
                List<double?> inquiryPerDayLines = new List<double?>();
                List<int> inquiryPerDayLabelLines = new List<int>();
                double? countDayLines = 0;
                int yLines = Convert.ToInt32(year);
                int mLines = Convert.ToInt32(months);
                int daysInMonthLines = DateTime.DaysInMonth(yLines, mLines);

                for (int i = 1; i <= daysInMonthLines; i++)
                {
                    foreach (var das in dayDashLines)
                    {
                        if (das.dateRegistered.Value.Day == i && das.dateRegistered.Value.Year == year && das.dateRegistered.Value.Month == months)
                        {
                            countDayLines += das.totalPrice;
                        }
                    }
                    inquiryPerDayLabelLines.Add(i);
                    inquiryPerDayLines.Add(countDayLines);
                    countDayLines = 0;
                }
                ViewBag.inquiryPerDayLabelLines = inquiryPerDayLabelLines;
                ViewBag.inquiryPerDayLines = inquiryPerDayLines;
            }
            else
            {

                var dayDashLines = GRM.tbl_OrderHistory.Where(a => a.isApproved == 1 && a.branch == branchManager).ToList();
                List<double?> inquiryPerDayLines = new List<double?>();
                List<int> inquiryPerDayLabelLines = new List<int>();
                double? countDayLines = 0;
                int yLines = Convert.ToInt32(year);
                int mLines = Convert.ToInt32(months);
                int daysInMonthLines = DateTime.DaysInMonth(yLines, mLines);

                for (int i = 1; i <= daysInMonthLines; i++)
                {
                    foreach (var das in dayDashLines)
                    {
                        if (das.dateRegistered.Value.Day == i && das.dateRegistered.Value.Year == year && das.dateRegistered.Value.Month == months)
                        {
                            countDayLines += das.totalPrice;
                        }
                    }
                    inquiryPerDayLabelLines.Add(i);
                    inquiryPerDayLines.Add(countDayLines);
                    countDayLines = 0;
                }
                ViewBag.inquiryPerDayLabelLines = inquiryPerDayLabelLines;
                ViewBag.inquiryPerDayLines = inquiryPerDayLines;

            }
                //DAILYY
    
            //DAILYY
            
        



            ViewBag.data = data.ToList();
            // create a variable "user" & "userType "
            // Pass the current username session and userPosition
         
            string userType = Session["userPosition"].ToString();

            //create a variable getNotifMessage
            //Pass the  tbl_users table where the userName is equal to the user variable that we declare.

            var getNotifMessage = GRM.tbl_Users.SingleOrDefault(a => a.userName == user);
            //Create a new session and pass the number of notifications came from the tbl_users.
            Session["a1"] = getNotifMessage.messageNotif;

            var getNotifFeed = GRM.tbl_Notifications.SingleOrDefault(a => a._int == 2);
            Session["a2"] = getNotifFeed.messageNotif;

            //Do the code if the userType variable ( has a session value )is equal to manager.
            if(userType == "manager") {
                //Get the tbl_notifications value where int is equal to 3
                // Get the number of order notification and pass it to a Session["a3"] variable
                var getNotifOrders = GRM.tbl_Notifications.SingleOrDefault(a => a._int == 3);
                Session["a3"] = getNotifOrders.messageNotif;

             
            }else {
                // if the userType is not equal to "manager" ex. the current user is branch manager .. do this code below
                //We are getting the orderNotif value of tbl_users and pass it to the Session["a3"].
                //We are using tbl_Users Instead of tbl_notification because there are specific orders of every clients and branch manager
                var getOrders = GRM.tbl_Users.Where(a => a.userName == user).FirstOrDefault();
                Session["a3"] = getOrders.orderNotif;
            }
            //Return the View.. means return /Index.cshtml
            return View();
        }

        public ActionResult RegisterUser()
        {
            //For Registration of User and Clients..

            return View();

        }



        public ActionResult RegisterExecute(tbl_Users users,string operateSunday, string operateSaturday, string dtiOrSec)
        {
            
            if (users.branchManager == null && users.position == "client") {
                users.outputMessage = "Please add a branch manager first";
                return View("RegisterUser", users);
            }

            if (dtiOrSec == null)
            {
                users.outputMessage = "Please select if DTI OR SEC";
                return View("RegisterUser", users);
            }

            //Check if the password is validated. Password is equal to ReType Password.
            //If Password 1 is not  equal to Password 2
            // " != " is equal to not equal
            if (users.PassWord != users.confirmPassword)
            {
                users.outputMessage = "Password dont match";
                return View("RegisterUser", users);
            }

           
            //Check if the username or email is existing to the database.
            //Failed the registration if the username or email is salready existed
            var checkUserEmail = GRM.tbl_Users.Where(user => user.userName == users.userName || user.emailAddress == users.emailAddress).SingleOrDefault();
            if (checkUserEmail != null)
            {
                users.outputMessage = "Username or Email is already existed";
                return View("RegisterUser", users);
            }
            users.messageNotif = 0;
            users.orderNotif = 0;
            Console.WriteLine("dalaga" + operateSaturday);
            if(operateSunday != null)
            {
                users.sunday_operate = "Operational";
            }else
            {
                users.sunday_operate = null;
            }
            if ( operateSaturday != null)
            {
                users.saturday_operate = "Operational";
            }else
            {
                users.saturday_operate = null;
            }


            if (dtiOrSec != null)
            {
                users.dti_or_sec = dtiOrSec;
            }

            
          
            users.tries = 0;
            string encryptedpass = base64Encode(users.PassWord);
            users.PassWord = encryptedpass;
            
          
            //Add the users model to the database
            GRM.tbl_Users.Add(users);
            //Add the date today 
            users.dateRegistered = DateTime.Now;
            //Save the Database.
         

            if (users.branchManager != null)
            {
                //Get the data of the newly inserted account.
                var Test = GRM.tbl_Users.SingleOrDefault(a => a.userName == users.branchManager);
            
            }

            // Save the database

            // Your code...
            // Could also be before try if you know the exception occurs in SaveChanges
            try
            {
                // Output Registered to the Register Page if the data is inserted properly.
                //This block of code is for sending mail
               
                GMailer.GmailUsername = "grmthesis02@gmail.com";
                GMailer.GmailPassword = "ragbyboy09";
                GMailer mailer = new GMailer();
                // passing the email address of the newly registered user
                mailer.ToEmail = users.emailAddress;
                // Mail Subject
                mailer.Subject = "GRM Registration";
                //Mail Body
                mailer.Body += "Hi " + users.firstName + " " + users.lastName;
                mailer.Body += "<br> You are now part of GRM. Thank you for your trust";
                mailer.IsHtml = true;
                mailer.Send();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
               
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Please enter a valid email address</h3><br><p></p>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/RegisterUser', '_self'); }});    });</script>     ");

            }
            GRM.SaveChanges();



            return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Successfully Registered</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/RegisterUser', '_self'); }});    });</script>     ");

        }

        public ActionResult RegisterClient()
        {


            return View();
        }
        public ActionResult RenewAccount(int? id,int? renewz)
        {
            var account = GRM.tbl_Users.Where(a => a.userId == id).FirstOrDefault();
            account.dateRegistered = DateTime.Now;
            account.contract_duration = renewz;
            GRM.SaveChanges();
            return Content("<script>window.open('../Admin/ListOfUsers','_self')</script>");
        }
        public ActionResult Logout()
        {
            // Logout the users
            Session.Remove("cart");
            Session["userName"] = null;
            Session["userPosition"] = null;
            Session["userId"] = null;

            return View("~/Views/Home/Login.cshtml");

        }
        //Create a static class for dropdown items
        public static List<SelectListItem> GetDropDown()
        {
            //Declare a new database because we are in a static attributes.
            GRMDB GRM = new GRMDB();
            //Instantiate a new list named ls
            List<SelectListItem> ls = new List<SelectListItem>();
            //Create a new Variable to select the product catgeories from the database
          var myUser =  System.Web.HttpContext.Current.Session["userName"].ToString();
           var selectCat = GRM.tbl_ProductCategories.Where(a => a.catsBy == myUser).ToList();
            //Create a foreach to loop on each categories from the database
            // temp will be the variable for each selection on category database (in selectCat)  
            foreach (var temp in selectCat)
            {
                //ls = list // Create a new selectListItem and add the value corresponding to the category database.
                //Text will be = temp.name which is the name of category and the value will be temp.categoryId.ToString();
                //The value of the dropdownlist will be the categoryId converted to string because dropdown list won't accept an integer.
                ls.Add(new SelectListItem() { Text = temp.name, Value = temp.categoryId.ToString() });
            }
            //Return ls means send the data whenever we want it.
            return ls;
        }

        public ActionResult ResetTries(int id) {
            var acct = GRM.tbl_Users.SingleOrDefault(a => a.userId == id);
            acct.tries = 0;
            GRM.SaveChanges();
            return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Account Reset Successfully</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/ListOfUsers', '_self'); }});    });</script>     ");

        }

        //This method will create, edit, delete product and categories.
        public ActionResult Products()
        {



            return View();
        }
        
   public ActionResult RawSales(string m_year, string m_month, string m_branch, string m_type)
        {
            if(m_type == "sales_daily")
            {
                var salex = GRM.tbl_OrderHistory.ToList();
                List<RawSales> sale = new List<RawSales>();

                foreach(var x in salex)
                {
                    if(x.dateRegistered.Value.Month.ToString() == m_month)
                    {
                        RawSales rs = new RawSales();
                        var getName = GRM.tbl_Users.FirstOrDefault(a => a.userId == x.clientId);
                        rs.client = getName.userName;
                        rs.orderDate = x.dateRegistered;
                        rs.totalSales = x.totalPrice;
                        rs.orderId = Convert.ToInt32(x.orderId) ;
                        sale.Add(rs);
                    }

                }
                ViewBag.Salez = sale;
            }
            else if (m_type == "sales_monthly")
            {
                var salex = GRM.tbl_OrderHistory.ToList();
                List<RawSales> sale = new List<RawSales>();

                foreach (var x in salex)
                {
                    if (x.dateRegistered.Value.Year.ToString() == m_year)
                    {
                        RawSales rs = new RawSales();
                        var getName = GRM.tbl_Users.FirstOrDefault(a => a.userId == x.clientId);
                        rs.client = getName.userName;
                        rs.orderDate = x.dateRegistered;
                        rs.totalSales = x.totalPrice;
                        rs.orderId = Convert.ToInt32(x.orderId);
                        sale.Add(rs);
                    }

                }
                ViewBag.Salez = sale;
            }else
            {
                var salex = GRM.tbl_OrderHistory.ToList();
                List<RawSales> sale = new List<RawSales>();

                foreach (var x in salex)
                {
                
                        RawSales rs = new RawSales();
                        var getName = GRM.tbl_Users.FirstOrDefault(a => a.userId == x.clientId);
                        rs.client = getName.userName;
                        rs.orderDate = x.dateRegistered;
                        rs.totalSales = x.totalPrice;
                        rs.orderId = Convert.ToInt32(x.orderId);
                        sale.Add(rs);
                   

                }
                ViewBag.Salez = sale;
            }

            return View();
        }

        public ActionResult RawInquiries(string p_year, string p_month, string p_branch, string p_type)
        {
          if(p_type == "inq_monthly") {
            var dash = GRM.tbl_Feedbacks.ToList();
            List<RawInquiries> inq = new List<RawInquiries>();
            foreach (var x in dash)
            {
                if(x.dateS.Value.Year.ToString() == p_year) { 
                RawInquiries inx = new RawInquiries();
                inx.date_inq = x.dateS;
                inx.inquiries = x.message;
                inx.name = x.name;
                inq.Add(inx);
                }
            }
            ViewBag.InquiryData = inq;
            }
            else if (p_type == "inq_daily")
            {
                var dash = GRM.tbl_Feedbacks.ToList();
                List<RawInquiries> inq = new List<RawInquiries>();
                foreach (var x in dash)
                {
                    if (x.dateS.Value.Month.ToString() == p_month)
                    {
                        RawInquiries inx = new RawInquiries();
                        inx.date_inq = x.dateS;
                        inx.inquiries = x.message;
                        inx.name = x.name;
                        inq.Add(inx);
                    }
                }

                ViewBag.InquiryData = inq;

            }
            else
            {
                var dash = GRM.tbl_Feedbacks.ToList();
                List<RawInquiries> inq = new List<RawInquiries>();
                foreach (var x in dash)
                {
                 
                        RawInquiries inx = new RawInquiries();
                        inx.date_inq = x.dateS;
                        inx.inquiries = x.message;
                        inx.name = x.name;
                        inq.Add(inx);
                   
                }

                ViewBag.InquiryData = inq;
            }

            return View();
        }
        public ActionResult RawData()
        {
            var eachUser = GRM.tbl_Users.Where(a => a.position == "branchmanager").ToList();
            var totalSales = GRM.tbl_OrderHistory.ToList();
           List <RawModel> model = new List<RawModel>();
           
            foreach (var each in eachUser) {
                double? totalSale = 0;
                foreach (var sales in totalSales)
                {
                    var getBM = GRM.tbl_Users.FirstOrDefault(a => a.userId == sales.clientId);
                    if (getBM.branchManager == each.userName)
                        totalSale += sales.totalPrice;
                }
                var normalModel = new RawModel();
                normalModel.user = each.userName;
                normalModel.totalSale = (double)totalSale;
               
                model.Add(normalModel);



            }


            ViewBag.modelFinal = model;



          

            //OTHER 







            return View();
        }

        [HttpPost]
        [ValidateInput(false)]

        public ActionResult AddProduct(string prodName, double? productPrice, string productDesc, HttpPostedFileBase prodImage, int productCategory)
        {
            //This code is for adding new product.


            string myUser = Session["userName"].ToString();
            //Instantiate a tbl_Products named product
            tbl_Products product = new tbl_Products();
            
            var selectAllProduct = GRM.tbl_Products.Where(a => a.productName == prodName && a.packageBy == myUser).FirstOrDefault();
            if (selectAllProduct != null)
            {
                ViewBag.errorMessage = "The name is already taken";
                return Content("false");

            }
            if (prodName == "") {
                return Content("noname");
            }
            if (productPrice == null)
            {
                return Content("noprice");
            }
            if (productPrice < 10)
            {
                return Content("priceten");
            }
            if (productDesc == "")
            {
                return Content("noDesc");
            }
            int? sss = productCategory;
            if (sss == null)
            {
                return Content("noCat");
            }
            //Product Image

            if (prodImage != null && prodImage.ContentLength > 0)
            {

                var fileName = Path.GetFileName(prodImage.FileName);
                prodImage.SaveAs(HttpContext.Server.MapPath("~/Content/") + prodImage.FileName);
                product.productImage = prodImage.FileName;
            }

          //assign a value in every column of the product table.
            product.dateRegistered = DateTime.Now;
            product.productName = prodName;
            product.productPrice = productPrice;
            product.productDescription = productDesc;
            product.categoryId = productCategory;
            product.packageBy = Session["userName"].ToString();
            //Add the newly created rows to the product table
            GRM.tbl_Products.Add(product);
            // Save the changes in the database
            GRM.SaveChanges();
           // ViewBag.successMessage = "Product Added";
            return Content("added");


        }


        public ActionResult ListProductsAndCategories(string search, int? i, string branchManager)
        {

           


            // This code is for listing products.
            var productList = GRM.tbl_Products.ToList();
            string myUser = Session["userName"].ToString();

            var getProducts = GRM.tbl_Products.ToList();
            var getBm = GRM.tbl_Users.Where(a => a.position == "branchmanager").ToList();
            List<tbl_Products> proxx = new List<tbl_Products>();
            foreach (var bm in getBm) {
                var product = new tbl_Products();
                int? boughts = 0;
            foreach (var prod in getProducts)
            {
                    if (bm.userName == prod.packageBy && prod.bought >= boughts) {
                        boughts = prod.bought;
                        product.bought = boughts;
                        product.productPrice = prod.productPrice;
                        product.packageBy = bm.userName;
                        product.productName = prod.productName;
                        product.productImage = prod.productImage;
                       
                    }
                }
             proxx.Add(product);
            }
            ViewBag.besters = proxx;
         

            var categoryList = GRM.tbl_ProductCategories.Where(a => a.catsBy == myUser).ToList();
            if (Session["userPosition"].ToString() == "manager") {
                
               productList = GRM.tbl_Products.Where(a => a.categoryId != 0 ).ToList();
                 categoryList = GRM.tbl_ProductCategories.Where(a => a.catsBy == myUser ).ToList();
                ViewBag.productList = productList.OrderByDescending(a => a.productId);
                ViewBag.categoryList = categoryList;

              
                return View();
            }
            else
            {
                 categoryList = GRM.tbl_ProductCategories.Where(a => a.catsBy == myUser ).ToList();
                productList = GRM.tbl_Products.Where(a => a.categoryId != 0 && a.packageBy == myUser ).ToList();
                ViewBag.productList = productList.OrderByDescending(a => a.productId);
                if (categoryList.Count == 0)
                {
                    return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Please add a category first</h3><br><p></p>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/Categories', '_self'); }});    });</script>     ");

                }
                ViewBag.categoryList = categoryList;
                return View();
            }
        }
        
       public ActionResult GetCredit(string user)
        {

            var getCredit = GRM.tbl_Users.FirstOrDefault(a => a.userName == user);
            if(getCredit.payment_period == null || getCredit.credit_added == null)
            {
                string format = string.Format("{0:C}", getCredit.credits);
                return Content("<h4> " + format + "</h4> <br> Available to add new contract");
            }
            else {
                string format = string.Format("{0:C}",getCredit.credits);
            return Content("<h4> "+ format + "</h4><br><p>Expiration : "+ getCredit.credit_added.Value.AddMonths(getCredit.payment_period.Value) +" </p>" );

            }
        }
        public ActionResult GetTheList(int?[] selectedProd)
        {
            //THis code block of code is for adding a package.
            // There is a parameter of selectProd Above.
            // selectProd is passing the number of clicked checkbox on the product page
            // and giving the value here to insert the products and convert it to package
            if (Request.Form["package"] != null)
            {
                if (selectedProd == null)
                {
                    return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Please Select A Product</h3><br><p></p>',icon: $.sweetModal.ICON_ERROR,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/ListProductsAndCategories', '_self'); }});    });</script>     ");


                }

                var getLatestId = GRM.tbl_Products.OrderByDescending(a => a.productId).FirstOrDefault();
                var newId = getLatestId.productId;
                newId++;
                string listOfItems = "";
                double? totalPrice = 0;
                foreach (var prod in selectedProd)
                {

                    var selectedItem = GRM.tbl_Products.SingleOrDefault(a => a.productId == prod);

                    tbl_PackageItems pkg = new tbl_PackageItems();

                    pkg.item = selectedItem.productName;
                    pkg.packageId = newId;
                    pkg.price = selectedItem.productPrice;
                    pkg.quantity = 1;
                    GRM.tbl_PackageItems.Add(pkg);
                    GRM.SaveChanges();

                    listOfItems += selectedItem.productName + " X " + selectedItem.productPrice + " ";
                    totalPrice += selectedItem.productPrice;
                }

                tbl_Products product = new tbl_Products();

                product.packageBy = Session["userName"].ToString();
                product.dateRegistered = DateTime.Now;
                product.productPrice = totalPrice;
                product.productDescription = listOfItems;
                product.productName = "New Package";
                GRM.tbl_Products.Add(product);
                GRM.SaveChanges();



                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>New Package Added</h3><br><p>You can check and edit new package on the package tab.</p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/ListProductsAndCategories', '_self'); }});    });</script>     ");

                //Write your code here
            }
            //This code is for multi deleting Products
            else if (Request.Form["delete"] != null)
            {
                if (selectedProd == null)
                {
                    return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Please check the item that you want to delete</h3><br><p></p>',icon: $.sweetModal.ICON_ERROR,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/ListProductsAndCategories', '_self'); }});    });</script>     ");

                }

                //We declare a foreach here to get the id of each product in selectProd
                foreach (var prod in selectedProd)
                {
                    //the prod variable has an ID of every product in selectProd
                    //so every loop we will delete each row in tbl_Products by getting the id of specific product.
                    var delete = GRM.tbl_Products.SingleOrDefault(a => a.productId == prod);



                    delete.isArchive = "yes";

                    GRM.SaveChanges();

                }


                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>The product has been successfully archived.</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/ListProductsAndCategories', '_self'); }});    });</script>     ");

                //Write your code here
            } else if (Request.Form["recoverProduct"] != null) {

                if (selectedProd == null)
                {
                    return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Please check the item that you want to delete</h3><br><p></p>',icon: $.sweetModal.ICON_ERROR,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/ListProductsAndCategories', '_self'); }});    });</script>     ");

                }

                //We declare a foreach here to get the id of each product in selectProd
                foreach (var prod in selectedProd)
                {
                    //the prod variable has an ID of every product in selectProd
                    //so every loop we will delete each row in tbl_Products by getting the id of specific product.
                    var delete = GRM.tbl_Products.SingleOrDefault(a => a.productId == prod);



                    delete.isArchive = null;

                    GRM.SaveChanges();

                }


                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>The product has been successfully archived.</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/ListProductsAndCategories', '_self'); }});    });</script>     ");


            }
            else if (Request.Form["recoverPackage"] != null)
            {

                if (selectedProd == null)
                {
                    return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Please check the item that you want to delete</h3><br><p></p>',icon: $.sweetModal.ICON_ERROR,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/Package', '_self'); }});    });</script>     ");

                }

                //We declare a foreach here to get the id of each product in selectProd
                foreach (var prod in selectedProd)
                {
                    //the prod variable has an ID of every product in selectProd
                    //so every loop we will delete each row in tbl_Products by getting the id of specific product.
                    var delete = GRM.tbl_Products.SingleOrDefault(a => a.productId == prod);



                    delete.isArchive = null;

                    GRM.SaveChanges();

                }


                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>The product has been successfully unarchived.</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/Package', '_self'); }});    });</script>     ");


            }
            //This code is for multi deleting Package
            else if (Request.Form["deletePackage"] != null)
            {
                if (selectedProd == null) {
                    return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Please check the item that you want to delete</h3><br><p></p>',icon: $.sweetModal.ICON_ERROR,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/Package', '_self'); }});    });</script>     ");

                }
                foreach (var prod in selectedProd)
                {
                    var delete = GRM.tbl_Products.SingleOrDefault(a => a.productId == prod);
                    delete.isArchive = "yes";
                    GRM.SaveChanges();

                }
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>The package has been successfully archived.</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/Package', '_self'); }});    });</script>     ");

            }

            //This line of code is for deleting a user.
            else if (Request.Form["deleteUser"] != null && selectedProd != null)
            {
                if (selectedProd == null)
                {
                    return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Please check the user that you want to delete</h3><br><p></p>',icon: $.sweetModal.ICON_ERROR,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/ListProductsAndCategories', '_self'); }});    });</script>     ");

                }

                foreach (var prod in selectedProd)
                {
                    var delete = GRM.tbl_Users.SingleOrDefault(a => a.userId == prod);
                    //If the userName is equals to admin
                    if (delete.userName == "admin")
                    {
                        //Return because admin account must not deletable.
                        return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>You can't delete an admin account</h3><br><p></p>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/ListProductsAndCategories', '_self'); }});    });</script>     ");

                    }
                    //If the userName is not admin = Proceed and delete the user
                    delete.isLocked = "yes";
                    //Save changes
                    GRM.SaveChanges();

                }
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>The user has been successfully removed.</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/ListOfUsers', '_self'); }});    });</script>     ");

            }


            //Write your code here



            return RedirectToAction("ListProductsAndCategories");
        }


        public ActionResult HomepageEdit()
        {

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult HomePageExecute(PageContent content, HttpPostedFileBase image)
        {
            //Do a try and catch code in case the program fails we can catch and alert the user and redirect the page.
            try
            {
                //create a variable and pass the PageContents where location is equal to the content selected . ex "Services 1
                var selectContent = GRM.PageContents.Where(a => a.location == content.location).SingleOrDefault();
                selectContent.contents = content.contents;
                //If we uplaod image insert an image instead of the message we type on the textarea.
                if (image != null && image.ContentLength > 0)
                {
                    //Create a variable and pass the filename of the image.
                    var fileName = Path.GetFileName(image.FileName);
                    //Put the image on the Content/ Folder
                    image.SaveAs(HttpContext.Server.MapPath("~/Content/") + image.FileName);
                  
                    // product.product_model.productImage = productImage.FileName;
                    selectContent.contents = "<img src='../../Content/" + fileName + "' width = '100%' height='300px' />";
                }
                //Save changes
                GRM.SaveChanges();
            }
            catch (Exception e) {
                //Redirect the page
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Please Write Something</h3><br><p></p>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/HomePageEdit', '_self'); }});    });</script>     ");

               
            }
            return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Successfully Edited</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/HomePageEdit', '_self'); }});    });</script>     ");

        }

        public ActionResult ShowMe(int num)
        {


            var selectMe = GRM.PageContents.SingleOrDefault(a => a.location == num);

            return Json(selectMe.contents.ToString(), JsonRequestBehavior.AllowGet);

            // return Content("<script>$('.texta').html('"+selectMe.contents+"')</script>");

        }

        public string base64Encode(string data)
        {
            try
            {
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Encode" + e.Message);
            }
        }

    
        public ActionResult News()
        {

            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
      
        public ActionResult News(tbl_News news,  HttpPostedFileBase featureImage)
        {

            //This code is for adding news
            try
            {
                if (featureImage != null && featureImage.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(featureImage.FileName);
                    featureImage.SaveAs(HttpContext.Server.MapPath("~/Content/") + featureImage.FileName);
                    // product.product_model.productImage = productImage.FileName;
                    news.featureImage = fileName;
                    Debug.WriteLine(fileName);
                }

                news.datapublish = DateTime.Now;
                GRM.tbl_News.Add(news);

                GRM.SaveChanges();
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>News Successfully Added</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/NewsControl', '_self'); }});    });</script>     ");
            }
            catch (Exception e) {
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Please Write Something</h3><br><p></p>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/News', '_self'); }});    });</script>     ");

            }
            return View();
        }

        public ActionResult NewsControl(string search, int? i)
        {

            var getAllNews = GRM.tbl_News.ToList();
            ViewBag.newsList = getAllNews;

            return View();

        }

        public ActionResult Package(int? i, string search )
        {
            //This code is for showing package
            string myUser = Session["userName"].ToString();
            //create a variable packagelist and past the products table
            var packageList = GRM.tbl_Products.ToList();
            // if the Position is equal to manager do the code below.
            if (Session["userPosition"].ToString() == "manager")
            {
                //Output all products if the position is manager.
                //Sort by descending using the product id
                //Only output rows where category Id is equal to 0
                 packageList = GRM.tbl_Products.Where(a => a.categoryId == 0).OrderByDescending(a => a.productId).ToList();
                //a ViewBag is a temporary variable. .packageList is the variable name that we declare.
                //pass the packageList variable that we created at the top to the ViewBag the we created below.
                ViewBag.packageList = packageList;
                //ViewBag.packageList is different from packageList.
                return View();
            }
            //If the position is not manager do the code below.
            else
            {
                //Output products if the caregory is equal to 0 and the column packageBy must equal to the current user
                // We do this because in branchManager we are only showing his/her client's products/packages
                 packageList = GRM.tbl_Products.Where(a => a.categoryId == 0 && a.packageBy == myUser).OrderByDescending(a => a.productId).ToList();
                ViewBag.packageList = packageList;
                return View();

            }


        }
        public ActionResult PackageEdit(int id)
        {

            //This code is for editing a package
            var package = GRM.tbl_Products.SingleOrDefault(a => a.productId == id);
            // pass the package object to the ViewBag.package
            //Package is a single row only because we use SingleOrDefault.. Unlike in other we used .ToList();
            ViewBag.package = package;
            ViewBag.packageId = id;

            return View();
        }

        
        [HttpPost]
        //This code is for editing a package
        public ActionResult EditPackage(tbl_Products product, HttpPostedFileBase productImage)
        {
            var selectAllProduct = GRM.tbl_Products.SingleOrDefault(a => a.productId == product.productId);


            //Product Image

            if (productImage != null && productImage.ContentLength > 0)
            {

                var fileName = Path.GetFileName(productImage.FileName);
                productImage.SaveAs(HttpContext.Server.MapPath("~/Content/") + productImage.FileName);
                // product.product_model.productImage = productImage.FileName;
                selectAllProduct.productImage = fileName;
            }

            product.dateRegistered = DateTime.Now;
            selectAllProduct.productName = product.productName;
            selectAllProduct.productDescription = product.productDescription;
            selectAllProduct.productPrice = product.productPrice;
            //  GRM.tbl_Products.Add(product.product_model);
            GRM.SaveChanges();
            //  product.successMessage = "Package Edited";
            return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>The package has been updated</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/Package', '_self'); }});    });</script>     ");

        }

        
        [HttpPost]
        //This code is for editing a product
        public ActionResult EditProduct(tbl_Products product, HttpPostedFileBase productImage)
        {
            var selectAllProduct = GRM.tbl_Products.SingleOrDefault(a => a.productId == product.productId);


            //Product Image

            if (productImage != null && productImage.ContentLength > 0)
            {

                var fileName = Path.GetFileName(productImage.FileName);
                productImage.SaveAs(HttpContext.Server.MapPath("~/Content/") + productImage.FileName);
                // product.product_model.productImage = productImage.FileName;
                selectAllProduct.productImage = fileName;
                Debug.WriteLine(fileName);
            }


            product.dateRegistered = DateTime.Now;
            selectAllProduct.productName = product.productName;
            selectAllProduct.categoryId = product.categoryId;
            selectAllProduct.productDescription = product.productDescription;
            selectAllProduct.productPrice = product.productPrice;
            //  GRM.tbl_Products.Add(product.product_model);
            GRM.SaveChanges();
            //  product.successMessage = "Package Edited";
            return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>The product has been updated</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/ListProductsAndCategories', '_self'); }});    });</script>     ");

          

        }
        public ActionResult EditPackageQuantities(int id)
        {
            var selectPackages = GRM.tbl_PackageItems.Where(a => a.packageId == id).ToList();
            ViewBag.selectedPackages = selectPackages;

            return View();
        }

        //This code is for editing a package quantity and price.
        public ActionResult EditPackageQuantity(int[] id, int[] quantity, int packId, int[] packPrice)
        {
            //create a float Total Price.
            float totalPrice = 0;
            //create a for loop i = 0(starting point) id.Count() means number of products inside a package. i++ means 1 or loop 
            for (int i = 0; i < id.Count(); i++)
            {
                int idMe = id[i];
                int quan = quantity[i];
                int price = packPrice[i];
                var editPackageQuantity = GRM.tbl_PackageItems.SingleOrDefault(a => a.id == idMe);
                editPackageQuantity.quantity = quan;
                editPackageQuantity.price = price;


                totalPrice += price * quan;

                GRM.SaveChanges();

            }
            var getProducts = GRM.tbl_Products.SingleOrDefault(a => a.productId == packId);

            getProducts.productPrice = totalPrice;
            GRM.SaveChanges();
            return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>The package quantities has been updated</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/Package', '_self'); }});    });</script>     ");

        }

        //This row is for outputing clients order
        public ActionResult ClientsOrder(string searched, int? i)
        {
            string getUser = Session["userName"].ToString();
            string getBranchManager = Session["userPosition"].ToString();
            if(getBranchManager != "manager") { 
            var getNotifMessage = GRM.tbl_Users.SingleOrDefault(a => a.userName == getUser);
            getNotifMessage.orderNotif = 0;
            }else { 
            var getNotifMessage = GRM.tbl_Notifications.SingleOrDefault(a => a._int == 3);
            getNotifMessage.messageNotif = 0;
            }
            GRM.SaveChanges();
            Session["a3"] = 0;
          
            if (getBranchManager != "manager")
            {
                try
                {
                    string brManager = Session["userName"].ToString();
                    var selectClient = GRM.tbl_Users.Where(a => a.branchManager == brManager).ToList();
                    List<tbl_OrderHistory> listOfHistory = new List<tbl_OrderHistory>();
                  
                    foreach (var secCli in selectClient)
                    {


                        var getOrders = GRM.tbl_OrderHistory.Where(a => a.clientId == secCli.userId).ToList().OrderByDescending(a => a.orderId);
                        foreach (var ord in getOrders)
                        {
                            var orderHis = new tbl_OrderHistory();
                            orderHis.orderId = ord.orderId;
                            orderHis.totalPrice = ord.totalPrice;
                            orderHis.deliveryDate = ord.deliveryDate;
                            orderHis.dateRegistered = ord.dateRegistered;
                            listOfHistory.Add(orderHis);
                        }
                        
                        
                    }
                    ViewBag.orderHistory = listOfHistory;
                }
                catch (Exception e) {
                    return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Please link your account to a customer.</h3><br><p></p>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/ListProductsAndCategories', '_self'); }});    });</script>     ");

                }
            }
            else {
                var getOrders = GRM.tbl_OrderHistory.OrderByDescending(a => a.orderId).ToList();
                ViewBag.orderHistory = getOrders;

            }

       
            return View();


        }
        //This code is for triggering a pending payment to complete payment
 
        public ActionResult CompletePayment(string id)
        {

            var selectOrder = GRM.tbl_OrderHistory.FirstOrDefault(a => a.orderId == id);
            selectOrder.isApproved = 1;
            var getBM = GRM.tbl_Users.FirstOrDefault(a => a.userId == selectOrder.clientId);
            selectOrder.branch = getBM.userName;
            GRM.SaveChanges();
            return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Payment successfully.</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../../Admin/ClientsOrder', '_self'); }});    });</script>     ");

        }
        //This code is for denying payment
        public ActionResult DenyPayment(string id)
        {
            var selectOrder = GRM.tbl_OrderHistory.FirstOrDefault(a => a.orderId == id);

            selectOrder.isApproved = 5;
            GRM.SaveChanges();

            return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>The payment has been denied.</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../../Admin/ClientsOrder', '_self'); }});    });</script>     ");

        }
        public ActionResult GetTheDetails(int num)
        {


            var selectMe = GRM.tbl_Products.SingleOrDefault(a => a.productId == num);

            return Json(selectMe.productDescription.ToString(), JsonRequestBehavior.AllowGet);

            // return Content("<script>$('.texta').html('"+selectMe.contents+"')</script>");

        }
        //This table is for Product Edit
        //We are passing the existing value of the product in the textbox so we can just edit it.
        public ActionResult ProductEdit(int id)
        {
            var product = GRM.tbl_Products.SingleOrDefault(a => a.productId == id);
            ViewBag.package = product;
            ViewBag.packageId = id;

            return View();
        }

        public ActionResult Credits() {
            var cred = GRM.tbl_Users.Where(a => a.position == "client").ToList();
            ViewBag.users = cred;
            return View();
        }

        public ActionResult AddCredit(string users,int number,int? month) {
        
            var user = GRM.tbl_Users.FirstOrDefault(a => a.userName == users);
                if (user.credits == null)
                {
                    user.credits = 0;
                }
                user.credits += number;
            int payPeriod = 0;
            if (user.payment_period == null)
                payPeriod = 0;
            else
                payPeriod = (int)user.payment_period;

            if (user.credit_added == null)
            {
                user.credit_added = DateTime.Now;
            }

            if (user.credit_added.Value.AddMonths(payPeriod) > DateTime.Now)
                {
                TimeSpan ts = user.credit_added.Value.AddMonths(payPeriod) - DateTime.Now;
                    return Content("Your contract will expire on " + user.credit_added.Value.AddMonths(payPeriod) + ". Remaining " + ts.TotalDays + " days" );
                }else { 
                    user.payment_period = month;
                }
               
                GRM.SaveChanges();
                return Content("success");
          

            
        }
        //A static list for showing the list of branchManager
        public static List<SelectListItem> BranchManagerDropDown()
        {
            List<SelectListItem> ls = new List<SelectListItem>();
            GRMDB grms = new GRMDB();

            var userList = grms.tbl_Users.Where(a => a.position == "branchmanager" && a.branchManager == null).ToList();


            foreach (var temp in userList)
            {
                ls.Add(new SelectListItem() { Text = temp.userName, Value = temp.userName });
            }
            return ls;
        }
        //Create a list of users
        public ActionResult ListOfUsers(string search, int? i)
        {

   
            var getUsers = GRM.tbl_Users.Where(a => a.branchManager != "manager").ToList();
            ViewBag.userModel = getUsers;

            return View();


        }
        public ActionResult RawBestSeller(string branchManager)
        {
            if (branchManager == null || branchManager == "")
            {
                List<BestSellers> bestList = new List<BestSellers>();
                var getProduct = GRM.tbl_Products.ToList();
                var getList = GRM.tbl_OrderList.ToList();
                foreach (var prod in getProduct)
                {
                    BestSellers best = new BestSellers();
                    best.numberOfSales = 0;
                    foreach (var list in getList)
                    {
                        if (prod.productId == list.packageId)
                            best.numberOfSales += list.quantity;
                        Console.WriteLine(list.quantity);
                    }

                    best.product = prod.productName;
                    best.branch = prod.packageBy;
                    bestList.Add(best);


                }
                ViewBag.bestList = bestList.OrderByDescending(a => a.numberOfSales);
            }
            else
            {
                List<BestSellers> bestList = new List<BestSellers>();
                var getProduct = GRM.tbl_Products.Where(a => a.packageBy == branchManager).ToList();
                var getList = GRM.tbl_OrderList.ToList();
                foreach (var prod in getProduct)
                {
                    BestSellers best = new BestSellers();
                    best.numberOfSales = 0;
                    foreach (var list in getList)
                    {
                        if (prod.productId == list.packageId)
                            best.numberOfSales += list.quantity;
                        Console.WriteLine(list.quantity);
                    }

                    best.product = prod.productName;
                    best.branch = prod.packageBy;
                    bestList.Add(best);


                }
                ViewBag.bestList = bestList.OrderByDescending(a => a.numberOfSales);

            }

            return View();
        }
        //This code is for editing users
        //We are also passinsg the existing value of users to the textboxs
        public ActionResult EditUser(int id, tbl_Users users)
        {
            var GetData = GRM.tbl_Users.SingleOrDefault(a => a.userId == id);
            ViewBag.userModel = GetData;

            if (users.userName != null)
            {

                //GetData.branchManager = users.userName;

            }

            return View();
        }
        //This code is for passing the data from form (textbox, textarea) to the database tables
        [HttpPost]
        public ActionResult EditNow(tbl_Users users, int myId)
        {
            var GetData = GRM.tbl_Users.FirstOrDefault(a => a.userId == myId);
            ViewBag.userModel = GetData;
            GetData.firstName = users.firstName;
            GetData.lastName = users.lastName;
            GetData.PassWord = base64Encode(users.PassWord);
            GetData.emailAddress = users.emailAddress;
            GetData.homeAddress =  users.homeAddress;
            GetData.gender = users.gender;
         
            GetData.contactnumber = users.contactnumber;
            GRM.SaveChanges();

            return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>User has been updated.</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../../Admin/ListOfUsers', '_self'); }});    });</script>     ");


        }



        //This code is for showing feedbacks
        public ActionResult FeedbackShow(string search, int? i, int? readable, string reads,string email)
        {
        
            var getNotifMessage = GRM.tbl_Notifications.SingleOrDefault(a => a._int == 2);
            getNotifMessage.messageNotif = 0;
            GRM.SaveChanges();
            Session["a2"] = 0;
            if (Request.Form["delete"] != null)
            {

                var searchMsg = GRM.tbl_Feedbacks.SingleOrDefault(a => a.id == readable);
                GRM.tbl_Feedbacks.Remove(searchMsg);
                GRM.SaveChanges();

            }

            if (readable != null && Request.Form["read"] != null)
            {

                string user = Session["userName"].ToString();
                var searchMsg = GRM.tbl_Feedbacks.SingleOrDefault(a => a.id == readable);

                ViewBag.Msg = searchMsg;
                Debug.WriteLine(searchMsg);
            }


            if (email != null && Request.Form["reply"] != null)
            {
              
                string user = Session["userName"].ToString();
                var searchMsg = GRM.tbl_Feedbacks.SingleOrDefault(a => a.id == readable);

                ViewBag.Msg = searchMsg;
                Debug.WriteLine(searchMsg);
            }
          
            var currentUser = System.Web.HttpContext.Current.Session["userName"].ToString();


            var getAllMessage = GRM.tbl_Feedbacks.ToList();


            ViewBag.messageList = getAllMessage.OrderByDescending(a => a.id);

            return View();

        }
        [ValidateInput(false)]
        public ActionResult SendMessage(string myUsers, string myMessages)
        {

            if (myMessages == "") {
                return Content("empty");
            }
            var sendMess = GRM.tbl_Users.FirstOrDefault(a => a.userName == myUsers);
            sendMess.messageNotif += 1;

            tbl_Message newMessage = new tbl_Message();

            newMessage.sent = Session["userName"].ToString();
            newMessage.receive = myUsers;
            newMessage.message = myMessages;
            newMessage.dateSent = DateTime.Now;
            GRM.tbl_Message.Add(newMessage);
            GRM.SaveChanges();
            return Content("sent");
        }



        public ActionResult Message(string search, int? i, int? readable, string reads)
        {

            var currentUser = System.Web.HttpContext.Current.Session["userName"].ToString();
            var getMe = GRM.tbl_Users.FirstOrDefault(a => a.userName == currentUser);
            getMe.messageNotif = 0;
            GRM.SaveChanges();

            GRM.SaveChanges();
            Session["a1"] = 0;
            if (Request.Form["delete"] != null)
            {

                var searchMsg = GRM.tbl_Message.Where(a => a.sent == reads).ToList();
                foreach (var c in searchMsg) {
                    GRM.tbl_Message.Remove(c);
                    GRM.SaveChanges();
                }
              

            }

            if (readable != null && Request.Form["read"] != null)
            {
                string user = Session["userName"].ToString();
                var searchMsg = GRM.tbl_Message.Where(a => a.receive == user && a.sent == reads || a.receive == reads && a.sent == user ).OrderBy(a => a.dateSent).ToList();

                ViewBag.Msg = searchMsg;
                Debug.WriteLine(searchMsg);
            }
            if(Session["userPosition"].ToString() == "manager") { 
            var userList = GRM.tbl_Users.Where(a => a.userName != currentUser).ToList();
            ViewBag.userList = userList;
            }else
            {
                string user1 = Session["userName"].ToString();
                var selectme = GRM.tbl_Users.FirstOrDefault(a => a.userName == user1);
                
                var userList = GRM.tbl_Users.Where(a => a.branchManager == selectme.userName || a.position == "manager").ToList();

                ViewBag.userList = userList;
            }
            var getAllMessage = GRM.tbl_Message.Where(a => a.receive == currentUser  ).OrderByDescending(a => a.id).ToList().GroupBy(a => a.sent).Select(a => a.First());


            ViewBag.messageList = getAllMessage;

            return View();

        }

        public ActionResult Categories(string search, int? i) {
            string myUser = Session["userName"].ToString();
            var categoryList = GRM.tbl_ProductCategories.Where(a=> a.catsBy == myUser).ToList();
            if (Session["userPosition"].ToString() == "manager")
            {
                 categoryList = GRM.tbl_ProductCategories.OrderByDescending(a => a.categoryId).ToList();

                ViewBag.productList = categoryList.OrderByDescending(a => a.categoryId);
                ViewBag.categoryList = categoryList;
                return View();


            }
            else {
                 categoryList = GRM.tbl_ProductCategories.Where(a => a.catsBy == myUser).OrderByDescending(a => a.categoryId).ToList();

                ViewBag.productList = categoryList.OrderByDescending(a => a.categoryId);
                ViewBag.categoryList = categoryList;
                return View();


            }

        }
        public ActionResult DeleteCategory(int id, string unarchive)
        {
            var category = GRM.tbl_ProductCategories.FirstOrDefault(a => a.categoryId == id);
            if (unarchive == "yes") {
                category.isArchive = null;
                GRM.SaveChanges();
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>The category has been un-archived.</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../../Admin/Categories', '_self'); }});    });</script>     ");

            }
            else
            {
                category.isArchive = "yes";
                GRM.SaveChanges();
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>The category has been archived.</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../../Admin/Categories', '_self'); }});    });</script>     ");

            }




        }
        public ActionResult AddCategoryBes(string catname, string catdesc) {
            string myUser = Session["userName"].ToString();
            
            tbl_ProductCategories cats = new tbl_ProductCategories();
            var selectCat = GRM.tbl_ProductCategories.Where(a => a.name == catname && a.catsBy == myUser).FirstOrDefault();
            if (selectCat != null)
            {
                ViewBag.errorMessage = "The name is already taken";
                return Content("false");

            }
            if (catname == "") {
                return Content("noname");
            }
            cats.name = catname;
            cats.description = catdesc;
            cats.catsBy = myUser;
            cats.dateRegistered = DateTime.Now;
            GRM.tbl_ProductCategories.Add(cats);
            
            GRM.SaveChanges();

            return Content("true");
        }
        public ActionResult ViewMessageFeed(string id) {
            ViewBag.Emails = id;
            
            return View();
        }

        public ActionResult ViewPackage(int id) {

            var getPackData = GRM.tbl_PackageItems.Where(a => a.packageId == id).ToList();
            var content = "";
            double? totalprice = 0;
            double? subTotal = 0;
            foreach (var z in getPackData) {
                totalprice += z.price * z.quantity;
                subTotal = z.price * z.quantity;
                content += "<tr>  <td>" + z.item + "</td>  <td>" + z.quantity + " </td>  <td> ₱" + string.Format("{0:N2}", z.price) + " </td> <td>₱" + string.Format("{0:N2}", subTotal) + " </td></tr>";



            }
                                                                                                                                                                                     
            return Content("<table class='table table-striped table-bordered nowrap'><tr><th>Product Name</th> <th>Product Quantity</th> <th>Product Price</th> <th>Total</th></tr>" + content+ "</table> <h3>Package Price : ₱" + string.Format("{0:N2}", totalprice) + "</h3>");

        }
        public ActionResult ReplyFeed(string emailmsg, string feedmsg) {
            if (emailmsg == "") {
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Write Something</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/ViewMessageFeed/" + emailmsg + "', '_self'); }});    });</script>     ");

               
            }
            try
            {
                GMailer.GmailUsername = "grmthesis02@gmail.com";
                GMailer.GmailPassword = "ragbyboy09";
                GMailer mailer = new GMailer();
                mailer.ToEmail = emailmsg;
                mailer.Subject = "Thank you for your feedback";
                mailer.Body = " " + feedmsg;

                mailer.IsHtml = true;
                mailer.Send();
            }
            catch (Exception e) {
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Wrong Email Address</h3><br><p></p>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../../Admin/FeedbackShow', '_self'); }});    });</script>     ");

            }

            return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Sent Successfully</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../../Admin/FeedbackShow', '_self'); }});    });</script>     ");

        }

        public ActionResult Invoice(string id)
        {

            var getOrder = GRM.tbl_OrderHistory.FirstOrDefault(a => a.orderId == id);
            ViewBag.MainOrder = getOrder;

            var getOrderList = GRM.tbl_OrderList.Where(a => a.orderId == id).ToList();
            ViewBag.OrderList = getOrderList;

         
            var getUser = GRM.tbl_Users.FirstOrDefault(a => a.userId == getOrder.clientId);
            var getUserDetails = GRM.tbl_Users.FirstOrDefault(a => a.userId == getUser.userId);
            ViewBag.UserDetails = getUserDetails;

            var getUserBM = GRM.tbl_Users.FirstOrDefault(a => a.userName == getUserDetails.branchManager);
            ViewBag.UserBM = getUserBM;

            return View();

        }
        public ActionResult DeleteNews(int id) {
            var news = GRM.tbl_News.FirstOrDefault(a => a.Id == id);
            GRM.tbl_News.Remove(news);
            GRM.SaveChanges();

            return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Successfully Deleted</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../../Admin/NewsControl', '_self'); }});    });</script>     ");

        }

        public ActionResult Test()
        {
            string myUser = Session["userName"].ToString();
           var categoryList = GRM.tbl_ProductCategories.Where(a => a.catsBy == myUser).ToList();
         
            ViewBag.categoryList = categoryList;
            return View();
        }

    }


}
//This is a class for email
//THis line of codes handle the emailing process

public class RawModel
{
    public string user { get; set; }
    public double totalSale { get; set; }
}


public class BestSellers
{
    public string product { get; set; }
    public int? numberOfSales { get; set; }
    public string branch { get; set;}
}

public class RawInquiries
{
    public string inquiries { get; set; }
    public DateTime? date_inq { get; set; }

    public string name { get; set; }
}


public class RawSales
{
    public string client { get; set; }
    public double? totalSales { get; set; }
    public int? orderId { get; set; }
    public DateTime? orderDate { get; set; }


}



public class GMailer
{
    public static string GmailUsername { get; set; }
    public static string GmailPassword { get; set; }
    public static string GmailHost { get; set; }
    public static int GmailPort { get; set; }
    public static bool GmailSSL { get; set; }

    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool IsHtml { get; set; }

    static GMailer()
    {
        GmailHost = "smtp.gmail.com";
        GmailPort = 587; // Gmail can use ports 25, 465 & 587; but must be 25 for medium trust environment.
        GmailSSL = true;
    }

    public void Send()
    {
        SmtpClient smtp = new SmtpClient();
        smtp.Host = GmailHost;
        smtp.Port = GmailPort;
        smtp.EnableSsl = GmailSSL;
        smtp.TargetName = "STARTTLS/smtp.gmail.com";

        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(GmailUsername, GmailPassword);

        using (var message = new MailMessage(GmailUsername, ToEmail))
        {
            message.Subject = Subject;
            message.Body = Body;
            message.IsBodyHtml = IsHtml;
            smtp.Send(message);
        }
    }

 
}