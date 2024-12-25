using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PhongKhamOnline.Models;
using PhongKhamOnline.Repositories;
using System.Data;

namespace PhongKhamOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class BacSiManagerController : Controller
    {
        private readonly IBacSiRepository _BacSiRepository;
        private readonly IChuyenMonBacSiRepository _ChuyenMonBacSiRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _roleManager;
        public BacSiManagerController(IBacSiRepository BacSiRepository, IUserRepository userRepository, UserManager<ApplicationUser> userManager, IChuyenMonBacSiRepository ChuyenMonBacSiRepository)
        {
            _roleManager = userRepository;
            _BacSiRepository = BacSiRepository;
            _ChuyenMonBacSiRepository = ChuyenMonBacSiRepository;
            _userManager = userManager;

        }
        public async Task<IActionResult> Index()
        {
            var BacSis = await _BacSiRepository.GetAllAsync();
            return View(BacSis);
        }

        public async Task<IActionResult> Create()
        {
            var chuyenMons = await _ChuyenMonBacSiRepository.GetAllAsync();
            ViewBag.ChuyenMonBacSi = new SelectList(chuyenMons, "Id", "TenChuyenMon");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(BacSi product, IFormFile AnhDaiDien)
        {
            var chuyenMons = await _ChuyenMonBacSiRepository.GetAllAsync();
            ViewBag.ChuyenMonBacSi = new SelectList(chuyenMons, "Id", "TenChuyenMon");
            if (ModelState.IsValid !=null)
            {
                if (AnhDaiDien == null || AnhDaiDien.Length == 0)
                {
                    ModelState.AddModelError("AnhDaiDien", "Vui lòng thêm ảnh đại diện cho bác sĩ.");
                    return View(product);
                }

                if (AnhDaiDien != null)
                {
                    product.AnhDaiDien = await SaveImage(AnhDaiDien);
                }

                if (string.IsNullOrEmpty(product.Email))
                {
                    ModelState.AddModelError("Email", "Email không được để trống.");
                    return View(product);
                }
                // Kiểm tra email đã tồn tại trong danh sách bác sĩ khác
                var existingDoctor = await _BacSiRepository.GetByEmail(product.Email);
                if (existingDoctor != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng bởi bác sĩ khác.");
                    return View(product);
                }

                var dataUser = await _userManager.FindByEmailAsync(product.Email);
                if (dataUser == null)
                {
                    ModelState.AddModelError("Email", "Email không khớp với email đăng ký của bác sĩ.");
                    return View(product);
                }

                var datarole = await _roleManager.GetByNameAsync("doctor");

                // Khởi tạo biến userRoleAdd
                var userRoleAdd = new IdentityUserRole<string>
                {
                    UserId = dataUser.Id,
                    RoleId = datarole.Id
                };
               
                await _roleManager.AddAsync(userRoleAdd);
                product.UserId = dataUser.Id;
                await _BacSiRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
           

            return View(product);
        }

        private async Task<string?> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
        }

        public async Task<IActionResult> Update(int id)
        {
            var product = await _BacSiRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var chuyenMons = await _ChuyenMonBacSiRepository.GetAllAsync();
            ViewBag.ChuyenMonBacSi = new SelectList(chuyenMons, "Id", "TenChuyenMon", product.ChuyenMonBacSiId);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, BacSi product, IFormFile AnhDaiDien)
        {
            if (id != product.Id)
            {
                ModelState.AddModelError("", "ID không khớp với thông tin bác sĩ.");
                return View(product);

            }
            var existingBacSi = await _BacSiRepository.GetByIdAsync(id);
            if (existingBacSi == null)
            {
                ModelState.AddModelError("", "Không tìm thấy bác sĩ với ID đã cung cấp.");
                return View(product);
            }          
            // Gán ảnh đại diện nếu không thay đổi
            if (AnhDaiDien != null)
            {
                product.AnhDaiDien = await SaveImage(AnhDaiDien);
            }
            else
            {
                product.AnhDaiDien = existingBacSi.AnhDaiDien;
            }

            var dataUser = await _userManager.FindByEmailAsync(product.Email);
            if (dataUser == null || dataUser.Id != product.UserId)
            {
                ModelState.AddModelError("Email", "Email không khớp với email của bác sĩ.");
                // Lấy lại danh sách chuyên môn
                var chuyenMons = await _ChuyenMonBacSiRepository.GetAllAsync();
                ViewBag.ChuyenMonBacSi = new SelectList(chuyenMons, "Id", "TenChuyenMon", product.ChuyenMonBacSiId);
                return View(product);
            }
            // Gán giá trị UserId nếu không thay đổi
            product.UserId = product.UserId ?? existingBacSi.UserId;  
            // Cập nhật dữ liệu
            await _BacSiRepository.UpdateAsync(product);

            // Chuyển hướng về Index sau khi lưu thành công
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _BacSiRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _BacSiRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Lấy thông tin user dựa trên UserId của bác sĩ
            var userRole = await _roleManager.GetByIdAsync(product.UserId);

            // Kiểm tra nếu user có vai trò "doctor"
            if (userRole != null && userRole.RoleId == (await _roleManager.GetByNameAsync("doctor")).Id)
            {
                // Xóa vai trò "doctor"
                await _roleManager.DeleteUserRoleAsync(product.UserId, userRole.RoleId);

                // Gán vai trò "user" cho tài khoản
                var userRoleUser = new IdentityUserRole<string>
                {
                    UserId = product.UserId,
                    RoleId = (await _roleManager.GetByNameAsync("user")).Id
                };
                await _roleManager.AddAsync(userRoleUser);
            }

            // Xóa thông tin bác sĩ
            await _BacSiRepository.DeleteAsync(id);

            TempData["SuccessMessage"] = "Xóa bác sĩ và cập nhật quyền thành công!";
            return RedirectToAction(nameof(Index));
        }
        
    }
}
