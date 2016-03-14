using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLayer.Interfaces;
using DataAccessLayer.Entities;
using DataAccessLayer.UnitOfWork;
using DataAccessLayer.Enums;

namespace BusinessLayer.Managers
{
    public class ManagerManager : IManagerManager
    {
        private IUnitOfWork _unitOfWork;

        public ManagerManager()
        {
            _unitOfWork = new UnitOfWork();
        }

        public ManagerManager(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Course> AllCourses()
        {
            var courses = _unitOfWork.GetRepository<Course>().Get(course => course.Visible == true,
                         null, "Language");

            return courses;
        }

        public IEnumerable<Course> GetNotAssignCourses(int studentId)
        {
            IEnumerable<Progress> studentCourses = GetStudentCourse(studentId);

            var studcourses = studentCourses.Select(c => c.Course);

            var allcourses = AllCourses();
            IList<Course> allcoursesresult = new List<Course>();

            if (studcourses.Count() == 0) { return allcourses; }

            if (allcourses.Count() == studcourses.Count()) { return allcoursesresult; }

            foreach (var item in allcourses)
            {
                if (studcourses.All(a => a.Id != item.Id))
                {
                    allcoursesresult.Add(item);
                }
            }
            return allcoursesresult.ToArray();
        }

        public IEnumerable<User> GetTeachers()
        {
            var teachers =
                  _unitOfWork.GetRepository<User>().Get(s => s.PersonRole == PersonRole.Teacher, null
                                  , "Profile");

            return teachers;
        }

        public IEnumerable<User> GetAllStudents()
        {
            var students =
                _unitOfWork.GetRepository<User>().Get(s => s.PersonRole == PersonRole.Student, null
                , "Profile");

            return students;
        }

        public void RemoveStudentCourse(int id)
        {
            _unitOfWork.GetRepository<Progress>().Delete(id);
            _unitOfWork.Save();
        }

        public void AssignTeacher(int teacherId, int courseId)
        {
            var newprogress = _unitOfWork.GetRepository<Progress>().GetAll().Where(p => p.CourceId == courseId);

            foreach (var p in newprogress)
            {
                p.TeacherId = teacherId;
                _unitOfWork.GetRepository<Progress>().Update(p);
            }

            _unitOfWork.Save();
        }

        public void AssignCourse(int courseId, int studentId)
        {
            Course course = _unitOfWork.GetRepository<Course>().GetByID(courseId);

            Progress newprogress = new Progress();
            newprogress.CourceId = courseId;
            newprogress.Course = course;
            newprogress.StartDate = DateTime.Now;
            newprogress.Status = 0;
            newprogress.StudentId = studentId;
            newprogress.TeacherId = course.UserId;

            User teacher = _unitOfWork.GetRepository<User>().GetByID(course.UserId);

            newprogress.Teacher = teacher;

            _unitOfWork.GetRepository<Progress>().Insert(newprogress);
            _unitOfWork.Save();
        }

        public IEnumerable<Progress> GetStudentCourse(int studId)
        {
            var studentcourses = _unitOfWork.GetRepository<Progress>()
                .Get(progress => progress.StudentId == studId, null
                , "Student.User.Profile,Teacher.Profile,Course,Course.Language")
                .Where(progress => progress.Course.Visible == true);
            return studentcourses;
        }

        public string GetHeaderName(int studentId)
        {
            var student = _unitOfWork.GetRepository<Profile>().GetByID(studentId).FullName;

            return student;
        }
    }
}
