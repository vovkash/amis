using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWriting.Model;

namespace TestWriting
{
    public static class Session
    {
        public enum Role
        { 
            Student,
            Teacher
        }


        static User user;
        static DateTime loginTime;
        static List<Role> roles = new List<Role>();


        public static User User
        {
            get { return user; }
            set { user = value; }
        }

        public static DateTime LoginTime
        {
            get { return loginTime; }
            set { loginTime = value; }
        }

        public static List<Role> Roles
        {
            get { return roles; }
            set { roles = value; }
        }

        public static bool HasRole(Role role)
        {
            return Roles.Exists(x => x == role);     
        }

    }
}
