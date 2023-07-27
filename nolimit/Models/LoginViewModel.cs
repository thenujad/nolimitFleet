﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


public class LoginViewModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

}
