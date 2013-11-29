using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Examonitor.Models
{
    public class MyUser : IdentityUser
    {
        public string Email { get; set; }
        public virtual ICollection<ToDo> ToDoes { get; set; }
    }

    public class Campus
    {
        public int Id { get; set; }
        [Display(Name = "Campus Naam")]
        public string Name { get; set; }
    }

    public class ToDo
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
        public virtual MyUser User { get; set; }
    }
    public class MyDbContext : IdentityDbContext<MyUser>
    {
        public MyDbContext()
            : base("Examonitor")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>()
                .ToTable("Users");
            modelBuilder.Entity<MyUser>()
                .ToTable("Users");
        }

        public System.Data.Entity.DbSet<Examonitor.Models.ToDo> ToDoes { get; set; }
        public DbSet<MonitorBeurtModel> MonitorBeurt { get; set; }
        public DbSet<ReservatieModel> Reservatie { get; set; }
        public DbSet<Campus> Campus { get; set; }
    }

}