﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project.Data
{
    public class projectContext : DbContext
    {
        public projectContext (DbContextOptions<projectContext> options)
            : base(options)
        {
        }

        public DbSet<project.Models.allusers> allusers { get; set; } = default!;

        public DbSet<project.Models.items>? items { get; set; }
        public DbSet<project.Models.categories>? categories { get; set; }
        public DbSet<project.Models.issues>? issues { get; set; }


    }
}
