﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Dtos
{
    public class UserUpdateDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string Usertype { get; set; }
        [Required]
        [MaxLength(250)]
        public string Province { get; set; }
    }
}
