using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Domain.Entities
{
    public class Photo
    {
        public Guid Id { get; set; }
        public bool IsProfilePhoto { get; set; }

        [ForeignKey("AnimalId")]
        public Guid? AnimalId { get; set; }
        public virtual Animal? Animal { get; set; }
    }
}
