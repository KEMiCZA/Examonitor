using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Examonitor.Models
{
    public class ExamenMonitorDbContext : MyDbContext
    {
        public DbSet<MonitorBeurtModel> MonitorBeurt { get; set; }
        public DbSet<ReservatieModel> Reservatie { get; set; }
    }
}