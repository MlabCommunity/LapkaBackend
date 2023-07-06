﻿using LapkaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Infrastructure.Interfaces
{
    public interface IUserService
    {
        public Task<User> FindUserByRefreshToken(string refreshToken);

    }
}
