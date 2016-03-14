using System.Web;
using DataAccessLayer.Entities;

namespace WordLearningMVC
{
    public class CurrentUserInfo
    {

        public CurrentUserInfo()
        {
            if (HttpContext.Current.Profile != null)
            {
                if (HttpContext.Current.Profile.UserName != null)
                    User = (User) HttpContext.Current.Profile["User"];
                else
                    User = null;
            }
            else
                User = null;

        }

        public User User { get; private set; }
    }
}