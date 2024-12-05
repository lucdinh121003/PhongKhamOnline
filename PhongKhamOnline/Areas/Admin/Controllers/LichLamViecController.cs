using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhongKhamOnline.Models;
using PhongKhamOnline.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhongKhamOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class LichLamViecController : Controller
    {
        private readonly ILichLamViecRepository _lichLamViecRepository;
        private readonly IBacSiRepository _bacSiRepository;

        public LichLamViecController(ILichLamViecRepository lichLamViecRepository, IBacSiRepository bacSiRepository)
        {
            _lichLamViecRepository = lichLamViecRepository;
            _bacSiRepository = bacSiRepository;
        }

        public async Task<IActionResult> Index()
        {
            var lichLamViecs = await _lichLamViecRepository.GetAllAsync();

            // Nhóm dữ liệu và tính số lượng tối đa (không tính theo mốc thời gian)
            var groupedData = lichLamViecs
                .GroupBy(l => new { l.BacSi.Ten, l.NgayLamViec })
                .Select(g => new
                {
                    Id = g.First().Id,  // Thêm Id từ mục đầu tiên trong nhóm
                    BacSi = g.Key.Ten,
                    NgayLamViec = g.Key.NgayLamViec,
                    ThoiGian = string.Join(", ", g.Select(l => l.KhungThoiGian.Time)),
                    
                    SoLuongToiDa = g.Max(l => l.SoLuongToiDa)  // Lấy số lượng tối đa trong nhóm
                })
                .ToList();

            return View(groupedData);
        }



        public async Task<IActionResult> Create()
        {
            // Lấy danh sách bác sĩ để gắn vào dropdown
            var bacSiList = await _bacSiRepository.GetAllAsync();
            ViewBag.BacSiList = bacSiList;

            // Khung giờ mặc định
            ViewBag.KhungGio = new List<string>
            {
                "7:00-7:30", "7:30-8:00", "8:00-8:30", "8:30-9:00",
                "9:00-9:30", "9:30-10:00", "10:00-10:30", "10:30-11:00",
                "13:30-14:00", "14:00-14:30", "14:30-15:00", "15:00-15:30",
                "15:30-16:00", "16:00-16:30", "16:30-17:00"
            };

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int bacSiId, DateTime ngayLamViec, List<string> khungGios, int soLuongToiDa)
        {
            // Kiểm tra khung giờ rỗng
            if (khungGios == null || khungGios.Count == 0)
            {
                ModelState.AddModelError("", "Bạn cần chọn ít nhất một khung giờ.");
                return View();
            }

            // Kiểm tra ngày làm việc hợp lệ
            if (ngayLamViec.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("", "Ngày làm việc không được nằm trong quá khứ.");
                return View();
            }

            // Lấy lịch làm việc hiện tại của bác sĩ trong ngày được chọn
            var existingSchedules = await _lichLamViecRepository.GetByBacSiAndDateAsync(bacSiId, ngayLamViec);

            foreach (var khungGio in khungGios)
            {
                // Kiểm tra trùng lặp khung giờ
                //if (existingSchedules.Any(s => s.KhungThoiGian.Time == khungGio))
                //{
                //    ModelState.AddModelError("", $"Bác sĩ đã có lịch làm việc trong khung giờ {khungGio}.");
                //    continue;
                //}

                // Tạo lịch làm việc mới nếu không trùng lặp
                var lichLamViec = new LichLamViec
                {
                    BacSiId = bacSiId,
                    NgayLamViec = ngayLamViec,
                    KhungThoiGianId = Convert.ToInt32(khungGio),
                    SoLuongToiDa = soLuongToiDa,

                };

                await _lichLamViecRepository.AddAsync(lichLamViec);
            }

            if (!ModelState.IsValid)
            {
                // Nếu có lỗi, giữ lại danh sách bác sĩ và khung giờ để hiển thị lại form
                var bacSiList = await _bacSiRepository.GetAllAsync();
                ViewBag.BacSiList = bacSiList;
                ViewBag.KhungGio = new List<string>
                {
                    "7:00-7:30", "7:30-8:00", "8:00-8:30", "8:30-9:00",
                    "9:00-9:30", "9:30-10:00", "10:00-10:30", "10:30-11:00",
                    "13:30-14:00", "14:00-14:30", "14:30-15:00", "15:00-15:30",
                    "15:30-16:00", "16:00-16:30", "16:30-17:00"
                };
                return View();
            }

            TempData["SuccessMessage"] = "Lịch làm việc đã được thêm thành công.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var lichLamViec = await _lichLamViecRepository.GetByIdAsync(id);
            if (lichLamViec == null)
            {
                return NotFound();
            }

            ViewBag.BacSiList = await _bacSiRepository.GetAllAsync();
            return View(lichLamViec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DateTime ngayLamViec, string thoiGian, int soLuongToiDa)
        {
            var lichLamViec = await _lichLamViecRepository.GetByIdAsync(id);
            if (lichLamViec == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                lichLamViec.NgayLamViec = ngayLamViec;
                lichLamViec.KhungThoiGianId = Convert.ToInt32(thoiGian);
                lichLamViec.SoLuongToiDa = soLuongToiDa;

                await _lichLamViecRepository.UpdateAsync(lichLamViec);
                return RedirectToAction(nameof(Index));
            }

            return View(lichLamViec);
        }

        // GET: LichLamViec/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var lichLamViec = await _lichLamViecRepository.GetByIdAsync(id);
            if (lichLamViec == null)
            {
                return NotFound();
            }

            return View(lichLamViec);
        }

        // POST: LichLamViec/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _lichLamViecRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index)); // Chuyển hướng về trang danh sách sau khi xóa
        }
    }
}