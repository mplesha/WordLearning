using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.UnitOfWork;
using BusinessLayer.ExtensionMethods;
using MoreLinq;

namespace BusinessLayer.Managers
{
    public class TeacherManager : ITeacherManager
    {
        private readonly IUnitOfWork _unit;

        public TeacherManager()
        {
            _unit = new UnitOfWork();
        }
        public TeacherManager(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            _unit = unitOfWork;
        }

        private readonly StudentManager _student = new StudentManager();
        private const int BasicCourse = 1;

        public IList<Profile> GetTeacherStudents(int teacherId)
        {
            var currentTeacherStudents =
                _unit.GetRepository<Progress>()
                    .Get(progress => progress.TeacherId == teacherId)
                    .Select(p => p.Student.User.Profile).Distinct().ToList();
            
            return currentTeacherStudents;
        }

        public IList<StudentCoursesModel> GetStudentCourses(int studentId)
        {
            string currentPersonRole = ((User)HttpContext.Current.Profile["User"]).PersonRole.ToString();
            IList<StudentCoursesModel> currentStudentCourses = new List<StudentCoursesModel>();
            List<Course> studentCourseList = _unit.GetRepository<Progress>().Get(progress => progress.StudentId == studentId)
                .Select(progress => progress.Course)
                .ToList();
            if (currentPersonRole == "Teacher")
                studentCourseList = studentCourseList.Where(course => course.Visible == true).ToList();

            for (int i = 0; i < studentCourseList.Count; i++)
            {
                StudentCoursesModel currentStudentCourse = new StudentCoursesModel();
                Progress currentProgress =
                    studentCourseList[i].Progresses.Where(std => std.StudentId == studentId).FirstOrDefault();
                currentStudentCourse.WordSuitesCount = studentCourseList[i].WordSuites.Count;
                currentStudentCourse.CourseName = studentCourseList[i].Name;
                currentStudentCourse.CourseId = studentCourseList[i].Id;
                currentStudentCourse.CourseLanguage = studentCourseList[i].Language;
                currentStudentCourse.Creator = studentCourseList[i].Creater;
                currentStudentCourse.Status = currentProgress.Status;
                currentStudentCourse.StudentId = studentId;
                currentStudentCourse.StudentFirstName = _unit.GetRepository<Profile>().GetByID(studentId).FirstName;
                currentStudentCourse.StudentLastName = _unit.GetRepository<Profile>().GetByID(studentId).LastName;

                currentStudentCourses.Add(currentStudentCourse);
            }
            return currentStudentCourses;
        }

        public IList<CourseWordSuitesModel> GetCourseWordSuites(int courseId)
        {
            IList<CourseWordSuitesModel> currentCourseWordsuites = new List<CourseWordSuitesModel>();

            Course currentCourse = _unit.GetRepository<Course>()
                .GetByID(courseId);
            List<WordSuite> courseWordSuites =
                _unit.GetRepository<WordSuite>()
                .Get(ws => ws.CourseId == currentCourse.Id)
                .ToList();

            for (int i = 0; i < courseWordSuites.Count; i++)
            {
                CourseWordSuitesModel currentCourseWordSuite = new CourseWordSuitesModel();

                currentCourseWordSuite.WordsuiteName = courseWordSuites[i].Name;
                currentCourseWordSuite.WordsuiteId = courseWordSuites[i].WordSuiteId;
                currentCourseWordSuite.Creator = courseWordSuites[i].Creater;
                currentCourseWordSuite.WordsuiteLanguage = courseWordSuites[i].Language;
                currentCourseWordSuite.TranslationsCount = courseWordSuites[i].Translations.Count;
                currentCourseWordSuite.CurrentCourse = currentCourse;

                currentCourseWordsuites.Add(currentCourseWordSuite);
            }
            return currentCourseWordsuites;
        }

        public IList<CourseWordSuitesModel> GetCourseWordSuites(int courseId, int studentId)
        {
            IList<CourseWordSuitesModel> currentCourseWordsuites = new List<CourseWordSuitesModel>();

            Course currentCourse = _unit.GetRepository<Course>()
                .GetByID(courseId);
            List<WordSuite> courseWordSuites =
                _unit.GetRepository<WordSuite>()
                .Get(ws => ws.CourseId == currentCourse.Id)
                .ToList();

            for (int i = 0; i < courseWordSuites.Count; i++)
            {
                CourseWordSuitesModel currentCourseWordSuite = new CourseWordSuitesModel();
                Profile currentStudentProfile = _unit.GetRepository<Profile>().GetByID(studentId);

                currentCourseWordSuite.WordsuiteName = courseWordSuites[i].Name;
                currentCourseWordSuite.WordsuiteId = courseWordSuites[i].WordSuiteId;
                currentCourseWordSuite.Creator = courseWordSuites[i].Creater;
                currentCourseWordSuite.WordsuiteLanguage = courseWordSuites[i].Language;
                currentCourseWordSuite.TranslationsCount = courseWordSuites[i].Translations.Count;
                currentCourseWordSuite.CurrentCourse = currentCourse;
                currentCourseWordSuite.Progress = _student.WordSuiteProgress(courseWordSuites[i].WordSuiteId, studentId);
                currentCourseWordSuite.StudentFirstName = currentStudentProfile.FirstName;
                currentCourseWordSuite.StudentLastName = currentStudentProfile.LastName;
                currentCourseWordSuite.StudentId = studentId;
                currentCourseWordSuite.CourseStatus =
                    _unit.GetRepository<Progress>()
                        .Get(prg => prg.StudentId == studentId && prg.CourceId == courseId)
                        .First()
                        .Status;

                currentCourseWordsuites.Add(currentCourseWordSuite);
            }
            return currentCourseWordsuites;
        }

        public IList<WordSuiteTranslationsModel> GetWordSuiteTranslations(int wordsuiteId)
        {
            WordSuite currentWordSuite = _unit.GetRepository<WordSuite>().GetByID(wordsuiteId);
            List<Translation> currentTranslations = currentWordSuite.Translations.ToList();

            List<WordSuiteTranslationsModel> translationsList = new List<WordSuiteTranslationsModel>();

            for (int i = 0; i < currentTranslations.Count; i++)
            {
                WordSuiteTranslationsModel currentWordsuiteTranslation = new WordSuiteTranslationsModel();
                currentWordsuiteTranslation.CurrentWordSuite = currentWordSuite;
                currentWordsuiteTranslation.OriginalItem = currentTranslations[i].OriginalItem.Word;
                currentWordsuiteTranslation.TranslatedItem = currentTranslations[i].TranslationItem.Word;
                currentWordsuiteTranslation.PartOfSpeech = currentTranslations[i].PartOfSpeach.ToString();
                currentWordsuiteTranslation.Transcription = currentTranslations[i].OriginalItem.Transcription;
                currentWordsuiteTranslation.TranslationTags = currentTranslations[i].WordType;

                translationsList.Add(currentWordsuiteTranslation);
            }
            return translationsList;
        }

        public IList<WordSuiteTranslationsModel> GetWordSuiteTranslations(int wordsuiteId, int studentId)
        {
            WordSuite currentWordSuite = _unit.GetRepository<WordSuite>().GetByID(wordsuiteId);
            Profile currentStudentProfile = _unit.GetRepository<Profile>().GetByID(studentId);
            List<Translation> currentTranslations = currentWordSuite.Translations.ToList();

            List<WordSuiteTranslationsModel> translationsList = new List<WordSuiteTranslationsModel>();

            for (int i = 0; i < currentTranslations.Count; i++)
            {
                WordSuiteTranslationsModel currentWordsuiteTranslation = new WordSuiteTranslationsModel();
                Translation currentTranslation = currentTranslations[i];
                currentWordsuiteTranslation.CurrentWordSuite = currentWordSuite;
                currentWordsuiteTranslation.OriginalItem = currentTranslations[i].OriginalItem.Word;
                currentWordsuiteTranslation.TranslatedItem = currentTranslations[i].TranslationItem.Word;
                currentWordsuiteTranslation.PartOfSpeech = currentTranslations[i].PartOfSpeach.ToString();
                currentWordsuiteTranslation.Transcription = currentTranslations[i].OriginalItem.Transcription;
                currentWordsuiteTranslation.TranslationTags = currentTranslations[i].WordType;
                currentWordsuiteTranslation.StudentFirstName = currentStudentProfile.FirstName;
                currentWordsuiteTranslation.StudentLastName = currentStudentProfile.LastName;
                currentWordsuiteTranslation.StudentId = studentId;
                currentWordsuiteTranslation.TranslationProgress = _unit.GetRepository<LearningWords>()
                    .Get(trs =>
                        trs.WordSuiteId == wordsuiteId && trs.StudentId == studentId &&
                        trs.TranslationId == currentTranslation.TranslationId).FirstOrDefault();

                translationsList.Add(currentWordsuiteTranslation);
            }
            return translationsList;
        }

        public IList<Course> GetAllCourses()
        {
            UnitOfWork unit = new UnitOfWork();

            List<Course> allCourses = unit.GetRepository<Course>().Get(course => course.Visible == true).ToList();

            return allCourses;
        }

        public IList<WordSuite> GetAllWordSuites(int courseId)
        {
            List<WordSuite> allWordSuites = _unit.GetRepository<WordSuite>().Get(ws => ws.Visible == true).ToList();
            Course currentCourse = _unit.GetRepository<Course>().GetByID(courseId);
            List<WordSuite> currentCourseWordSuites = _unit.GetRepository<WordSuite>()
                .Get(ws => ws.CourseId == courseId)
                .ToList();
            if (currentCourse.Language != null)
            {
                List<WordSuite> currentLangugeWordSuites =
                    _unit.GetRepository<WordSuite>()
                        .Get(ws => ws.Language.Lang == currentCourse.Language.Lang && ws.Visible == true)
                        .ToList();

                IEnumerable<WordSuite> distinctWordSuits = currentLangugeWordSuites.DistinctBy(WS => WS.Name);
                currentLangugeWordSuites = distinctWordSuits.ToList();
                return currentLangugeWordSuites.ExceptBy(currentCourseWordSuites, ws => ws.Name).ToList();
            }
            else
            {
                IEnumerable<WordSuite> distinctWordSuits = allWordSuites.DistinctBy(WS => WS.Name);
                allWordSuites = distinctWordSuits.ToList();
                return allWordSuites.ExceptBy(currentCourseWordSuites, ws => ws.Name).ToList();
            }
        }

        public Course CreateNewCourse(string courseName, string courseLanguage, int userId)
        {
            if (courseLanguage != "" && courseLanguage != "Multilingual" && courseName != "")
            {
                Language currentLanguage =
                    _unit.GetRepository<Language>().Get(lang => lang.Lang == courseLanguage).First();
                _unit.GetRepository<Course>().Insert(new Course { Name = courseName, Visible = true, LanguageId = currentLanguage.Id, UserId = userId });
            }
            if (courseLanguage == "Multilingual" && courseName != "")
            {
                _unit.GetRepository<Course>().Insert(new Course { Name = courseName, Visible = true, UserId = userId });
            }
            _unit.Save();
            return null;
        }

        public void AddWordsuites(int courseId, int wordSuiteId)
        {
            WordSuite currentWordSuite =
                _unit.GetRepository<WordSuite>().GetByID(wordSuiteId);

            WordSuite newWS = new WordSuite
            {
                Name = currentWordSuite.Name,
                CourseId = courseId,
                LanguageId = currentWordSuite.LanguageId,
                Translations = currentWordSuite.Translations,
                Creater = currentWordSuite.Creater
            };
            _unit.GetRepository<WordSuite>().Insert(newWS);
            _unit.Save();
        }

        public void RemoveCourse(int courseId)
        {
            _unit.GetRepository<Course>().Delete(courseId);
            _unit.Save();
        }

        public void RemoveWordsuite(int wordsuiteId)
        {
            _unit.GetRepository<WordSuite>().Delete(wordsuiteId);
            _unit.Save();
        }

        public void AddItemsToWordSuite(List<int> translationsId, string wordSuiteName, string wordSuiteLanguage)
        {
            int currentUserId = ((User)HttpContext.Current.Profile["User"]).Id;

            WordSuite wordSuite =
                _unit.GetRepository<WordSuite>()
                    .Get(wordsuite => wordsuite.Name.ToLower() == wordSuiteName.ToLower()
                        && wordsuite.Visible == true && wordsuite.UserId == currentUserId).FirstOrDefault();

            List<Translation> translations =
                _unit.GetRepository<Translation>()
                    .Get(translation => translationsId.Contains(translation.TranslationId))
                    .ToList();

            Language language = _unit.GetRepository<Language>()
                    .Get(lang => lang.Lang.ToLower() == wordSuiteLanguage.ToLower())
                    .First();

            if (wordSuite == null)
            {

                wordSuite = new WordSuite
                {
                    Name = wordSuiteName,
                    Translations = translations,
                    UserId = currentUserId,
                    Visible = true,
                    CourseId = BasicCourse,
                    Language = language
                };
                _unit.GetRepository<WordSuite>().Insert(wordSuite);
            }

            else
            {
                foreach (var tr in translations)
                    wordSuite.Translations.Add(tr);
            }
            _unit.Save();
        }


        
    }
}