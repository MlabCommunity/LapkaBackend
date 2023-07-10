using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CreatedAt { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        //public char Role { get; set; } // role u - user, w - worker, a - admin, s - superAdmin
        //public enum Role { user, worker, admin, superAdmin }
        public string RefreshToken { get; set; }
    }
}
