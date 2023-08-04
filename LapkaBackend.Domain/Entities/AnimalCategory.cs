using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Domain.Entities
{
    public sealed class AnimalCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<Animal>? Animals { get; set; }
    }
}
