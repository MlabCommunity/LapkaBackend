﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Domain.Entities
{
    public class AnimalView
    {
        public Guid Id { get; set; }

        public Guid AnimalId { get; set; }
        public virtual Animal Animal { get; set; }

        public Guid? UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime ViewDate { get; set; }
    }
}
