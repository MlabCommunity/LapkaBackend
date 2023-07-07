using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        //TODO: Odkomentować atrybuty w końcowym projekcie
        //[Required] 
        //[MaxLength(255)]
        //[EmailAddress]
        public string Email { get; set; }
        //[Required]
        //[MinLength(8)]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Hasło musi zawierać co najmniej jedną małą literę, jedną wielką literę i jedną cyfrę.")]
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpire { get; set; }
    }
}
