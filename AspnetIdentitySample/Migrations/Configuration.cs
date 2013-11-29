namespace Examonitor.Migrations
{
    using Examonitor.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
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
            UserManager.UserValidator = new UserValidator<MyUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };

            var userResult = new IdentityResult();

            string adminName = "Admin";
            string adminPassword = "123456";
            string adminEmail = "ad@m.in";
            string adminRole = "Admin";

            List<string> testUsers = new List<string>() { "user1@ap.be", "user2@ap.be", "user3@ap.be" };
            string testUserPassword = "password";
            string testUserRole = "Docent";
            
            //Create Roles
            if (!RoleManager.RoleExists(adminRole))
            {
                var roleresult = RoleManager.Create(new IdentityRole(adminRole));
            }

            if (!RoleManager.RoleExists(testUserRole))
            {
                var roleresult = RoleManager.Create(new IdentityRole(testUserRole));
            }

            //Create Admin user
            var user = new MyUser();
            user.UserName = adminName;
            user.IsConfirmed = true;
            user.Email = adminEmail;
            userResult = UserManager.Create(user, adminPassword);

            //Add Admin user to role
            if (userResult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, adminRole);
            }
            
            //Create test users
            foreach (string testUser in testUsers)
            {
                user = new MyUser();
                user.UserName = testUser;
            user.IsConfirmed = true;
                user.Email = testUser;
                userResult = UserManager.Create(user, testUserPassword);

                //Add to test user role
                if (userResult.Succeeded)
            {
                    var result = UserManager.AddToRole(user.Id, testUserRole);
                }
            }

            base.Seed(context);

            List<Campus> campussen = new List<Campus>() { new Campus
                {
                    Name = "Meistraat"
                },            
             new Campus
                {
                    Name = "Bouwmeesterstraat"
                }};

            foreach (Campus campus in campussen)
            {               
                if(context.Campus.Where(n => n.Name.Equals(campus.Name)).Count().Equals(0))
                {
                    context.Campus.AddOrUpdate(i => i.Id, campus);
                }
            }

            List<MonitorBeurtModel> monitorbeurten = new List<MonitorBeurtModel>(){
               new MonitorBeurtModel
               {
                   Datum = DateTime.Parse("2013/10/18"),
                   ExamenNaam = "Ontwikkelen in een bedrijfsomgeving",
                   Start = "8:30",
                   Einde = "10:30",
                   Duurtijd = "2:30",
                   Capaciteit = 2,
                   Gereserveerd = 0,
                   Campus = campussen[0],
                   Digitaal = true

               },
               new MonitorBeurtModel
               {
                   Datum = DateTime.Parse("2013/10/18"),
                   ExamenNaam = "Informatica-architectuur",
                   Start = "8:30",
                   Einde = "10:30",
                   Duurtijd = "2:30",
                   Capaciteit = 2,
                   Campus = campussen[1],
                   Gereserveerd = 0,
                   
               }};

            foreach (MonitorBeurtModel monitorbeurt in monitorbeurten)
            {
                if(context.MonitorBeurt.Where(n => n.ExamenNaam.Equals(monitorbeurt.ExamenNaam)).Count().Equals(0))
                {
                    context.MonitorBeurt.AddOrUpdate(i => i.MonitorBeurtId, monitorbeurt);
                }
            }
       
        }
    }
}
