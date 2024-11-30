using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PhongKhamOnline.Models;
using System.Security.Claims;
using System;
using PhongKhamOnline.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PhongKhamOnline.Repositories;

namespace PhongKhamOnline.Areas.User.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class LichLamViecDoctorController : Controller
    {
        private readonly ILichLamViecRepository _lichLamViecRepository;
        private readonly IBacSiRepository _bacSiRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public LichLamViecDoctorController(ILichLamViecRepository lichLamViecRepository, IBacSiRepository bacSiRepository, UserManager<ApplicationUser> userManager)
        {
            _lichLamViecRepository = lichLamViecRepository;
            _bacSiRepository = bacSiRepository;
            _userManager = userManager;
        }

        // Lấy thông tin bác sĩ từ tài khoản người dùng
        private async Task<BacSi> GetDoctorInfoAsync()
        {
            // Lấy người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);

            // Lấy thông tin bác sĩ theo IdBacSi
            var doctor = await _bacSiRepository.GetByIdAsync(user.IdBacSi);

            return doctor;
        }

        public async Task<IActionResult> Create()
        {
            var doctor = await GetDoctorInfoAsync();

            // Nếu không có bác sĩ, trả về lỗi
            if (doctor == null)
            {
                return NotFound("Không tìm thấy bác sĩ với tài khoản này.");
            }

            // Sử dụng thông tin bác sĩ ở đây
            ViewBag.DoctorName = doctor.Ten;

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
        public async Task<IActionResult> Create(DateTime ngayLamViec, List<string> khungGios, int soLuongToiDa)
        {
            var doctor = await GetDoctorInfoAsync();

            // Nếu không có bác sĩ, trả về lỗi
            if (doctor == null)
            {
                return NotFound("Không tìm thấy bác sĩ với tài khoản này.");
            }

            // Lấy lịch làm việc hiện tại của bác sĩ trong ngày được chọn
            var existingSchedules = await _lichLamViecRepository.GetByBacSiAndDateAsync(doctor.Id, ngayLamViec);

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
                    BacSiId = doctor.Id,
                    NgayLamViec = ngayLamViec,
                    ThoiGian = khungGio,
                    SoLuongToiDa = soLuongToiDa,
                };

                await _lichLamViecRepository.AddAsync(lichLamViec);
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            TempData["SuccessMessage"] = "Lịch làm việc đã được thêm thành công.";
            return RedirectToAction(nameof(Index));
        }
    }

}
