namespace DatingApp.API.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class UserLoginDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}