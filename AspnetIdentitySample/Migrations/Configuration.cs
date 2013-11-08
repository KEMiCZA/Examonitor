namespace Examonitor.Migrations
{
    using Examonitor.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Examonitor.Models.ExamenMonitorDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Examonitor.Models.ExamenMonitorDbContext context)
        {
             context.MonitorBeurt.AddOrUpdate(i => i.MonitorBeurtId,
                new MonitorBeurtModel
                {
                    Datum = DateTime.Parse("2013/10/18"),
                    Start = "8:30",
                    Einde = "10:30",
                    Duurtijd = "2:30",
                    Capaciteit = 2,
                    Gereserveerd = 0,
                    AangemaaktOp = DateTime.Parse("2013/10/18"),
                    Aangepast = DateTime.Parse("2013/10/18"),
                    Soort = "Digitaal",
                    Campus = "BouwmeesterStraat",
                    Departement = "ONDERWIJS EN TRAINING"

                },
                new MonitorBeurtModel
                {
                    Datum = DateTime.Parse("2013/10/18"),
                    Start = "8:30",
                    Einde = "10:30",
                    Duurtijd = "2:30",
                    Capaciteit = 2,
                    Gereserveerd = 0,
                    AangemaaktOp = DateTime.Parse("2013/10/18"),
                    Aangepast = DateTime.Parse("2013/10/18"),
                    Soort = "Digitaal",
                    Campus = "Meistraat",
                    Departement = "IT"

                });
       
        }
    }
}
