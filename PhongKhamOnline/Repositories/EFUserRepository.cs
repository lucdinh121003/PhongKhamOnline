using PhongKhamOnline.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PhongKhamOnline.Models;


namespace PhongKhamOnline.Repositories
{
    public class EFUserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public EFUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<List<IdentityUserRole<string>>> GetAllAsync()
        {
            var data = await _context.UserRoles.ToListAsync();
            return data;
        }

        public async Task<List<IdentityRole>> GetAllRoleAsync()
        {
            var data = await _context.Roles.ToListAsync();
            return data;
        }

        public async Task<List<UserWithRoleViewModel>> GetUsersWithRolesAsync()
        {
            var users = await _context.Users.ToListAsync();
            var userRoles = new List<UserWithRoleViewModel>();

            foreach (var user in users)
            {
                var role = await (from userRole in _context.UserRoles
                                  join r in _context.Roles on userRole.RoleId equals r.Id
                                  where userRole.UserId == user.Id
                                  select r).FirstOrDefaultAsync(); // Lấy vai trò đầu tiên

                userRoles.Add(new UserWithRoleViewModel
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = role // Gán vai trò đầu tiên
                });
            }

            return userRoles;
        }
        
        public async Task<IdentityUserRole<string>> GetByIdAsync(string id)
        {
            return await _context.UserRoles.FirstOrDefaultAsync(p => p.UserId == id);
        }
        public async Task<IdentityRole> GetByNameAsync(string name)
        {
            var data = await _context.Roles.FirstOrDefaultAsync(p => p.Name == name);
            return data;
        }

        public async Task AddAsync(IdentityUserRole<string> userRole)
        {
            _context.UserRoles.Add(userRole);
             await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(IdentityUserRole<string> userRoleData)
        {
          
            _context.UserRoles.Update(userRoleData);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
            }
        }


        public async Task DeleteUserRoleAsync(string userId, string roleId)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
            }
        }

    }
}
