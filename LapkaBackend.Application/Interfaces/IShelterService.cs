using LapkaBackend.Application.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Interfaces
{
    public interface IShelterService
    {
        public Task UpdateShelter(ShelterRegistrationRequest request);

    }
}
