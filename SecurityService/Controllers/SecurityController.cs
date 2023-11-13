using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    [Route("api/v{v:apiVersion}/lms/user")]
    public class SecurityController : Controller
    {
        private IConfiguration _config;

        private readonly UserdbContext dBContext;

        private readonly IRabbitMQConsumer _consumer;

        private readonly ILogger<SecurityController> _logger;
        public SecurityController(IConfiguration config,UserdbContext db, IRabbitMQConsumer consumer, ILogger<SecurityController> logger)
        {
            dBContext = db;
            _config = config;
            _consumer = consumer;
            _logger = logger;
        }

            [Route("login")]
            [AllowAnonymous]
            [HttpPost]
            public IActionResult Login([FromBody] Login login)
            {
                IActionResult response = Unauthorized();
                var user = AuthenticateUser(login);
            try
            {

                if (user != null)
                {
                    var tokenString = GenerateJSONWebToken(user);
                    response = Ok(new { token = tokenString, user.UserName, status = "Login Successful!" });
                    _logger.LogInformation("A user with " + login.UserName + " has logged in at {date}", DateTime.UtcNow);
                }

            }
            catch(Exception e)
            {
                response = Unauthorized();
            }

                return response;
            }

            private string GenerateJSONWebToken(ViewProfile userInfo)
            {

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                    new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.EmailId),
                new Claim(ClaimTypes.Role,(userInfo.IsAdmin)? "Administrator":"User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
                var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims,
                  expires: DateTime.Now.AddMinutes(15),
                  signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            private ViewProfile AuthenticateUser(Login login)
            {
                ViewProfile user = null;
                try
                {
                    var userOut = (from u in dBContext.Users
                                   where (u.LmsEmailId == login.EmailId && u.LmsUserName == login.UserName) & u.LmsUserPassword == login.UserPassword
                                   select new
                                   {
                                       EmailAddress = u.LmsEmailId,
                                       userName = u.LmsUserName,
                                       mobileNo = u.LmsMobileNumber,
                                       isAdmin = u.IsAdmin
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
                        response = Ok(new { username = newuser.LmsUserName, status = "Successfully Registered! Please Continue to login!!" });
                        _logger.LogInformation("A user with " + newuser.LmsUserName + " has successfully registered in at {date}", DateTime.UtcNow);
                    }
                    else
                    {
                        response = NotFound(new { status = "User Already Exists!! Please try again later!!" });
                        _logger.LogError("A user with " + newuser.LmsUserName + " tried to login at {date}", DateTime.UtcNow);
                    }
    
            }
                catch (Exception e)
                {
                    response = Unauthorized();

                }
                return response;
            }


            [Route("Viewprofile")]
            [Authorize]
            [HttpGet]
            public IActionResult ViewProfile([FromBody] Login login)
            {
                IActionResult response = Unauthorized();
                var user = AuthenticateUser(login);
                try
                {
                    if (user != null)
                    {
                        //Rabbit MQ Messsage Consuming..
                        //_consumer.ConsumeMessage();
                        response = Ok(new { name = user.UserName, isAdmin = user.IsAdmin });
                    }
                    else
                    {
                        response = NotFound(new { status = "User not found!!! Please try again with different credentials!!" });
                    }
                }
                    catch (Exception e)
                    {
                        response = Unauthorized();

                    }


                    return response;
            }
        }
 }

