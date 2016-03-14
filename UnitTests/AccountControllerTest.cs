using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WordLearningMVC.Controllers;
using System.Web.Mvc;
using WordLearningMVC.Models;


namespace UnitTests
{
    [TestClass]
    public class AccountControllerTest
    {
        [TestMethod]
        public void Can_Login_With_Valid_Info()
        {

            LoginModel model = new LoginModel
            {
                UserName = "mykhalik",
                Password = "1234"
            };

            AccountController target = new AccountController();

            ActionResult result = target.Login(model);

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.IsTrue(((ViewResult)result).ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Login_With_InValid_Info()
        {

            LoginModel model = new LoginModel
            {
                UserName = "abcdefg",
                Password = "1234"
            };

            AccountController target = new AccountController();

            ActionResult result = target.Login(model);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }



        //[TestMethod]
        //public void Can_Logout()
        //{
        //    AccountController target = new AccountController();

        //    ActionResult result = target.LogOff();

        //    Assert.IsNull(User.Identity.IsAuthenticated);

        //}
    }
}
