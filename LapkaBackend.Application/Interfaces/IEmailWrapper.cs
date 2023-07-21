using LapkaBackend.Application.Enums;
using LapkaBackend.Application.Helper;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Interfaces
{
    public interface IEmailWrapper
    {
        BodyBuilder GetBuilder(Templates template);
    }
}
