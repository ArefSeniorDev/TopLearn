using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.DataLayer.Context
{
    public class TopLearnContext : DbContext
    {

        public TopLearnContext(DbContextOptions<TopLearnContext> options) : base(options)
        {

        }

        #region User

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WalletType>()
           .HasData
           (
             new WalletType() { TypeId = 1, TypeTitle = "برداشت", },
             new WalletType() { TypeId = 1, TypeTitle = "واریز", }
           );
            modelBuilder.Entity<User>()
           .HasData
           (
            new User() { Email = "Aref@gmail.com", IsActive = true, RegisterDate = DateTime.Now, UserName = "Aref", Password = "Admin", }
           );
            base.OnModelCreating(modelBuilder);
        }

        #endregion

        #region Wallet
        public DbSet<Wallet> Wallet { get; set; }
        public DbSet<WalletType> WalletType { get; set; }

        #endregion
    }
}
