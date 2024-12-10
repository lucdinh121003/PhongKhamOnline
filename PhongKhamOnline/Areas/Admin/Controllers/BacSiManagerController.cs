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

        public BacSiManagerController(IBacSiRepository BacSiRepository, UserManager<ApplicationUser> userManager, IChuyenMonBacSiRepository ChuyenMonBacSiRepository)
        {
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
                if (AnhDaiDien != null)
                {
                    product.AnhDaiDien = await SaveImage(AnhDaiDien);
                }
                if (product.Email == null || product.Email == "")
                {
                    return View(product);
                }
                var dataUser = await _userManager.FindByEmailAsync(product.Email);
                if (dataUser == null)
                {
                    ModelState.AddModelError("Email", "Email không khớp với email đăng ký của bác sĩ.");
                    return View(product);
                }
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
            await _BacSiRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
