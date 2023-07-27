﻿using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Infrastructure.ModelBuilders
{
    public class PhotoModelBuilder
    {
        public static void BuildPhotoModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Photo>(p =>
            {
                p.Property(x => x.FilePath)
                .IsRequired();

                p.HasOne(r => r.Animal)
                .WithMany(u => u.Photos);
            });
        }
    }   
}
