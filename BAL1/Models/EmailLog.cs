using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL1.Models;

    public partial class EmailLog
{
    public int Id { get; set; }
    public string EmailTo { get; set; }
    // public string EmailFrom { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public DateTime SentDate { get; set; }
}


