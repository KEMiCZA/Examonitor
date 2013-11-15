namespace Examonitor.Migrations
{
    using Examonitor.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Examonitor.Models.MyDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Examonitor.Models.MyDbContext context)
        {
            var UserManager = new UserManager<MyUser>(new UserStore<MyUser>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            string name = "Admin";
            string password = "123456";
            string test = "test";

            //Create Role Test and User Test
            RoleManager.Create(new IdentityRole(test));
            var user2 = new MyUser();
            user2.UserName = "User";
            user2.IsConfirmed = true;
            UserManager.Create(user2, "password");

            //Create Role Admin if it does not exist
            if (!RoleManager.RoleExists(name))
            {
                var roleresult = RoleManager.Create(new IdentityRole(name));
            }

            //Create User=Admin with password=123456
            var user = new MyUser();
            user.UserName = name;
            user.IsConfirmed = true;
            var adminresult = UserManager.Create(user, password);

            //Add User Admin to Role Admin
            if (adminresult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, name);
            }
            base.Seed(context);

             //context.MonitorBeurt.AddOrUpdate(i => i.MonitorBeurtId,
             //   new MonitorBeurtModel
             //   {
             //       Datum = DateTime.Parse("2013/10/18"),
             //       Start = "8:30",
             //       Einde = "10:30",
             //       Duurtijd = "2:30",
             //       Capaciteit = 2,
             //       Gereserveerd = 0,
             //       AangemaaktOp = DateTime.Parse("2013/10/18"),
             //       Aangepast = DateTime.Parse("2013/10/18"),
             //       Soort = "Digitaal",
             //       Campus = "BouwmeesterStraat",
             //       Departement = "ONDERWIJS EN TRAINING"

             //   },
             //   new MonitorBeurtModel
             //   {
             //       Datum = DateTime.Parse("2013/10/18"),
             //       Start = "8:30",
             //       Einde = "10:30",
             //       Duurtijd = "2:30",
             //       Capaciteit = 2,
             //       Gereserveerd = 0,
             //       AangemaaktOp = DateTime.Parse("2013/10/18"),
             //       Aangepast = DateTime.Parse("2013/10/18"),
             //       Soort = "Digitaal",
             //       Campus = "Meistraat",
             //       Departement = "IT"

             //   });
       
        }
    }
}
