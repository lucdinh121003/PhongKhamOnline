using Microsoft.AspNetCore.Identity;

namespace PhongKhamOnline.Models
{
    public class ApplicationUser :IdentityUser
    {
        public String FullName { get; set; }
        public int IdBacSi { get; set; }

        public List<BacSi>? BacSis { get; set; }
    }
}
