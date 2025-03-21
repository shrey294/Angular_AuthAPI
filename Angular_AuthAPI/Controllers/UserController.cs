using Angular_AuthAPI.Context;
using Angular_AuthAPI.Helpers;
using Angular_AuthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Angular_AuthAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly AppDbContext _context;
		public UserController(AppDbContext appDbContext)
		{
			_context = appDbContext;
		}
		[HttpPost("authenticate")]
		public async Task<IActionResult> Authenticate([FromBody] User user)
		{
			try
			{
				if (user == null)
				{
					return BadRequest();
				}
				else
				{
					var User = await _context.Users.FirstOrDefaultAsync(x => x.Username == user.Username);
					if (User == null)
					{
						return NotFound(new { Message = "User Not Found" });
					}
					if (!PasswordEncrypt.verifyPassword(user.Password, User.Password))
					{
						return BadRequest(new { Message = "Password is incorrect" });
					}
					user.Token = CreateJwttoken(User);

					return Ok(new {Token=user.Token ,Message = "Login Success" });

				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpPost("register")]
		public async Task<IActionResult> RegisterUser([FromBody] User user)
		{
			try
			{
				if (user == null)
				{
					return BadRequest();
				}
				if (await usernameexisits(user.Username))
				{
					return BadRequest(new { Message = "Username Already Exists" });
				}
				if (await Emailexisits(user.Email))
				{
					return BadRequest(new { Message = "Email Already Exists" });
				}
				var pass = passwordstrength(user.Password);
				if (!string.IsNullOrEmpty(pass))
				{
					return BadRequest(new { Message = pass.ToString() });
				}
				else
				{
					user.Password = PasswordEncrypt.HashedPassword(user.Password);
					user.Role = "User";
					user.Token = "";
					await _context.Users.AddAsync(user);
					await _context.SaveChangesAsync();
					return Ok(new { Message = "User Registered" });
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpGet]
		public async Task<ActionResult<User>> GetAllUsers()
		{
			return Ok(await _context.Users.ToListAsync());
		}
		private async Task<bool> usernameexisits(string username)
		{
			return await _context.Users.AnyAsync(x => x.Username == username);
		}
		private async Task<bool> Emailexisits(string email)
		{
			return await _context.Users.AnyAsync(x => x.Email == email);
		}

		private string passwordstrength(string password)
		{
			StringBuilder sb = new StringBuilder();

			// Check for minimum length of 8 characters
			if (password.Length < 8)
			{
				sb.Append("Minimum password length should be 8" + Environment.NewLine);
			}

			// Check if password contains at least one lowercase, one uppercase, and one digit
			if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
			{
				sb.Append("Password should be alphanumeric" + Environment.NewLine);
			}

			// Check if password contains at least one special character
			if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':]"))
			{
				sb.Append("Password should contain special characters" + Environment.NewLine);
			}

			return sb.ToString();
		}
		private string CreateJwttoken(User user)
		{
			var jwtTokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes("veryveryscerete.....");
			var identity = new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Role, user.Role),
				new Claim(ClaimTypes.Name,$"{user.FirstName} {user.LastName}")
			});
			var Credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = identity,
				Expires = DateTime.Now.AddDays(1),
				SigningCredentials = Credentials,
			};
			var token = jwtTokenHandler.CreateToken(tokenDescriptor);
			return jwtTokenHandler.WriteToken(token);
		}
	}
}
