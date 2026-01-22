using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Service
{
    internal class authService
    {
        public static bool IsLoggedIn { get; private set; }
        public static string UserName { get; private set; }
        public static string Token { get; private set; }

        public static bool AuthLogin(string username, string password)
        {
       
            if (username == "admin" && password == "123456")
            {
                IsLoggedIn = true;
                UserName = username;
                Token = "fake-jwt-token";
                return true;
            }
            return false;
        }

        public static void Logout()
        {
            IsLoggedIn = false;
            UserName = null;
            Token = null;
        }
    }
}
