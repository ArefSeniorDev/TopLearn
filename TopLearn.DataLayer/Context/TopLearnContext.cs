using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TopLearn.DataLayer.Entities.Course;
using TopLearn.DataLayer.Entities.Order;
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
        public DbSet<UserDiscountCode> UserDiscountCodes { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Role>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<CourseGroup>().HasQueryFilter(x => !x.IsDelete);
            modelBuilder.Entity<Course>().HasQueryFilter(x => !x.IsDeleted);

            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            //MY Way OF Course Relation
            modelBuilder.Entity<Course>()

             .HasOne<CourseGroup>(f => f.CourseGroup)

             .WithMany(g => g.Courses)

             .HasForeignKey(f => f.GroupId);



            modelBuilder.Entity<Course>()

               .HasOne<CourseGroup>(f => f.Group)

                .WithMany(g => g.SubGroup)

               .HasForeignKey(f => f.SubGroup);


            // modelBuilder.Entity<Permission>()
            //.HasData
            //(
            //  new Permission() { ParentID = null, PermissionId = 1, PermissionTitle = "پنل مدریت", },
            //  new Permission() { ParentID = 1, PermissionId = 2, PermissionTitle = "مدیریت کاربران", },
            //  new Permission() { ParentID = 2, PermissionId = 3, PermissionTitle = "افزودن کاربران", },
            //  new Permission() { ParentID = 2, PermissionId = 4, PermissionTitle = "ویرایش کاربر", },
            //  new Permission() { ParentID = 2, PermissionId = 5, PermissionTitle = "مدیریت نقش ها"},
            //  new Permission() { ParentID = 2, PermissionId = 6, PermissionTitle = "حذف کاربر", },
            //  new Permission() { ParentID = 2, PermissionId = 7, PermissionTitle = "حذف کاربر", },
            //  new Permission() { ParentID = 2, PermissionId = 8, PermissionTitle = "حذف کاربر", },
            //  new Permission() { ParentID = 2, PermissionId = 9, PermissionTitle = "حذف کاربر", },
            //  new Permission() { ParentID = 2, PermissionId = 10, PermissionTitle = "حذف کاربر", },
            //  new Permission() { ParentID = 2, PermissionId = 11, PermissionTitle = "حذف کاربر", },
            //  new Permission() { ParentID = 2, PermissionId = 12, PermissionTitle = "حذف کاربر", },
            //  new Permission() { ParentID = 2, PermissionId = 13, PermissionTitle = "حذف کاربر", }

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
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<CourseComment> CourseComments { get; set; }

        #endregion

        #region Order
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Discount> Discounts { get; set; }

        #endregion

        #region Wallet
        public DbSet<Wallet> Wallet { get; set; }

        #endregion
    }
}
