using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhongKhamOnline.Models;

namespace PhongKhamOnline.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<BacSi> BacSis { get; set; }
        public DbSet<ChuyenMonBacSi> ChuyenMonBacSi { get; set; }
      
        public DbSet<BacSiImage> BacSiImages { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<LichLamViec> LichLamViecs { get; set; }

        public DbSet<KhungThoiGian> KhungThoiGian { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BacSi>()
                .HasOne(b => b.User) // Quan hệ 1 bác sĩ thuộc 1 user
                .WithMany()          // Không cần danh sách bác sĩ trong ApplicationUser
                .HasForeignKey(b => b.UserId) // Khóa ngoại là UserId
                .OnDelete(DeleteBehavior.Cascade); // Tùy chọn hành vi xóa
        }
    }
}
