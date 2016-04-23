namespace KAssets.Data.Migrations
{
    using KAssets.Models;
    using KAssets.Resources;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        /// <summary>
        /// Add default data to database
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(KAssets.Data.ApplicationDbContext context)
        {
            //Create the default security group
            var securityGroup = new SecurityGroup
            {
                Name = BaseAdmin.BaseGroup
            };

            //Create the default administrator user
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
            String hashedNewPassword = UserManager.PasswordHasher.HashPassword(BaseAdmin.Password);

            var user = new ApplicationUser
            {
                Email = BaseAdmin.Email,
                PasswordHash = hashedNewPassword,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                UserName = BaseAdmin.Email,
                Status = "Active"
            };


            bool tran = false;

            var rights = CreateRights();

            //Add rights to database
            foreach (var item in rights)
            {
                //Check if right exist in table
                if (!context.Rights.Any(x => x.Name == item.Name))
                {
                    context.Rights.Add(item);
                    tran = true;
                }
            }
           
            //Check base user exist
            if (!context.Users.Any(x => x.Email == BaseAdmin.Email))
            {
                UserManager.Create(user);
                tran = true;
            }

            //Check base security group exist
            if (!context.SecurityGroups.Any(x => x.Name == BaseAdmin.BaseGroup))
            {
                context.SecurityGroups.Add(securityGroup);
                tran = true;
            }

            ////Check are there currency in table and add base currency
            //if (context.Currency.Count() == 0)
            //{
            //    context.Currency.Add(new Currency
            //        {
            //            Code = "EUR"
            //        });
            //    context.Currency.Add(new Currency
            //    {
            //        Code = "BGN"
            //    });
            //}

            ////Check are there exchange rates for the base currency
            //if (context.ExchangeRates.Count() == 0)
            //{
            //    context.SaveChanges();

            //    context.ExchangeRates.Add(
            //        new ExchangeRate
            //        {
            //            ExchangeRateValue = 1.96,
            //            FromId = context.Currency.Where(x => x.Code == "BGN").First().Id,
            //            ToId = context.Currency.Where(x => x.Code == "EUR").First().Id,
            //        });
            //}

            //If are there a change in database save the context
            if (tran)
            {
                try
                {
                    context.SaveChanges();
                    securityGroup = context.SecurityGroups.Where(x => x.Name == BaseAdmin.BaseGroup).FirstOrDefault();
                    securityGroup.Rights.Add(context.Rights.First());
                    securityGroup.ApplicationUsers.Add(context.Users.Where(x=>x.Email==BaseAdmin.Email).FirstOrDefault());
                    context.SaveChanges();
                }
                catch { };
            }
        }

        /// <summary>
        /// Create the base rights in system and return the them
        /// </summary>
        /// <returns></returns>
        private List<Right> CreateRights()
        {
            var rights = new List<Right>();

            rights.Add(new Right
            {
                Name = "Admin right",
                Description = "Super admin",
                Code="1"
            });
            
            rights.Add(new Right
            {
                Name = "Manage items",
                Description = "The user can add, remove and edit items in application.",
                Code = "4"
            });

            rights.Add(new Right
            {
                Name = "Manage assets",
                Description = "The user can edit, add assets, send request for scrapping",
                Code = "5"
            });

            rights.Add(new Right
            {
                Name = "Manage locations",
                Description = "The user can add, remove and edit items in location.",
                Code = "6"
            });

            rights.Add(new Right
            {
                Name = "Create order for item",
                Description = "The user can create a order for item.",
                Code = "7"
            });

            rights.Add(new Right
            {
                Name = "Approve order for item",
                Description = "The user can approve orders for item.",
                Code = "8"
            });

            rights.Add(new Right
            {
                Name = "Give items for item order",
                Description = "The user can give items  for item order",
                Code = "9"
            });

            rights.Add(new Right
            {
                Name = "Approve request for scrapping",
                Description = "The user can approve request for scrapping an asset..",
                Code = "10"
            });

            rights.Add(new Right
            {
                Name = "Send request for relocation",
                Description = "The user can send request for relocation.",
                Code = "11"
            });

            rights.Add(new Right
            {
                Name = "Approve request for relocation",
                Description = "The user can approve request for relocation.",
                Code = "12"
            });

            rights.Add(new Right
            {
                Name = "Send request for renovation",
                Description = "The user can send request for renovation.",
                Code = "13"
            });

            rights.Add(new Right
            {
                Name = "Approve request for renovation",
                Description = "The user can approve request for renovation.",
                Code = "14"
            });

            rights.Add(new Right
            {
                Name = "Renovate assets",
                Description = "The user can renovate assets.",
                Code = "15"
            });

            rights.Add(new Right
            {
                Name = "Manage providers",
                Description = "The user can add, edit and delete providers.",
                Code = "16"
            });

            rights.Add(new Right
            {
                Name = "Create order for asset",
                Description = "The user can create order for asset.",
                Code = "17"
            });

            rights.Add(new Right
            {
                Name = "Approve orders for asset",
                Description = "The user can approve orders for asset.",
                Code = "18"
            });

            rights.Add(new Right
            {
                Name = "Give assets for asset orders",
                Description = "The user can give items for asset order.",
                Code = "19"
            });

            rights.Add( new Right
            {
                Name = "Send request to provider",
                Description = "The user can send request to provider for items.",
                Code = "20"
            });

            rights.Add(new Right
            {
                Name = "Approve request to provider",
                Description = "The user can Approve request to provider for items.",
                Code = "21"
            });

            rights.Add(new Right
            {
                Name = "Manage currency",
                Description = "The user can add ad edit currency.",
                Code = "22"
            });

            rights.Add( new Right
            {
                Name = "Manage exchange rates",
                Description = "The user can add and change exchange rates.",
                Code = "23"
            });

            rights.Add( new Right
            {
                Name = "Create invoice",
                Description = "The user can create invoices.",
                Code = "24"
            });

            rights.Add( new Right
            {
                Name = "Approve invoice",
                Description = "The user can approve invoices.",
                Code = "25"
            });

            rights.Add(new Right
            {
                Name = "Pay invoice",
                Description = "The user can pay invoices.",
                Code = "26"
            });

            rights.Add( new Right
            {
                Name = "Responding to incidents",
                Description = "The user can answer queries from other users on the system.",
                Code = "27"
            });


            rights.Add(new Right
            {
                Name = "Report for assets by status",
                Description = "The user can create report for assets by status.",
                Code = "28"
            });


            rights.Add(new Right
            {
                Name = "Report for accidents by date",
                Description = "The user can create report for accidents by status.",
                Code = "29"
            });

            rights.Add(new Right
            {
                Name = "Report for invoices by date",
                Description = "The user can create report for invoice by creation, approving and payment date and other fields.",
                Code = "30"
            });

            rights.Add(new Right
            {
                Name = "Report for asset relocations",
                Description = "The user can create report about all relocations for certain asset.",
                Code = "31"
            });

            rights.Add(new Right
            {
                Name = "Report for renovated assets",
                Description = "The user can create report about all renovated assets between two dates.",
                Code = "32"
            });

            rights.Add(new Right
            {
                Name = "Low admin",
                Description = "The user manages organisation, sites, users and security groups.",
                Code = "33"
            });
            return rights;
        }
    }
}
