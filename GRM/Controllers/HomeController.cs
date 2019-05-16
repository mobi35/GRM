using GRM.Models;
using MlkPwgen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace GRM.Controllers
{
    public class HomeController : Controller
    {
        GRMDB GRM = new GRMDB();

        // GET: Home
        public ActionResult Index()
        {
          
            var select1 = GRM.PageContents.FirstOrDefault(a => a.location == 1);
            var select2 = GRM.PageContents.FirstOrDefault(a => a.location == 2);
            var select3 = GRM.PageContents.FirstOrDefault(a => a.location == 3);
            var select4 = GRM.PageContents.FirstOrDefault(a => a.location == 4);
            var select5 = GRM.PageContents.FirstOrDefault(a => a.location == 5);
            var select6 = GRM.PageContents.FirstOrDefault(a => a.location == 6);
            var select7 = GRM.PageContents.FirstOrDefault(a => a.location == 7);
            var select8 = GRM.PageContents.FirstOrDefault(a => a.location == 8);
            var select9 = GRM.PageContents.FirstOrDefault(a => a.location == 9);
            var select10 = GRM.PageContents.FirstOrDefault(a => a.location == 10);
            var select11= GRM.PageContents.FirstOrDefault(a => a.location == 11);
            var select12 = GRM.PageContents.FirstOrDefault(a => a.location == 12);

            TempData["s1"] = select1.contents;
            TempData["s2"] = select2.contents;
            TempData["s3"] = select3.contents;
            TempData["s4"] = select4.contents;
            TempData["s5"] = select5.contents;
            TempData["s6"] = select6.contents;

            TempData["s7"] = select7.contents;
            TempData["s8"] = select8.contents;
            TempData["s9"] = select9.contents;
            TempData["s10"] = select10.contents;
            TempData["s11"] = select11.contents;
            TempData["s12"] = select12.contents;

            var selectNews = GRM.tbl_News.ToList().OrderByDescending(a => a.Id).Take(5);
            ViewBag.news = selectNews;
            return View();
        }

        public ActionResult ErrorPage() {

            return View();

        }

      
        public ActionResult Login()
        {

            return View();
        }
        public string base64Decode2(string sData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(sData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
        public ActionResult LoginExecute(tbl_Users login)
        {

            var searchAgain = GRM.tbl_Users.Where(user => user.userName == login.userName).FirstOrDefault();
            tbl_Users searchUser = null;
            //Check if the field is empty or not
            // Throw success if the field is not empty and failed if empty.
            if (login != null)
            {
          
                var userz = GRM.tbl_Users.ToList();
              
                foreach (var a in userz) {
                   
                    if (base64Decode2(a.PassWord) == login.PassWord && a.userName == login.userName)
                    {
                       
                    
             searchUser = GRM.tbl_Users.FirstOrDefault(x => x.userName == a.userName );
                      

                    }
                }

                try
                {
                    var dateExpiration = searchUser.dateRegistered.Value.AddYears(searchUser.contract_duration.Value);
                    Session["expiration"] = dateExpiration.ToShortDateString().ToString();
                    if (dateExpiration < DateTime.Now)
                    {

                        return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Your contract Duration has expired please contact your Branch Manager</h3><br><p></p>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Admin/RegisterUser', '_self'); }});    });</script>     ");
                    }
                }
                catch (Exception e)
                {

                }


                //create a searchuser variable then transfer the searched paremeters on the database to the searchuser


                if (searchUser != null && searchUser.tries < 3)
                {
                   


                    //Pass the user's id to a Session Variable.
                    // Session Variable can be save through your browser's cache.
                    Session["userId"] = searchUser.userId;
                    Session["userName"] = searchUser.userName;
                    Session["userPosition"] = searchUser.position;

                    
                    
                    if(searchUser.credits == null) {
                        Session["credits"] = 0;
                    }
                    else
                    {
                        Session["credits"] = searchUser.credits;
                    }
                    var searchBM = GRM.tbl_Users.FirstOrDefault(a => a.branchManager == searchUser.userName || a.branchManager == null);
                    var user5 = GRM.tbl_Users.ToList();
                    string thisUser = "";
                    foreach (var s in user5) {
                        if (s.branchManager == Session["userName"].ToString()) {
                            thisUser = s.userName;
  
                              
                        
                              
                          
                        }
                    }
                    Session["myDoubles"] = thisUser;
                    Debug.WriteLine("Gumagana ang session" +Session["myDoubles"].ToString());
                 
                    if (Session["userPosition"] != null)
                    {
                        if (searchUser.position == "client")
                        {
                            Session["myDouble"] = searchUser.branchManager;

                        }
                    }
                    searchUser.tries = 0;
                    GRM.SaveChanges();
                    login.outputMessage = "Success";

                    if (searchUser.position == "client")
                    {
                        //Return webpage to Client's page if the Login user's position is client.
                        return RedirectToAction("Index", "Client");
                    }

                    //Return webpage to Admin's/Manager's page if the Login is determined.
                    return RedirectToAction("Index", "Admin");
                }


                if (searchAgain != null)
                {
                    if (login.userName == searchAgain.userName)
                    {

                        if (searchAgain.tries == null)
                        {
                            searchAgain.tries = 1;
                        }
                        else {
                            searchAgain.tries += 1;
                        }
                        GRM.SaveChanges();

                    }
                }
            }
            if (searchAgain != null)
            {
                if (searchAgain.tries > 3)
                {
                    login.outputMessage = "Your account is blocked due to 3 consecutive mistakes";

                }
                else {
                    login.outputMessage = "Wrong Credentials";
                }
            }
            else {
                login.outputMessage = "Wrong Credentials";
            }


            //Return to login screen if failed.
            return View("Login", login);
        }


        public ActionResult ShowNews(string urls)
        {
            int b = Convert.ToInt32(urls);
            var getSpecificNews = GRM.tbl_News.FirstOrDefault(x => x.Id == b);
            ViewBag.specNews = getSpecificNews;


            return View();
        }

        public ActionResult Services() {
            var select7 = GRM.PageContents.SingleOrDefault(a => a.location == 7);
            var select8 = GRM.PageContents.SingleOrDefault(a => a.location == 8);
            var select9 = GRM.PageContents.SingleOrDefault(a => a.location == 9);
            var select10 = GRM.PageContents.SingleOrDefault(a => a.location == 10);
            var select11 = GRM.PageContents.SingleOrDefault(a => a.location == 11);
            var select12 = GRM.PageContents.SingleOrDefault(a => a.location == 12);
            var select13 = GRM.PageContents.SingleOrDefault(a => a.location == 13);
            var select14 = GRM.PageContents.SingleOrDefault(a => a.location == 14);
            var select15 = GRM.PageContents.SingleOrDefault(a => a.location == 15);
            TempData["s7"] = select7.contents;
            TempData["s8"] = select8.contents;
            TempData["s9"] = select9.contents;
            TempData["s10"] = select10.contents;
            TempData["s11"] = select11.contents;
            TempData["s12"] = select12.contents;
            TempData["s13"] = select13.contents;
            TempData["s14"] = select14.contents;
            TempData["s15"] = select15.contents;
            return View();
        }
        public ActionResult ForgotPasssword()
        {

            return View();
        }

   

        [HttpPost]
        public ActionResult ForgotPasssword(tbl_Users email)
        {
            try {
                var checkEmail = GRM.tbl_Users.Where(a => a.emailAddress == email.emailAddress).FirstOrDefault();
                if (checkEmail != null) {
                    var str = PasswordGenerator.Generate(length: 10, allowed: Sets.Alphanumerics);

                    Session["ResetCode"] = str;
                    Session["ForgotUser"] = checkEmail.userId;
                    GMailer.GmailUsername = "grmthesis02@gmail.com";
                    GMailer.GmailPassword = "ragbyboy09";
                    GMailer mailer = new GMailer();
                    mailer.ToEmail = checkEmail.emailAddress;
                    mailer.Subject = "Password Reset";
                    mailer.Body += "Please click this link to reset your password";
                    mailer.Body += " http://localhost:1827/Home/ResetPassword/" + str;
                    mailer.IsHtml = true;
                    mailer.Send();

                }

                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Please check your email to reset your password.</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Home/Login', '_self'); }});    });</script>     ");

            }
            catch (Exception e) {
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Something went wrong please enter a valid email.</h3><br><p></p>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Home/ForgotPassword', '_self'); }});    });</script>     ");

            }
        }

        public ActionResult ResetPassword(string id) {
            Session["ResetPassword"] = 5;
            if (id == null) {
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Something went wrong please check the link.</h3><br><p></p>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Home/ForgotPassword', '_self'); }});    });</script>     ");

            }
            if (Session["ResetCode"].ToString() != id) {
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Something went wrong please check your link</h3><br><p></p>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Home/ForgotPassword', '_self'); }});    });</script>     ");

            }

            Session["ResetPassword"] = id;
            return View();
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
        public ActionResult ChangePassword(string pass1, string pass2)
        {
            if (pass1== pass2 && pass1 != "" && pass2 != "")
            {
                int userId = Convert.ToInt32(Session["ForgotUser"]);
                var changeMyPassword = GRM.tbl_Users.FirstOrDefault(a => a.userId == userId);
                changeMyPassword.PassWord = base64Encode(pass1);
                GRM.SaveChanges();
                return Content("changed");



            }
            else
            {
                return Content("failed");

            }




        }

        public ActionResult ContactUs() {

            return View();
        }

        public ActionResult InsertInquiry(string name, string email, string message) {

            tbl_Feedbacks feed = new tbl_Feedbacks();
            feed.name = name;
            feed.email = email;
            feed.message = message;
            feed.dateS = DateTime.Now;
            GRM.tbl_Feedbacks.Add(feed);

            var notifications = GRM.tbl_Notifications.SingleOrDefault(a => a._int == 2);
            notifications.messageNotif += 1;
            GRM.SaveChanges();
            return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Thank you for your feedback!</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Home/Index', '_self'); }});    });</script>     ");

        }

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

}


