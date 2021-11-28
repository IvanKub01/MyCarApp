using API.Data;
using API.DTO;

using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }
        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDTO  registerDTO)
        {
            if (await UserExists(registerDTO.UserName)) return BadRequest("UserName is taken");
            using var hmac = new HMACSHA512();


            var User = new AppUser
            {

                UserName = registerDTO.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return User;
        }
        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDTO  loginDTO){
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);        
            if(user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash =  hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }
            return user;
        }

        private async Task<bool> UserExists(string username)
        {

            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}