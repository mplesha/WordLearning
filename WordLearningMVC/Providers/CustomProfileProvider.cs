using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Profile;
using DataAccessLayer.Entities;
using DataAccessLayer.UnitOfWork;
using WordLearningMVC.Models;

namespace WordLearningMVC.Providers
{
    public class CustomProfileProvider : ProfileProvider
    {
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context,
            SettingsPropertyCollection collection)
        {
            SettingsPropertyValueCollection result = new SettingsPropertyValueCollection();

            if (collection == null || collection.Count < 1 || context == null)
            {
                return result;
            }
            string username = (string) context["UserName"];
            if (String.IsNullOrEmpty(username))
                return result;

            using (FinalWordLearn newContext = new FinalWordLearn())
            {
                try
                {
                    UnitOfWork unit = new UnitOfWork(newContext);
                    int userId = unit.GetRepository<User>().Get(u => u.Login.Equals(username)).FirstOrDefault().Id;
                    Profile profile = unit.GetRepository<Profile>().Get(u => u.Id == userId).FirstOrDefault();
                    if (profile != null)
                    {
                        foreach (SettingsProperty prop in collection)
                        {
                            SettingsPropertyValue svp = new SettingsPropertyValue(prop);
                            svp.PropertyValue = profile.GetType().GetProperty(prop.Name).GetValue(profile, null);
                            result.Add(svp);
                        }
                    }
                    else
                    {
                        foreach (SettingsProperty prop in collection)
                        {
                            SettingsPropertyValue svp = new SettingsPropertyValue(prop);
                            svp.PropertyValue = null;
                            result.Add(svp);
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }
            return result;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            string username = (string)context["UserName"];

            if (username == null || username.Length < 1 || collection.Count < 1)
                return;

            using (FinalWordLearn newContext = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(newContext);
                int userId = unit.GetRepository<User>().Get(u => u.Login.Equals(username)).FirstOrDefault().Id;

                Profile profile = unit.GetRepository<Profile>().Get(u => u.Id == userId).FirstOrDefault();

                if (profile != null)
                {
                    foreach (SettingsPropertyValue val in collection)
                    {
                        profile.GetType().GetProperty(val.Property.Name).SetValue(profile, val.PropertyValue);
                    }
                    //profile.LastUpdateDate = DateTime.Now;
                    unit.Context.Entry(profile).State = EntityState.Modified;
                }
                else
                {

                    profile = new Profile();
                    foreach (SettingsPropertyValue val in collection)
                    {
                        profile.GetType().GetProperty(val.Property.Name).SetValue(profile, val.PropertyValue);
                    }
                    // profile.LastUpdateDate = DateTime.Now;
                    profile.Id = userId;
                    unit.GetRepository<Profile>().Insert(profile);
                }
                unit.Save();
            }
        }

        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
        }

        public override int DeleteProfiles(string[] usernames)
        {
            throw new NotImplementedException();
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
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
    }
}