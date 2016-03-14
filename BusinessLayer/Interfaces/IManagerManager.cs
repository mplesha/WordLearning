using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using BusinessLayer.Models;

namespace BusinessLayer.Interfaces
{
    public interface IManagerManager
    {
        IEnumerable<User> GetTeachers();
        IEnumerable<User> GetAllStudents();
        void RemoveStudentCourse(int id);
        void AssignTeacher(int teacherId, int progressId);
        void AssignCourse(int courseId, int studentId);
        IEnumerable<Course> AllCourses();
        IEnumerable<Course> GetNotAssignCourses(int studentId);
        IEnumerable<Progress> GetStudentCourse(int courseId);
        string GetHeaderName(int studentId);
    }
}
