using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reservation;

namespace reservations.Controllers
{
     
    public class HomeController : Controller
    {
        BusinessHelper bhelper = new BusinessHelper();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index2()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            //http://jsfiddle.net/pxfunc/j3AN7/
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
       
        public JsonResult SaveOpportunitie(string strmsg)
        {  
            Response response = new Response();
            try
            {
                response = bhelper.Transaction_Opportunities(strmsg);
                if (response.ErrResponse != null || response.ErrResponse == "")
                {
                    throw new Exception(response.ErrResponse);
                }

                return Json(new { result = true, message = "", status = bhelper.CreateHTMLmessage(response) }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                response.ResponseCode = ResCode.Error;
                response.ErrResponse = ex.Message.ToString();

                return Json(new { result = true, message = "", status = bhelper.CreateHTMLmessage(response) }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}