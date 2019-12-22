using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.Entities
{
    public class ContactPerson
    {
        public long Id { get; set; }

        public string FirstName { get; set; }
        public string SecondName { get; set; }

        public string Phone { get; set; }
        public bool IsActive { get; set; }

        public string Email { get; set; }
    }
}