using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Helpter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmail(Mailrequest request);
    }
}
