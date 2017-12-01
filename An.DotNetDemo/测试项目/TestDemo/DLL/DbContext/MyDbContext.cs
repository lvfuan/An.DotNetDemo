using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DLL.Models;

namespace DLL
{
     public class MyDbContext:DbContext
    {
        public MyDbContext() : base("Default") { Database.SetInitializer(new DropCreateDatabaseIfModelChanges<MyDbContext>()); }
        public DbSet<UserModel> User { get; set; }
        public DbSet<ManagementModel> Management { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);        
        }
    }
}
