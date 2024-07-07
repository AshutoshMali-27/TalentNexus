using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BAL1.Models;

namespace TalentNexus.Auth
{
   
        public class ApplicationDbContext : IdentityDbContext<IdentityUser>
        {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        
        
        
        }

        public DbSet<EmailLog> EmailLogs { get; set; }
    }
    
}
