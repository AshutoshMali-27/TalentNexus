using DAL1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Data;
using TalentNexus.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using TalentNexus.Logfile;
using BAL1.Models;
using Microsoft.AspNetCore.Routing;

namespace TalentNexus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //private readonly AuthService _AuthService = new AuthService();

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly EmailLogService _emailLogService;
        private readonly LinkGenerator _linkGenerator;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, EmailLogService emailLogServic, LinkGenerator linkGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailLogService = emailLogServic;
            _linkGenerator = linkGenerator;

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistration model)
        {

            var user = new IdentityUser { UserName = model.Username };
            var MobileNumber = new IdentityUser { PhoneNumber = model.PhoneNumber };
            var Email = new IdentityUser { Email = model.Emailid };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(model.Role));
                }

                await _userManager.AddToRoleAsync(user, model.Role);

                return Ok(new { Username = user.UserName, Role = model.Role });
            }

            return BadRequest(result.Errors);
        }

        //    [HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] UserRegistration model)
        //{
        //    var user = new IdentityUser { UserName = model.Username, Email = model.Email, PhoneNumber = model.PhoneNumber };
        //    var result = await _userManager.CreateAsync(user);

        //    if (result.Succeeded)
        //    {
        //        if (!await _roleManager.RoleExistsAsync("User"))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole("User"));
        //        }

        //        await _userManager.AddToRoleAsync(user, "User");

        //        // Generate the email confirmation token
        //        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        //        // Construct the password setting link
        //        //      var setPasswordLink = Url.Action(nameof(SetPassword), "Account", new { userId = user.Id, token = token }, Request.Scheme);
        //      //   var setPasswordLink = _linkGenerator.GetUriByAction(HttpContext, nameof(SetPassword), "Account", new { userId = user.Id, token = token }, Request.Scheme);
        //        var setPasswordLink1 = GenerateLink(user.Id , token);


        //        await SendEmailAsync(user.Email, "Set your password", $"Please set your password by clicking this link: {setPasswordLink1}");


        //        return Ok(new { Username = user.UserName, Message = "Registration successful. Please check your email to set your password." });
        //    }

        //    return BadRequest(result.Errors);
        //}

        [HttpGet("GenerateLink")]
        public IActionResult GenerateLink(string userId, string token)
        {
            var setPasswordLink = _linkGenerator.GetUriByAction(
                HttpContext,
                action: nameof(SetPassword),
                controller: "AuthController",
                values: new { userId, token },
                scheme: Request.Scheme
            );

            if (setPasswordLink == null)
            {
                Console.WriteLine($"Unable to generate link for userId: {userId}, token: {token}");
                return BadRequest("Unable to generate link");
            }

            return Ok(setPasswordLink);
        }

        private async Task SendEmailAsync(string email, string subject, string message)
        {
            // Use an email service to send the email
            // Example using SendGrid
            var client = new SendGridClient("your_sendgrid_api_key");
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("your_email@example.com", "Your Name"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            await _emailLogService.LogEmailAsync(email, subject, message);

            var response = await client.SendEmailAsync(msg);
            // Handle response as needed
        }

 

        [HttpPost("setpassword")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordModel model)
        {
            if (model.UserId == null || model.Token == null || model.Password == null)
            {
                return BadRequest("Invalid password setting request.");
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return Ok("Password set successfully.");
            }

            return BadRequest(result.Errors);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] userLogin model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                var roles = await _userManager.GetRolesAsync(user);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }.Union(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("sVDSLKSNRSg89359&^#$@!*^$@%!(&%GKFDGKHSGDF9&#6"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "yourdomain.com",
                    audience: "yourdomain.com",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return Unauthorized();
        }



        //[HttpPost("register")]
        //public IActionResult Register([FromBody] UserRegistration model)
        //{
        //    var user = new users
        //    {
        //        Username = model.Username,
        //        Password = model.Password,
        //        Role = model.Role
        //    };

        //    var result = _AuthService.CreateUser(user);

        //    if (result)
        //    {
        //        return Ok(new { Username = user.Username, Role = user.Role });
        //    }

        //    return BadRequest("User registration failed.");
        //}


        //[HttpPost("login")]
        //public IActionResult Login([FromBody] userLogin model)
        //{
        //    var user = _AuthService.GetUser(model.Username, model.Password);

        //    if (user != null)
        //    {
        //        var claims = new[]
        //        {
        //            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
        //        };

        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is secret key SDGKJNLLNGSDLB#@$@%^&$^(^(#%@#%"));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        var token = new JwtSecurityToken(
        //            issuer: "yourdomain.com",
        //            audience: "yourdomain.com",
        //            claims: claims,
        //            expires: DateTime.Now.AddMinutes(30),
        //            signingCredentials: creds);

        //        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        //    }

        //    return Unauthorized();
        //}




    }
}
