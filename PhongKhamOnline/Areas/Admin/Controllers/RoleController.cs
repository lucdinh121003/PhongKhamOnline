using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PhongKhamOnline.Repositories;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using PhongKhamOnline.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PhongKhamOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class RoleController : Controller
    {
        private readonly IUserRepository _roleManager;

        public RoleController( IUserRepository userRepository)
        {
            _roleManager = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {         
            var Users = await _roleManager.GetUsersWithRolesAsync();           
            return View(Users);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = (await _roleManager.GetUsersWithRolesAsync()).FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            // Lấy danh sách tất cả vai trò
            var roles = await _roleManager.GetAllRoleAsync(); 
            if (user?.Role != null)
            {
                ViewBag.Role = new SelectList(roles, "Id", "Name", user.Role.Id);
            }
            else
            {
                // Nếu user không có role chọn vai trò mặc định
                ViewBag.Role = new SelectList(roles, "Id", "Name");
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserWithRoleViewModel model)
        {
           
            var userRole = await _roleManager.GetByIdAsync(model.UserId);

            // Kiểm tra xem user đã có vai trò hay chưa
            if (userRole != null)
            {
                await _roleManager.DeleteUserRoleAsync(model.UserId, userRole.RoleId);
                IdentityUserRole<string> newUserTRole = new IdentityUserRole<string>
                {
                    RoleId = model.Role.Id,
                    UserId = userRole.UserId
                };
                // Cập nhật role hiện tại của user với role mới
                await _roleManager.AddAsync(newUserTRole);

            }
            else
            {
                // Nếu user chưa có role, gán role mới cho user
                var role = await _roleManager.GetByNameAsync(model.Role.Id);
                // Gán role mới cho user
                await _roleManager.AddAsync(new IdentityUserRole<string>
                {
                    UserId = model.UserId,
                    RoleId = model.Role.Id
                });
            }
            var user = (await _roleManager.GetUsersWithRolesAsync()).FirstOrDefault(u => u.UserId == model.UserId);
            var roles = await _roleManager.GetAllRoleAsync();
            ViewBag.Role = new SelectList(roles, "Id", "Name", user.Role.Id);
            TempData["SuccessMessage"] = "Cập nhật quyền thành công!";
            return RedirectToAction("Edit");
        }
    }
}
