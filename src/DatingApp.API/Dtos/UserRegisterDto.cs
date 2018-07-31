namespace DatingApp.API.Dtos
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UserRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string KnownAs { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required] 
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Invalid Password.")]
        public string Password { get; set; }

        public UserRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}