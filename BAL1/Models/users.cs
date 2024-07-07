using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL1.Models
{
    public class users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }

        public string Email { get; set; }
        public string Role { get; set; }
    }
}
