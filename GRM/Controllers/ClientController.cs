using GRM.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using System.Diagnostics;

namespace GRM.Controllers
{
    
    public class ClientController : Controller
    {
        GRMDB GRM = new GRMDB();
        // GET: Client
        public ActionResult Index()
        {
            string user = Session["userName"].ToString();
          
            
          
            var getNotifMessage = GRM.tbl_Users.SingleOrDefault(a => a.userName == user);

            Session["clmsg"] = getNotifMessage.messageNotif;

            return View();
        }

        //Ordering a product/package page
        public ActionResult Order()
        {

            string myUser = Session["userName"].ToString();
          
            var getUserRating = GRM.tbl_Rating.FirstOrDefault(a => a.client == myUser);
            if (getUserRating != null)
            {
                ViewBag.Rating = "ALREADYRATING";
            }
            var searchBM = GRM.tbl_Users.FirstOrDefault(a => a.userName == myUser);
            var selectCats = GRM.tbl_ProductCategories.Where(a => a.catsBy ==searchBM.branchManager).ToList();

            ViewBag.allCategory = selectCats;

           // var selectAllProducts = GRM.tbl_Products.ToList();

         

            return View();
        }

  
        public ActionResult AddToCart(int id, string orderType) {

            // string username = Session["userName"].ToString();
            //Check if the cart is empty. Do the function below if its empty.
            if (Session["cart"] == null)
            {
                //Select the products using productId and matching it to the parameters above "Buy(int id)"
                var selectProduct = GRM.tbl_Products.Where(model => model.productId == id).SingleOrDefault();

                //Instantiate a list of ProductItem in the models. 
                //So we can create a list of product and insert it in the cart
                List<ProductItem> cart = new List<ProductItem>();

                //Insert new Items in cart and set the product to the matched ID and Quantity to 1
                //
                cart.Add(new ProductItem { Product = selectProduct, Quantity = 1 });

                //Create a new Session named cart and insert the list of cart that we made above.
                Session["cart"] = cart;

                //Return a javascript and throw an alert.
                if(orderType == "yes") {
                    return Content("package");
                }
                return Content("added");
            }
            else {
                List<ProductItem> cart = (List <ProductItem>)Session["cart"];
                var selectProduct = GRM.tbl_Products.Where(model => model.productId == id).SingleOrDefault();
                int index = isExist(selectProduct.productId);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new ProductItem { Product = selectProduct, Quantity = 1 });

                }
                Session["cart"] = cart;
                if (orderType == "yes")
                {
                    return Content("package");
                }
                return Content("added");
            }

        }

        private int isExist(int id)
        {
            // Check if the item is already in the cart ..
            List<ProductItem> cart = (List < ProductItem > )Session["cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].Product.productId.Equals(id))
                    return i;
            return -1;

        }


        public ActionResult Remove(int id)
        {
            //Remove item in the cart
            List<ProductItem> cart = (List < ProductItem >) Session["cart"];
            int index = isExist(id);
            cart.RemoveAt(index);
            Session["cart"] = cart;
            return RedirectToAction("Cart");
        }

        public ActionResult GetRating(string user)
        {
            Console.WriteLine("USER EXIST " + user);
           var clientRating = GRM.tbl_Rating.FirstOrDefault(a => a.client == user);
            return Json(new { GetRating = clientRating }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cart()
        {
            string myUser = Session["userName"].ToString();
            var getUserRating = GRM.tbl_Rating.FirstOrDefault(a => a.client == myUser);
            if (getUserRating != null)
            {
                ViewBag.Rating = "ALREADYRATING";
            }
            return View();
        }

        public ActionResult Product(int? id)
        {
           
            var selectProduct = GRM.tbl_Products.SingleOrDefault(a => a.productId == id);
            ViewBag.Details = selectProduct;

            var selectPackage = GRM.tbl_PackageItems.Where(a => a.packageId == id).ToList();
            ViewBag.Package = selectPackage;
            return View();

        }

        public ActionResult Checkout(int[] sendQuantity, int[] prodId,double[] price,string[] prodItem, string date) {
         
            if(Session["userName"] != null) { 
            if (Session["cart"] != null) { 
        
            var notifications = GRM.tbl_Notifications.SingleOrDefault(a => a._int == 3);
            notifications.messageNotif += 1;

            var getCurrentUser = Session["userName"].ToString();
            var getBM = GRM.tbl_Users.FirstOrDefault(a => a.userName == getCurrentUser);
            var getBMNow = GRM.tbl_Users.FirstOrDefault(a => a.userName == getBM.branchManager);
            if(getBMNow != null) {
                getBMNow.orderNotif += 1;
            }
            else
            {
                    return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Warning</h3><br><p>You don't have a branch manager yet. Please request.</p>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Client/MyOrders', '_self'); }});    });</script>     ");
            }
                GRM.SaveChanges();
                
              
                    tbl_OrderHistory orderHistory = new tbl_OrderHistory();
                    if (date == "") {


                

                        if (getBM.saturday_operate == null)
                        {
                            if (DateTime.Now.AddDays(4).DayOfWeek == DayOfWeek.Saturday)
                                orderHistory.deliveryDate = DateTime.Now.AddDays(6);

                        }else
                        {
                            orderHistory.deliveryDate = DateTime.Now.AddDays(4);
                        }

                        if (getBM.sunday_operate == null)
                        {
                            if (DateTime.Now.AddDays(4).DayOfWeek == DayOfWeek.Sunday) 
                                orderHistory.deliveryDate = DateTime.Now.AddDays(5);
                          
                        }else  {
                            orderHistory.deliveryDate = DateTime.Now.AddDays(4);
                        }
                    





                    }
            else {
                        var dateDelivery = DateTime.Now;
                 dateDelivery = DateTime.Parse(date);
                        if(dateDelivery.DayOfWeek == DayOfWeek.Saturday)
                        {
                            if(getBM.saturday_operate == null)
                            {
                                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Not available on Saturday</h3>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Client/Cart', '_self'); }});    });</script>     ");
                            }
                        }

                        if (dateDelivery.DayOfWeek == DayOfWeek.Sunday)
                        {
                            if (getBM.sunday_operate == null)
                            {
                                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Not available on Sunday</h3>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Client/Cart', '_self'); }});    });</script>     ");
                            }
                        }
                    }
           
            var getOrderNumber = GRM.tbl_OrderHistory.OrderByDescending(a => a.orderId).FirstOrDefault();
            int orNumber = 0;
            if (getOrderNumber != null)
            {
                orNumber = Convert.ToInt32(getOrderNumber.orderId);
                orNumber++;
               
            }
            else {
                orNumber = 100000000;
            }
            double totalPrice = 0;
            for (int i = 0; i < prodId.Count(); i++) {
                int productId = prodId[i];
                int quantity = sendQuantity[i];
                      
                double prodPrice = price[i];
                string itemName = prodItem[i];
                tbl_OrderList order = new tbl_OrderList();

                order.orderId = orNumber +"";
                order.quantity = quantity;
                order.packageId = productId;
                       
                var getProduct = GRM.tbl_Products.FirstOrDefault(a => a.productId == productId);
                        if (getProduct.bought == null)
                            getProduct.bought = 0;

                          getProduct.bought += order.quantity;
                order.prodName = itemName;
                order.price = prodPrice;
                        order.branch = getBMNow.userName;
                order.clientId = Convert.ToInt32(Session["userId"].ToString());
               
                GRM.tbl_OrderList.Add(order);
                GRM.SaveChanges();

                totalPrice += prodPrice * quantity;

            }
       
                
                    orderHistory.isApproved = 1;
                   
            orderHistory.clientId = Convert.ToInt32(Session["userId"].ToString());
            orderHistory.orderId = orNumber + "";
        
            orderHistory.totalPrice = totalPrice;
           orderHistory.branch = getBMNow.userName;
            orderHistory.dateRegistered = DateTime.Now;
            GRM.tbl_OrderHistory.Add(orderHistory);

           double? totalDeduction;
            if (getBM.credits > totalPrice && getBM.credits != null)
             {
                 totalDeduction = getBM.credits - totalPrice;
                   getBM.credits = totalDeduction;
                   GRM.SaveChanges();
                   Session.Remove("cart");
                   Session["credits"] = totalDeduction;
                  return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Successfully paid</h3><br> <p> We deducted ₱" + string.Format("{0:N2}", totalPrice) + " to your credit. </p><br> <p>You now have ₱" + string.Format("{0:N2}", totalDeduction) + " in your account</p> ',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Client/', '_self'); }});    });</script>     ");

             }
                  else
             {

                  return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>You dont have enough balance.</h3>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Client/Cart', '_self'); }});    });</script>     ");

            }
                   
            
              
            }else
            {
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Please order something</h3><br><p>Check our product and package.</p>',icon: $.sweetModal.ICON_WARNING,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Client/MyOrders', '_self'); }});    });</script>     ");

            }
            }
            return View();
        }
       

        public ActionResult MyOrders(string searched, int? i) {
            string myUser = Session["userName"].ToString();
            var getUserRating = GRM.tbl_Rating.FirstOrDefault(a => a.client == myUser);
            if (getUserRating != null)
            {
                ViewBag.Rating = "ALREADYRATING";
            }
            int userId = Convert.ToInt32(Session["userId"].ToString());
            var getOrders = GRM.tbl_OrderHistory.Where(a => a.clientId == userId).OrderByDescending(a => a.orderId).ToList();
            ViewBag.ordered = getOrders;

            return View();

        }

        public ActionResult Invoice(string id) {

            var getOrder = GRM.tbl_OrderHistory.FirstOrDefault(a => a.orderId == id);
            ViewBag.MainOrder = getOrder;

            var getOrderList = GRM.tbl_OrderList.Where(a => a.orderId == id).ToList();
            ViewBag.OrderList = getOrderList;

            int userId = Convert.ToInt32(Session["userId"].ToString());
            var getUserDetails = GRM.tbl_Users.FirstOrDefault(a => a.userId == userId);
            ViewBag.UserDetails = getUserDetails;

            var getUserBM = GRM.tbl_Users.FirstOrDefault(a => a.userName == getUserDetails.branchManager);
            ViewBag.UserBM = getUserBM;
            return View();

        }

        public void Rating(double? quality,string reviewMessage,double? services, double? priceImprovement)
        {
            var client = Session["userName"].ToString();

            var searchIfExist = GRM.tbl_Rating.FirstOrDefault(a => a.client == client);
            if(searchIfExist == null) {  
            var rating = new tbl_Rating();
            rating.client = client;
            rating.quality = quality;
            rating.message = reviewMessage;
            rating.services = services;
            rating.price_improvement = priceImprovement;
            rating.date_rated = DateTime.Now;
            GRM.tbl_Rating.Add(rating);
            GRM.SaveChanges();
            }
            else
            {
                searchIfExist.client = client;
                searchIfExist.quality = quality;
                searchIfExist.message = reviewMessage;
                searchIfExist.services = services;
                searchIfExist.price_improvement = priceImprovement;
                searchIfExist.date_rated = DateTime.Now;
                GRM.SaveChanges();
            }

        }
        public ActionResult StorePackage() {
            string myUser = Session["userName"].ToString();
       



            var getUserRating = GRM.tbl_Rating.FirstOrDefault(a => a.client == myUser);
            if (getUserRating != null)
            {
                ViewBag.Rating = "ALREADYRATING";
            }
            var searchBM = GRM.tbl_Users.FirstOrDefault(a => a.userName == myUser);
            var AllProducts = GRM.tbl_Products.Where(a => a.categoryId == 0 && a.packageBy == searchBM.branchManager  && a.isArchive == null).ToList();

            ViewBag.AllProducts = AllProducts;

            // var selectAllProducts = GRM.tbl_Products.ToList();
          
          

            var getProducts = GRM.tbl_Products.Where(a => a.packageBy == searchBM.branchManager && a.categoryId == 0).OrderByDescending(a => a.bought).FirstOrDefault();
            ViewBag.besters = getProducts;

            return View();
        }


        [ValidateInput(false)]
        public ActionResult SendMessage(string myUsers, string myMessages)
        {
            if (myMessages == "") {
                return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Please insert a message.</h3><br><p></p>',icon: $.sweetModal.ICON_ERROR,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Client/Message', '_self'); }});    });</script>     ");

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
            return Content("<link href='../../Scripts/jquery.sweet-modal.css' rel='stylesheet'/><script src='../../Scripts/jquery223.js'></script><script src='../../Scripts/jquery.sweet-modal.js'></script>   <script> $(document).ready(function() {     $.sweetModal({content: '<h3>Message Sent</h3><br><p></p>',icon: $.sweetModal.ICON_SUCCESS,theme: $.sweetModal.THEME_DARK,onClose: function () {window.open('../Client/Message', '_self'); }});    });</script>     ");

        }

        public ActionResult Message(string search, int? i, int? readable, string reads)
        {
            string myUser = Session["userName"].ToString();
            var getUserRating = GRM.tbl_Rating.FirstOrDefault(a => a.client == myUser);
            if (getUserRating != null)
            {
                ViewBag.Rating = "ALREADYRATING";
            }
            if (Request.Form["delete"] != null) {

                var searchMsg = GRM.tbl_Message.SingleOrDefault(a => a.id == readable);
                if (searchMsg == null) {
                   return RedirectToAction("Message");
                }
                GRM.tbl_Message.Remove(searchMsg);
                GRM.SaveChanges();

            }
            if (readable != null && Request.Form["reply"] != null)
            {
                string user = Session["userName"].ToString();
                var searchMsg = GRM.tbl_Message.Where(a => a.receive == user && a.sent == reads || a.receive == reads && a.sent == user).OrderBy(a => a.dateSent).ToList();

                ViewBag.Msg = searchMsg;
                Debug.WriteLine(searchMsg);

            }
            var currentUser = System.Web.HttpContext.Current.Session["userName"].ToString();
            var getMyBranchManager = GRM.tbl_Users.SingleOrDefault(a => a.userName == currentUser);

            var getMe = GRM.tbl_Users.FirstOrDefault(a => a.userName == currentUser);
            getMe.messageNotif = 0;
            Session["clmsg"] = "0";
            GRM.SaveChanges();
          
            var userList = GRM.tbl_Users.Where(a =>  a.branchManager == getMyBranchManager.branchManager  ).ToList();
            ViewBag.userList2 = userList;
            
            var getAllMessage = GRM.tbl_Message.Where(a => a.receive == currentUser).ToList().OrderByDescending(a => a.id).GroupBy(a => a.sent).Select(a => a.First());
            ViewBag.messageList = getAllMessage;
           
            foreach (var a in getAllMessage) {
                Debug.WriteLine(a.sent + " - " + a.message);
            }
            return View();

        }

    }
}