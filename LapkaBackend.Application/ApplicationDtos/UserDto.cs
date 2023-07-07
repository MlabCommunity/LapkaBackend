﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.ApplicationDtos
{
    public class UserDto
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? emailAddress { get; set; }
        public string? password { get; set; }
        public string? confirmPassword { get; set; }
        //public char Role { get; set; } // role u - user, w - worker, a - admin, s - superAdmin
        public enum Role { user,worker,admin,superAdmin }
    }
}
