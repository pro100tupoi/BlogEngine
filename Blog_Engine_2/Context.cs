using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.Collections.Generic;
using Blog_Engine_2.Models;
using Blog_Engine_2.Objects;

namespace Blog_Engine_2
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public DbSet<Post> Posts { get; set; } = null!;

        public DbSet<Picture> Pictures { get; set; } = null!;

        public Context(DbContextOptions<Context> options) : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }
    }
}
//public class Context(DbContextOptions<Context> options) : DbContext(options)
//{
//    public DbSet<User> Users { get; set; } = null!;

//    public DbSet<Post> Posts { get; set; } = null!;

//    public DbSet<Picture> Pictures { get; set; } = null!;
//}