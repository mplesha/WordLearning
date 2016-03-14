using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.UnitOfWork;
using WordLearningMVC.Models;

namespace WordLearningMVC.Controllers
{
    public class HomeController : Controller
    {
        //private int count = 0;
        public ActionResult Index()
        {

            return View();
        }


    }
}
