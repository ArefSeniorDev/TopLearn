using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TopLearn.DataLayer.Entities.Course;
using TopLearn.DataLayer.Entities.Permissions;
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

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Role>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<CourseGroup>().HasQueryFilter(x => !x.IsDelete);

            //MY Way OF Course Relation
            modelBuilder.Entity<Course>()

             .HasOne<CourseGroup>(f => f.CourseGroup)

             .WithMany(g => g.Courses)

             .HasForeignKey(f => f.GroupId);



            modelBuilder.Entity<Course>()

               .HasOne<CourseGroup>(f => f.Group)

                .WithMany(g => g.SubGroup)

               .HasForeignKey(f => f.SubGroup);

            // modelBuilder.Entity<WalletType>()
            //.HasData
            //(
            //  new WalletType() { TypeId = 1, TypeTitle = "برداشت", },
            //  new WalletType() { TypeId = 1, TypeTitle = "واریز", }
            //);
            // modelBuilder.Entity<User>()
            //.HasData
            //(
            // new User() { Email = "Aref@gmail.com", IsActive = true, RegisterDate = DateTime.Now, UserName = "Aref", Password = "Admin", }
            //);
            base.OnModelCreating(modelBuilder);
        }


        #region Permission

        public DbSet<Permission> Permission { get; set; }

        public DbSet<RolePermission> RolePermission { get; set; }

        #endregion


        #region Course

        public DbSet<CourseGroup> CourseGroups { get; set; }
        public DbSet<CourseLevel> CourseLevels { get; set; }
        public DbSet<CourseStatus> CourseStatuses { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseEpisode> CourseEpisodes { get; set; }

        #endregion



        #region Wallet
        public DbSet<Wallet> Wallet { get; set; }

        #endregion
    }
}
