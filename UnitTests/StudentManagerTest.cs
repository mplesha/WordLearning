using BusinessLayer.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace UnitTests
{
    [TestClass]
    public class StudentManagerTest
    {
        private const int wordSuiteId=4;
        private StudentManager student = new StudentManager();

        
        [TestMethod]
        public void IsWordSuteLanguageCorrect()
        {
           
            Assert.AreEqual("German", student.GetWordSuiteLanguage(wordSuiteId));
        }

        [TestMethod]
        public void IsWordSuteNameCorrect()
        {

            Assert.AreEqual("German terminology", student.GetWordSuiteName(wordSuiteId));
        }
    }
}
