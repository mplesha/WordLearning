using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLayer.Interfaces;
using BusinessLayer.Managers;
using Ninject;
using DataAccessLayer.UnitOfWork;
using WordLearningMVC.Logging;


namespace WordLearningMVC.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver()
        {
            kernel = new StandardKernel();
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            kernel.Bind<IAdminManager>().To<AdminManager>();
            kernel.Bind<IStudentManager>().To<StudentManager>();
            kernel.Bind<ITeacherManager>().To<TeacherManager>();
            kernel.Bind<IDictionaryManager>().To<DictionaryManager>();
            kernel.Bind<IManagerManager>().To<ManagerManager>();
            kernel.Bind<ITutorialManager>().To<TutorialManager>();
            kernel.Bind<IQuizManager>().To<QuizManager>();
            kernel.Bind<IAddTranslationManager>().To<AddTranslationManager>();
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            kernel.Bind<ILogger>().To<MainClassLogger>();
        }
    }
}