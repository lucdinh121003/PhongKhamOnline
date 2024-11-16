using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using PhongKhamOnline.Models;
using PhongKhamOnline.Repositories;
using System.Threading.Tasks;

namespace PhongKhamOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ChuyenKhoaManagerController : Controller
    {
        private readonly IChuyenMonBacSiRepository _ChuyenMonBacSiRepository;
        public ChuyenKhoaManagerController(IChuyenMonBacSiRepository ChuyenMonBacSiRepository)
        {
            _ChuyenMonBacSiRepository = ChuyenMonBacSiRepository;
        }
        public async Task<IActionResult> Index()
        {
            var chuyenMonList = await _ChuyenMonBacSiRepository.GetAllAsync();
            return View(chuyenMonList);
        }
        
       
        public IActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChuyenMonBacSi chuyenMonBacSi)
        {
            if (ModelState.IsValid)
            {
                await _ChuyenMonBacSiRepository.AddAsync(chuyenMonBacSi);
                return RedirectToAction(nameof(Index));
            }
            return View(chuyenMonBacSi);
        }

     
        public async Task<IActionResult> Edit(int id)
        {
            var chuyenMon = await _ChuyenMonBacSiRepository.GetByIdAsync(id);
            if (chuyenMon == null)
            {
                return NotFound();
            }
            return View(chuyenMon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string newTenChuyenMon)
        {
            var chuyenMon = await _ChuyenMonBacSiRepository.GetByIdAsync(id);
            if (chuyenMon == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                chuyenMon.TenChuyenMon = newTenChuyenMon;  // Đổi tên chuyên môn
                await _ChuyenMonBacSiRepository.UpdateAsync(chuyenMon);
                return RedirectToAction(nameof(Index));
            }

            return View(chuyenMon);
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            var chuyenMon = await _ChuyenMonBacSiRepository.GetByIdAsync(id);
            if (chuyenMon == null)
            {
                return NotFound();
            }
            return View(chuyenMon);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _ChuyenMonBacSiRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
