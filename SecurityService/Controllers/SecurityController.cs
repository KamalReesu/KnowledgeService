using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SecurityService.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SecurityService.Controllers
{

    [ApiController]

    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/lms")]
    public class SecurityController : Controller
    {
        private IConfiguration _config;

        private LmsDBContext dBContext = new LmsDBContext();

        public SecurityController(IConfiguration config)
        {
            _config = config;
        }

            [Route("login")]
            [AllowAnonymous]
            [HttpPost]
            public IActionResult Login([FromBody] Login login)
            {
                IActionResult response = Unauthorized();
                var user = AuthenticateUser(login);

                if (user != null)
                {
                    var tokenString = GenerateJSONWebToken(user);
                    response = Ok(new { token = tokenString, user.UserName, status = "Successful!" });
                }

                return response;
            }

            private string GenerateJSONWebToken(ViewProfile userInfo)
            {

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.EmailId),
                //new Claim("DateOfJoing", userInfo.DateOfJoing.ToString("yyyy-MM-dd")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
                var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Issuer"], claims,
                  expires: DateTime.Now.AddMinutes(60),
                  signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            private ViewProfile AuthenticateUser(Login login)
            {
                ViewProfile user = null;
                try
                {
                    using (dBContext = new LmsDBContext())
                    {
                    var userOut = (from u in dBContext.Users
                                   where (u.LmsEmailId == login.EmailId || u.LmsUserName == login.UserName) & u.LmsUserPassword == login.UserPassword
                                   select new
                                   {
                                       EmailAddress = u.LmsEmailId,
                                       userName = u.LmsUserName,
                                       mobileNo = u.LmsMobileNumber,
                                       isAdmin = u.IsAdmin
                                       //LmsEmailId.Remove(u.LmsEmailId.IndexOf("@")),
                                   }).FirstOrDefault();
                    if(userOut == null)
                    {
                        NotFound();
                    }
                    else
                    {
                        user = new ViewProfile()
                        {
                            UserName = userOut.userName,
                            EmailId = userOut.EmailAddress,
                            MobileNumber = userOut.mobileNo,
                            IsAdmin = userOut.isAdmin
                        };
                    }
                    

                }
                  
                }
                catch (Exception e)
                {
                    Unauthorized();
                }



                return user;
            }


            [Route("register")]
            [AllowAnonymous]
            [HttpPost]
            public IActionResult Register([FromBody] User newuser)
            {
                IActionResult response = Unauthorized();
                try
                {
                using (dBContext = new LmsDBContext())
                {
                    var checkuser = dBContext.Users.Any(u => u.LmsEmailId == newuser.LmsEmailId && newuser.LmsUserName == u.LmsUserName);
                    if (!checkuser)
                    {

                        var n = new User
                        {

                            LmsEmailId = newuser.LmsEmailId,
                            LmsUserPassword = newuser.LmsUserPassword,
                            LmsUserName = newuser.LmsUserName,
                            LmsMobileNumber = newuser.LmsMobileNumber,
                          IsAdmin = newuser.IsAdmin,
                        };
                        dBContext.Users.Add(n);
                        dBContext.SaveChanges();
                        response = Ok(new { username = newuser.LmsUserName, status = "Successfully Registered!" });
                    }
                    else
                    {
                        response = NotFound(new { status = "User Already Exists!! Please try again later" });
                    }
                }


            }
                catch (Exception e)
                {
                    response = Unauthorized();

                }
                return response;
            }


            [Route("Viewprofile")]
            [AllowAnonymous]
            [HttpGet]
            public IActionResult ViewProfile([FromQuery] Login login)
            {
                IActionResult response = Unauthorized();
                var user = AuthenticateUser(login);

                if (user != null)
                {
                    var tokenString = GenerateJSONWebToken(user);
                    response = Ok(new { token = tokenString, user, status = "Successful!" });
                }

                return response;
            }
        }
 }

