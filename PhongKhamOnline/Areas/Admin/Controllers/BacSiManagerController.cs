using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PhongKhamOnline.Models;
using PhongKhamOnline.Repositories;

namespace PhongKhamOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class BacSiManagerController : Controller
    {
        private readonly IBacSiRepository _BacSiRepository;
       
        private readonly IChuyenMonBacSiRepository _ChuyenMonBacSiRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BacSiManagerController(IBacSiRepository BacSiRepository,  IChuyenMonBacSiRepository ChuyenMonBacSiRepository, UserManager<ApplicationUser> userManager)
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
            if (ModelState.IsValid !=null)
            {
                if (AnhDaiDien != null)
                {
                    product.AnhDaiDien = await SaveImage(AnhDaiDien);
                }
                var data = await _userManager.FindByEmailAsync(product.Email);
                product.UserId = data.Id;
                await _BacSiRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
           
            var chuyenMons = await _ChuyenMonBacSiRepository.GetAllAsync();
          
            ViewBag.ChuyenMonBacSi = new SelectList(chuyenMons, "Id", "TenChuyenMon");

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
                return NotFound();
            }
            if (ModelState.IsValid !=null ) 
            {
                if (AnhDaiDien != null)
                {
                    // Lưu hình ảnh đại diện
                    product.AnhDaiDien = await SaveImage(AnhDaiDien);
                }
                await _BacSiRepository.UpdateAsync(product);
                return RedirectToAction(nameof(Index));
            }
           
            var chuyenMons = await _ChuyenMonBacSiRepository.GetAllAsync();
            
            ViewBag.ChuyenMonBacSi = new SelectList(chuyenMons, "Id", "TenChuyenMon", product.ChuyenMonBacSiId);
            return View(product);
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
