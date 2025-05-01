using Microsoft.EntityFrameworkCore;
using SM_API.Models;

namespace SM_API.Data
{
    public class SupermarketDbContext : DbContext
    {
        public SupermarketDbContext(DbContextOptions<SupermarketDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillType> BillTypes { get; set; }
        public DbSet<BillItem> BillItems { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }


        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Models.Object> Objects { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ObjectPermission> ObjectPermissions { get; set; }
        public DbSet<RoleObjectPermission> RoleObjectPermissions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships

            //user, roles, UserRole
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            //Object, PermissionType, ObjectPermission
            //modelBuilder.Entity<ObjectPermission>()
            //    .HasKey(ur => new { ur.ObjectId, ur.PermissionId });

            modelBuilder.Entity<ObjectPermission>()
                .HasKey(op => op.Id);

            modelBuilder.Entity<ObjectPermission>()
                .HasOne(op => op.Object)
                .WithMany(o => o.ObjectPermissions)
                .HasForeignKey(op => op.ObjectId);

            modelBuilder.Entity<ObjectPermission>()
                .HasOne(op => op.Permission)
                .WithMany(pt => pt.ObjectPermissions)
                .HasForeignKey(op => op.PermissionId);


            //role, ObjectPermission ,RoleObjectPermission
            modelBuilder.Entity<RoleObjectPermission>()
                .HasKey(ur => new { ur.RoleId, ur.ObjectPermissionId });

            modelBuilder.Entity<RoleObjectPermission>()
               .HasOne(rop => rop.Role)
               .WithMany(r => r.RoleObjectPermissions)
               .HasForeignKey(rop => rop.RoleId);

            modelBuilder.Entity<RoleObjectPermission>()
               .HasOne(rop => rop.ObjectPermission)
               .WithMany(op => op.RoleObjectPermissions)
               .HasForeignKey(rop => rop.ObjectPermissionId);
            

            ////////////////////////////////////////////////////////////
            //Indexing

            //USER
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            //ROLE
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            //USER ROLE
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            // OBJECT
            modelBuilder.Entity<Models.Object>()
                .HasIndex(o => o.Name)
                .IsUnique();

            // PERMISSION TYPE
            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Name)
                .IsUnique();

            // OBJECT PERMISSION
            modelBuilder.Entity<ObjectPermission>()
                .HasIndex(op => new { op.ObjectId, op.PermissionId })
                .IsUnique();


            // ROLE OBJECT PERMISSION
            modelBuilder.Entity<RoleObjectPermission>()
                .HasIndex(rop => new { rop.RoleId, rop.ObjectPermissionId })
                .IsUnique();
            
            ////////////////////////////////////////////////////////////
            //CreatedBy, ModifiedBy

            //User
            modelBuilder.Entity<User>()
                .HasOne(o => o.CreatedBy)
                .WithMany()
                .HasForeignKey(o => o.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(o => o.ModifiedBy)
                .WithMany()
                .HasForeignKey(o => o.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            //Role
            modelBuilder.Entity<Role>()
                .HasOne(o => o.CreatedBy)
                .WithMany()
                .HasForeignKey(o => o.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>()
                .HasOne(o => o.ModifiedBy)
                .WithMany()
                .HasForeignKey(o => o.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            //UserRole
            modelBuilder.Entity<UserRole>()
                .HasOne(o => o.CreatedBy)
                .WithMany()
                .HasForeignKey(o => o.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(o => o.ModifiedBy)
                .WithMany()
                .HasForeignKey(o => o.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            //Object
            modelBuilder.Entity<Models.Object>()
                .HasOne(o => o.CreatedBy)
                .WithMany()
                .HasForeignKey(o => o.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.Object>()
                .HasOne(o => o.ModifiedBy)
                .WithMany()
                .HasForeignKey(o => o.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            //Permission
            modelBuilder.Entity<Permission>()
                .HasOne(o => o.CreatedBy)
                .WithMany()
                .HasForeignKey(o => o.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Permission>()
                .HasOne(o => o.ModifiedBy)
                .WithMany()
                .HasForeignKey(o => o.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);


            //ObjectPermission
            modelBuilder.Entity<ObjectPermission>()
                .HasOne(o => o.CreatedBy)
                .WithMany()
                .HasForeignKey(o => o.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ObjectPermission>()
                .HasOne(o => o.ModifiedBy)
                .WithMany()
                .HasForeignKey(o => o.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            //RoleObjectPermission
            modelBuilder.Entity<RoleObjectPermission>()
                .HasOne(o => o.CreatedBy)
                .WithMany()
                .HasForeignKey(o => o.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoleObjectPermission>()
                .HasOne(o => o.ModifiedBy)
                .WithMany()
                .HasForeignKey(o => o.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);
            ////////////////////////////////////////////////////////////
        }
    }
}

////Bill, BillItem, Product
//modelBuilder.Entity<Bill>()
//    .HasOne(b => b.Client)
//    .WithMany()
//    .HasForeignKey(b => b.ClientId);

//modelBuilder.Entity<Bill>()
//    .HasOne(b => b.BillType)
//    .WithMany()
//    .HasForeignKey(b => b.BillTypeId);

//modelBuilder.Entity<BillItem>()
//    .HasOne(bi => bi.Bill)
//    .WithMany(b => b.Items)
//    .HasForeignKey(bi => bi.BillId);

//modelBuilder.Entity<BillItem>()
//    .HasOne(bi => bi.Product)
//    .WithMany()
//    .HasForeignKey(bi => bi.ProductId);

//modelBuilder.Entity<Product>()
//    .HasOne(p => p.Category)
//    .WithMany()
//    .HasForeignKey(p => p.CategoryId);


//Guid SystemUserId = Guid.NewGuid();
//string? AdminhashedPassword = BCrypt.Net.BCrypt.HashPassword("1234");

//// Seed initial product data
//modelBuilder.Entity<User>().HasData(
//    new User { Id = SystemUserId, Username = "SystemUser", PasswordHash = "", Description = "System User", CreatedById = SystemUserId, CreationDate = DateTime.Now },
//    new User { Id = Guid.NewGuid(), Username = "Admin", PasswordHash = AdminhashedPassword, Description = "Admin User", CreatedById = SystemUserId, CreationDate = DateTime.Now }
//);