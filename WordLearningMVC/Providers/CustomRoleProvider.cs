using System;
using System.Linq;
using System.Web.Security;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.UnitOfWork;

namespace WordLearningMVC.Providers
{
    public class CustomRoleProvider : RoleProvider
    {
        public override string[] GetRolesForUser(string login)
        {
            string[] role = new string[] { };
            using (FinalWordLearn context = new FinalWordLearn())
            {
                try
                {
                    UnitOfWork unit = new UnitOfWork(context);
                    
                    User user = (from u in unit.GetRepository<User>().GetAll()
                                 where u.Login == login
                                 select u).FirstOrDefault();
                    if (user != null)
                    {
                        // Отримуєм роль
                        PersonRole userRole = user.PersonRole;

                        if (userRole != null)
                        {
                            role = new string[] {userRole.ToString()};
                        }
                    }
                }
                catch
                {
                    role = new string[] { };
                }
            }
            return role;
        }

        public bool IsUserInRole(string username, PersonRole role)
        {
            bool isUserInRole = false;
            
            using (FinalWordLearn context = new FinalWordLearn())
            {
                try
                {
                    UnitOfWork unit = new UnitOfWork(context);

                    User user = (from u in unit.GetRepository<User>().GetAll()
                                 where u.Login == username
                                 select u).FirstOrDefault();
                    if (user != null)
                    {

                        if (user.PersonRole != null && user.PersonRole == role)
                        {
                            isUserInRole = true;
                        }
                    }
                }
                catch
                {
                    isUserInRole = false;
                }
            }
            return isUserInRole;
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();

        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }


        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}