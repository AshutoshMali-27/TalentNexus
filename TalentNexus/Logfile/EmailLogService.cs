
using BAL1.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TalentNexus.Auth;


namespace TalentNexus.Logfile
{
    public class EmailLogService {


        private readonly ApplicationDbContext _context;

        public EmailLogService(ApplicationDbContext context)
        {
            _context = context;
        }

     //   private string connectionString = "Data Source=LAPTOP-92R3ST6F\\SQLEXPRESS;Initial Catalog=StudentsDB;Integrated Security=True;";
       public async Task LogEmailAsync(string to ,string subject, string body)
        {
            var emailLog = new EmailLog
            {
                EmailTo = to,
               // EmailFrom = from,
                Subject = subject,
                Body = body,
                SentDate = DateTime.UtcNow
            };


            _context.EmailLogs.Add(emailLog);
            await _context.SaveChangesAsync();
        }

    }
}
