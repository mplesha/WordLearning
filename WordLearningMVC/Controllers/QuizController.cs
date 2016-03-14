using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;

namespace WordLearningMVC.Controllers
{
    //[Authorize(Roles = "Student, Listener")]
    public class QuizController : Controller
    {
        private CurrentUserInfo currentUser = new CurrentUserInfo();
        private IQuizManager quiz;

        public QuizController(IQuizManager quizManager)
        {
            quiz = quizManager;
        }

        public ActionResult RunQuiz(int wordSuiteId)
        {
            IList<ItemTranslationModel> itemTranslations = quiz.GenerateQuiz(currentUser.User.Id, wordSuiteId);
            if (itemTranslations[0].QuizTimeOut != DateTime.MinValue)
                return RedirectToAction("QuizTimeout", "Quiz", new { quizTimeOut = itemTranslations[0].QuizTimeOut, wordSuiteId = wordSuiteId});
            return View(itemTranslations);
        }

        public ActionResult QuizAnswers(List<ItemTranslationModel> itemTranslations, List<string> userTranslations)
        {
            QuizModel quizAnswers = quiz.QuizAnswers(currentUser.User.Id,itemTranslations, userTranslations);
            return View(quizAnswers);
        }

        public ActionResult QuizTimeout(DateTime quizTimeOut, int wordSuiteId)
        {
            ViewBag.QuizTimeOut = quizTimeOut.ToShortTimeString();
            ViewBag.WordSuiteId = wordSuiteId;
            return View();
        }
    }
}
