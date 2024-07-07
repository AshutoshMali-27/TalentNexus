using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL1.Models
{
    public class SetPasswordModel
    {

        public string UserId { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }

    }
}
