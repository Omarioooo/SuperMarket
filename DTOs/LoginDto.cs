﻿using System.ComponentModel.DataAnnotations;

namespace SuperMarket.DTOs
{
    public class LoginDto
    {
        [Required]
        public string? UserNameOrEmail { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}