using System.Linq;
using DataAccessLayer.Entities;
using DataAccessLayer.UnitOfWork;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class RepositoryTest
    {
        [TestMethod]
        public void GetAllTest()
        {
            var context = new FinalWordLearn();
            var unit = new UnitOfWork(context);

            var count = unit.GetRepository<User>().GetAll().Count();

            Assert.AreEqual(15, count, "Error");
        }
    }
}
