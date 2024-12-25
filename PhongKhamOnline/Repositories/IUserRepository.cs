using Microsoft.AspNetCore.Identity;
using PhongKhamOnline.Models;


namespace PhongKhamOnline.Repositories
{
    public interface IUserRepository
    {
        Task<List<IdentityUserRole<string>>> GetAllAsync();
        Task<List<IdentityRole>> GetAllRoleAsync();
        Task<IdentityUserRole<string>> GetByIdAsync(string id);
        Task<IdentityRole> GetByNameAsync(string name);
        Task<List<UserWithRoleViewModel>> GetUsersWithRolesAsync();
        Task AddAsync(IdentityUserRole<string> userRole);
        Task UpdateAsync(IdentityUserRole<string> userRoleData);
        Task DeleteAsync(string id);
        Task DeleteUserRoleAsync(string userId, string roleId);
    }
}
