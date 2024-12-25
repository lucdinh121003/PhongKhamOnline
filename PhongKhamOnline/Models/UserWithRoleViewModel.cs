using Microsoft.AspNetCore.Identity;

namespace PhongKhamOnline.Models
{
    public class UserWithRoleViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public IdentityRole Role { get; set; }
    }
}
