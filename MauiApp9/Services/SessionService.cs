using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp9.Services
{
    public class SessionService
    {
        private static SessionService _instance;

        public static SessionService Instance => _instance ??= new SessionService();
        public int UserId { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }

        private SessionService() { }

        public void Clear()
        {
            UserId = 0;
            FName = string.Empty;
            LName = string.Empty;
            Email = string.Empty;
        }
    }
}
