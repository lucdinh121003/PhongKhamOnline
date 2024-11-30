using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhongKhamOnline.Models;
using PhongKhamOnline.Repositories;
using System.Data;

namespace PhongKhamOnline.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "doctor")]
    public class LichLamViecDoctorController : Controller
    {
        private readonly ILichLamViecRepository _lichLamViecRepository;
        private readonly IBacSiRepository _bacSiRepository;
        private readonly IKhungThoiGianRepository _khungThoiGianRepository;

        public LichLamViecDoctorController(ILichLamViecRepository lichLamViecRepository, IBacSiRepository bacSiRepository, IKhungThoiGianRepository khungThoiGianRepository)
        {
            _lichLamViecRepository = lichLamViecRepository;
            _bacSiRepository = bacSiRepository;
            _khungThoiGianRepository = khungThoiGianRepository;
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
                    ThoiGian = string.Join(", ", g.Select(l => l.ThoiGian)),

                    SoLuongToiDa = g.Max(l => l.SoLuongToiDa)  // Lấy số lượng tối đa trong nhóm
                })
                .ToList();

            return View(groupedData);
        }



        public async Task<IActionResult> Create(int bacSiId, DateTime ngayLamViec)
        {
            // Lấy danh sách bác sĩ để gắn vào dropdown
            var bacSiList = await _bacSiRepository.GetAllAsync();
            ViewBag.BacSiList = bacSiList;

            // Khung giờ mặc định
            var listTime = await _khungThoiGianRepository.GetAllAsync();
            ViewBag.KhungGio = listTime;

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
                if (existingSchedules.Any(s => s.ThoiGian == khungGio))
                {
                    ModelState.AddModelError("", $"Bác sĩ đã có lịch làm việc trong khung giờ {khungGio}.");
                    continue;
                }

                // Tạo lịch làm việc mới nếu không trùng lặp
                var lichLamViec = new LichLamViec
                {
                    BacSiId = bacSiId,
                    NgayLamViec = ngayLamViec,
                    ThoiGian = khungGio,
                    SoLuongToiDa = soLuongToiDa,

                };

                await _lichLamViecRepository.AddAsync(lichLamViec);
            }

            if (!ModelState.IsValid)
            {
                // Nếu có lỗi, giữ lại danh sách bác sĩ và khung giờ để hiển thị lại form
                var bacSiList = await _bacSiRepository.GetAllAsync();
                ViewBag.BacSiList = bacSiList;
                var listTime = await _khungThoiGianRepository.GetAllAsync();
                ViewBag.KhungGio = listTime;
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
                lichLamViec.ThoiGian = thoiGian;
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
