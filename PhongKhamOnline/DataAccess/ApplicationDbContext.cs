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
        public DbSet<KhungGioBacSi> KhungGioBacSis { get; set; }
        public DbSet<BacSiImage> BacSiImages { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}
