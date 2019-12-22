using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.Entities
{
    public class User
    {
        public long Id { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string RegistrationStatus { get; set; }
        public string SequrityQuestion { get; set; }
        public string SequrityAnswer { get; set; }

        public DateTime? RegistrationDateTime { get; set; }
        public DateTime? FailedLoginAttemptDateTime { get; set; }
        public int FailedLoginAttemptCounter { get; set; }

        public string FullName { get; set; }
        public int UserRole { get; set; }

        public bool IsActive { get; set; }
    }
}