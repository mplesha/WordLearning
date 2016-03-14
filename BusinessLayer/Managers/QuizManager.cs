using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.UnitOfWork;

namespace BusinessLayer.Managers
{
    public class QuizManager : IQuizManager
    {
        private StudentManager student = new StudentManager();
        private AdminManager admin = new AdminManager();
        public readonly int ENFORCED_NUMBER_OF_ATTEMPTS;
        public readonly TimeSpan ENFORCED_DELAY_BETWEEN_ATTEMPTS;
        private const double IN_PROGRESS_STATUS = 0.5;
        private const double FINISHED_STATUS = 1;
        private const double CHECKING_FINISHED_STATUS = 99;
        private const double CHECKING_IN_PROGRESS_STATUS = 1;
        public DateTime currentDateTime = DateTime.Now;

        public QuizManager()
        {
            Settings settings = admin.GetSettings();
            ENFORCED_NUMBER_OF_ATTEMPTS = settings.ENFORCED_NUMBER_OF_ATTEMPTS;
            ENFORCED_DELAY_BETWEEN_ATTEMPTS = settings.ENFORCED_DELAY_BETWEEN_ATTEMPTS;
        }

        public List<ItemTranslationModel> ItemTranslationsRand(List<ItemTranslationModel> itemTranslations)
        {
            List<ItemTranslationModel> itemTranslationsRand = new List<ItemTranslationModel>();
            List<int> randomNumbers = new List<int>();
            Random rnd = new Random();
            while (randomNumbers.Count != itemTranslations.Count)
            {
                int newNumber = rnd.Next(itemTranslations.Count);
                if (!randomNumbers.Contains(newNumber))
                {
                    randomNumbers.Add(newNumber);
                }
            }
            for (int i = 0; i < randomNumbers.Count; i++)
            {
                itemTranslationsRand.Add(itemTranslations[randomNumbers[i]]);
            }
            return itemTranslationsRand;
        }

        public List<ItemTranslationModel> GenerateQuiz(int currentUserId, int wordSuiteId)
        {
            List<ItemTranslationModel> itemTranslations = student.AllItems(wordSuiteId);
            List<LearningWords> userLearningWords = ItemsLearning(currentUserId, itemTranslations);
            if (userLearningWords.All(learningWords => learningWords == null))
                return ItemTranslationsRand(itemTranslations);
            DateTime quizTimeout = CheckQuizTimeout(userLearningWords.First(learningWord => learningWord != null));
            if (quizTimeout != DateTime.MinValue)
            {
                List<ItemTranslationModel> timeToPassQuiz = new List<ItemTranslationModel>();
                timeToPassQuiz.Add(new ItemTranslationModel()
                {
                    QuizTimeOut = quizTimeout,
                });
                return timeToPassQuiz;
            }
            else
                return NotFirstQuizGeneration(currentUserId, wordSuiteId, userLearningWords);
        }

        public List<ItemTranslationModel> NotFirstQuizGeneration(int studentId, int wordSuiteId, List<LearningWords> userLearningWords)
        {
            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);

                List<Translation> translations = new List<Translation>();
                userLearningWords.RemoveAll(learningWords => learningWords == null);

                foreach (var translation in userLearningWords)
                {
                    translations.Add(unit.GetRepository<Translation>().GetByID(translation.TranslationId));
                }
                List<ItemTranslationModel> itemTranslations = student.ItemTranslation(translations, wordSuiteId);

                return ItemTranslationsRand(itemTranslations);
            }
        }

        public List<LearningWords> ItemsLearning(int currentUserId, List<ItemTranslationModel> itemTranslations)
        {
            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);
                List<LearningWords> currentLearningWords = new List<LearningWords>();
                foreach (var itemTranslation in itemTranslations)
                {
                    LearningWords currentLearningWord =
                        unit.GetRepository<LearningWords>()
                            .Get(translation => translation.WordSuiteId == itemTranslation.WordSuiteIdFrom &&
                                 translation.StudentId == currentUserId &&
                                 translation.TranslationId == itemTranslation.Id &&
                                 translation.Progress <= 0.99)
                            .FirstOrDefault();

                    currentLearningWords.Add(currentLearningWord);
                }
                return currentLearningWords;
            }
        }

        public List<ItemTranslationModel> CheckingRepeatOfItems(List<ItemTranslationModel> itemTranslations, string translation)
        {
            List<ItemTranslationModel> repeatedItems = new List<ItemTranslationModel>();

            if (itemTranslations.Count(itm => itm.Translation == translation) > 1)
                repeatedItems.AddRange(itemTranslations.Where(trs => trs.Translation == translation));

            return repeatedItems;
        }

        public QuizModel QuizAnswers(int currentUserId, List<ItemTranslationModel> itemTranslations, List<string> userTranslations)
        {
            QuizModel quizModel = new QuizModel();
            List<LearningWords> userLearningWord = ItemsLearning(currentUserId, itemTranslations);
            List<bool> answers = new List<bool>();

            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);

                for (int i = 0; i < itemTranslations.Count; i++)
                {
                    List<ItemTranslationModel> currentItemsRepeats = CheckingRepeatOfItems(itemTranslations, itemTranslations[i].Translation);

                    double progress = 0;
                    answers.Add(false);
                    if (userLearningWord[i] == null)
                    {
                        if (userTranslations[i] == itemTranslations[i].Item ||
                            currentItemsRepeats.Any(trs => trs.Item == userTranslations[i]))
                        {
                            progress = Math.Round(1.0 / ENFORCED_NUMBER_OF_ATTEMPTS, 2);
                            answers[i] = true;
                        }

                        userLearningWord[i] = new LearningWords
                        {
                            StudentId = currentUserId,
                            WordSuiteId = itemTranslations[i].WordSuiteIdFrom,
                            LearnedDate = DateTime.Now,
                            Progress = progress,
                            TranslationId = itemTranslations[i].Id
                        };
                        unit.GetRepository<LearningWords>().Insert(userLearningWord[i]);
                    }
                    else
                    {
                        if (userTranslations[i] == itemTranslations[i].Item ||
                            currentItemsRepeats.Any(trs => trs.Item == userTranslations[i]))
                        {
                            userLearningWord[i].Progress += Math.Round(1.0 / ENFORCED_NUMBER_OF_ATTEMPTS, 2);
                            if (userLearningWord[i].Progress > 0.9)
                                userLearningWord[i].Progress = FINISHED_STATUS;
                            answers[i] = true;
                        }

                        userLearningWord[i].LearnedDate = DateTime.Now;
                        unit.GetRepository<LearningWords>().Update(userLearningWord[i]);
                    }
                    quizModel.Item.Add(itemTranslations[i].Translation);
                    quizModel.Translation.Add(itemTranslations[i].Item);
                }

                quizModel.UserTranslation.AddRange(userTranslations);
                quizModel.isCorrectTranslation.AddRange(answers);
                quizModel.WordSuiteId = itemTranslations[0].WordSuiteIdFrom;
                unit.Save();

                SetCourseStatus(itemTranslations[0].WordSuiteIdFrom, currentUserId);
                return quizModel;
            }
        }

        public void SetCourseStatus(int wordSuiteId, int userId)
        {
            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);

                Course currentCourse =
                unit.GetRepository<WordSuite>()
                    .Get(ws => ws.WordSuiteId == wordSuiteId)
                    .Select(cs => unit.GetRepository<Course>().GetByID(cs.CourseId))
                    .FirstOrDefault();

                Progress currentProgress =
                unit.GetRepository<Progress>()
                    .Get(pr => pr.CourceId == currentCourse.Id && pr.StudentId == userId)
                    .FirstOrDefault();

                double currentWordSuiteProgress = student.WordSuiteProgress(wordSuiteId, userId);
                if (currentWordSuiteProgress > CHECKING_IN_PROGRESS_STATUS)
                {
                    currentProgress.Status = IN_PROGRESS_STATUS;
                    List<WordSuite> currentCourseWordsuites = currentCourse.WordSuites.ToList();

                    for (int i = 0; i < currentCourseWordsuites.Count; i++)
                    {
                        double wordsuiteStatus = student.WordSuiteProgress(currentCourseWordsuites[i].WordSuiteId,
                            userId);
                        if (wordsuiteStatus < CHECKING_FINISHED_STATUS)
                            break;

                        if (i == currentCourseWordsuites.Count - 1)
                        {
                            if (wordsuiteStatus > CHECKING_FINISHED_STATUS)
                            {
                                currentProgress.Status = FINISHED_STATUS;
                            }
                        }
                    }
                    unit.GetRepository<Progress>().Update(currentProgress);

                    unit.Save();
                }
            }
        }

        public DateTime CheckQuizTimeout(LearningWords userLearningWord)
        {
            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);

                if (currentDateTime.Date == userLearningWord.LearnedDate.Date)
                {
                    DateTime timeToPassQuiz = userLearningWord.LearnedDate;
                    if (currentDateTime.TimeOfDay.Subtract(userLearningWord.LearnedDate.TimeOfDay) >= ENFORCED_DELAY_BETWEEN_ATTEMPTS)
                        return DateTime.MinValue;
                    else
                        return timeToPassQuiz.Add(ENFORCED_DELAY_BETWEEN_ATTEMPTS);
                }
                return DateTime.MinValue;
            }
        }
    }
}
