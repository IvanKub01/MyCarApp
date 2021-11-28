using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class LoginDTO
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string password { get; set; }
    }
}