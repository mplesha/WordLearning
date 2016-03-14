using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.UnitOfWork;

namespace generatorDB.Users
{
    class StudentsGenerator
    {
        public static void Generate()
        {
            using (FinalWordLearn context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);
                List<int> studetntsDBId = unit.GetRepository<Student>().GetAll().Select(student => student.Id).ToList();
                List<int> studetntsId = unit.GetRepository<User>().Get(user => user.PersonRole == PersonRole.Student).Select(user => user.Id).ToList();
                studetntsId = studetntsId.Except(studetntsDBId).ToList();
                List<Student> students = new List<Student>();
                foreach (var id in studetntsId)
                {
                    students.Add(new Student { Id = id });
                }
                unit.GetRepository<Student>().Insert(students);
                unit.Save();

            }
        }
    }
}
