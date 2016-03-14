using System.Collections.Generic;
using BusinessLayer.Models;
using System.Web.Mvc;
using BusinessLayer.Interfaces;

namespace WordLearningMVC.Controllers
{
    //[Authorize(Roles = "Student, Listener")]
    public class TutorialController : Controller
    {
        private ITutorialManager tutorial;
      
        public TutorialController(ITutorialManager tutorialManager)
        {
            tutorial = tutorialManager;
        }

        public ActionResult Index(int wordsuiteId, int time)
        {
            ViewBag.Time = time;
            IList<TutorialModel> models = tutorial.GetItemsFromWordSuite(wordsuiteId);
            return View(models);
        }

        public ActionResult FinishTutorial()
        {
            return View();
        }

        public ActionResult ChooseLevel(int wordsuiteId)
        {
            return View(wordsuiteId);
        }
    }
}
