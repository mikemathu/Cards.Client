﻿using System.ComponentModel.DataAnnotations;

namespace Cards.Client.Models
{
    public class AppUser
    {
        [Required(ErrorMessage = "Email is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Email is 30 characters.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is a required field.")]
        [MaxLength(100, ErrorMessage = "Maximum length for the Password is 100 characters.")]
        public string Password { get; set; } = null!;
    }
}
