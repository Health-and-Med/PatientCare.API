﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientCare.Domain.Entities
{
    public class LoginModel
    {
        public string? Email { get; set; }

        public string? Cpf {  get; set; }
        public string Password { get; set; }
    }
}

