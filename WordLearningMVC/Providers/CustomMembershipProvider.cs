using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Security;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.UnitOfWork;
using WordLearningMVC.Models;

namespace WordLearningMVC.Providers
{
    public class CustomMembershipProvider : MembershipProvider
    {
        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;

            using (FinalWordLearn context = new FinalWordLearn())
            {
                try
                {
                    User user = FindUsersByName(username).FirstOrDefault();

                    if (user != null && Crypto.VerifyHashedPassword(user.Password, password))
                    {

                        isValid = true;
                    }
                }
                catch (Exception ex)
                {
                    isValid = false;
                }
            }
            return isValid;
        }

        public MembershipUser CreateUser(RegisterModel model)
        {
            
           
            MembershipUser membershipUser = GetUser(model.UserName, false);

            if (membershipUser == null)
            {
                try
                {
                    using (FinalWordLearn context = new FinalWordLearn())
                    {
                        UnitOfWork unit = new UnitOfWork(context);
                        User user = new User();
                        user.Login= model.UserName;
                        user.Password = Crypto.HashPassword(model.Password);
                        user.CreationDate = DateTime.Now;
                        //user.PersonRole = PersonRole.Admin;
                        if (model.PersonRole != PersonRole.Listener)
                        {
                            user.PersonRole = model.PersonRole;
                        }

                        Profile profile = new Profile();
                        profile.FirstName = model.FirstName;
                        profile.LastName = model.LastName;
                        profile.DateOfBirth = model.DateOfBirth;
                        profile.Email = model.Email;
                        profile.Sex = model.Sex;
                        profile.PhoneNumber = model.PhoneNumber;
                        
                        user.Profile = profile;

                        unit.GetRepository<Profile>().Insert(profile);
                        unit.GetRepository<User>().Insert(user);

                        unit.Save();
                        membershipUser = GetUser(model.UserName, false);
                        return membershipUser;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }

 

        public override MembershipUser GetUser(string login, bool userIsOnline)
        {
            try
            {
               
                List<User> users=FindUsersByName(login);
                if (users.Count() > 0)
                {
                    User user = users.First();
                    MembershipUser memberUser = new MembershipUser("MyMembershipProvider", user.Login, null, null, null, null,
                        false, false, user.CreationDate, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
                    return memberUser;
                }
                
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public List<User> FindUsersByName(string username)
        {
            using (FinalWordLearn context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);

                List<User> users = (from u in unit.GetRepository<User>().GetAll()
                    where u.Login.ToLower() == username.ToLower()
                    select u).ToList();
                return users;
            }
        }

        public List<User> FindUsersByEmail(string email)
        {
            using (FinalWordLearn context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);

                List<User> users = (from u in unit.GetRepository<User>().GetAll()
                                    where u.Profile.Email.ToLower() == email.ToLower()
                                    select u).ToList();
                return users;
            }
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

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }
        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }
        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }
        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }
        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }
        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }
        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }
        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }
        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }
        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }
        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }
        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }
        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }
    }
}