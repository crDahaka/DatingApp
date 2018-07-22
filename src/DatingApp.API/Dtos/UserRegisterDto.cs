namespace DatingApp.API.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class UserRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Invalid Password.")]
        public string Password { get; set; }
    }
}