using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Objects
{
    public class UserObject
    {
        public UserObject()
        {
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public bool isActive { get; set; }
        public bool isLocked { get; set; }


    }
}
