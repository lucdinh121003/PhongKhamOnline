using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public LichLamViecDoctorController(ILichLamViecRepository lichLamViecRepository, IBacSiRepository bacSiRepository, IKhungThoiGianRepository khungThoiGianRepository, UserManager<ApplicationUser> userManager)
        {
            _lichLamViecRepository = lichLamViecRepository;
            _bacSiRepository = bacSiRepository;
            _khungThoiGianRepository = khungThoiGianRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentBacSi = await _bacSiRepository.GetByUserId(currentUser.Id);

            // Lấy danh sách lịch làm việc của bác sĩ đăng nhập
            var lichLamViecs = await _lichLamViecRepository.GetByBacSiIdAsync(currentBacSi.Id);

            // Nhóm dữ liệu và tính số lượng tối đa (không tính theo mốc thời gian)
            var groupedData = lichLamViecs
                .GroupBy(l => new { l.BacSi.Ten, l.NgayLamViec })
                .Select(g => new
                {
                    Id = g.First().Id, // Thêm Id từ mục đầu tiên trong nhóm
                    BacSi = g.Key.Ten,
                    NgayLamViec = g.Key.NgayLamViec,
                    ThoiGian = string.Join(", ", g.Select(l => l.KhungThoiGian.Time)),
                    SoLuongToiDa = g.Max(l => l.SoLuongToiDa) // Lấy số lượng tối đa trong nhóm
                })
                .ToList();


            return View(groupedData);
        }



        [HttpGet]
        public async Task<IActionResult> GetKhungGioDaDat(string idUser, DateTime ngayLamViec)
        {
            var findBacSi = await _bacSiRepository.GetByUserId(idUser);
            var existingSchedules = await _lichLamViecRepository.getListLichLamViecByBacSiIdAndDate(findBacSi.Id, ngayLamViec);
            var khungGioDaDat = existingSchedules.Select(l => l.KhungThoiGian).ToList();

            return Json(khungGioDaDat);
        }

        public async Task<IActionResult> Create(int bacSiId, DateTime ngayLamViec)
        {
            // Khung giờ mặc định
            var listTime = await _khungThoiGianRepository.GetAllAsync();
            ViewBag.KhungGio = listTime;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string idUser, DateTime ngayLamViec, List<string> khungGios, int soLuongToiDa)
        {
            var listTime = await _khungThoiGianRepository.GetAllAsync();
            ViewBag.KhungGio = listTime;
            var newDate = DateTime.Now;
            if (ngayLamViec <= newDate)
            {
                ModelState.AddModelError("KhungGios", "Vui lòng không chọn ngày đã qua.");
                return View();
            }
            // Kiểm tra khung giờ rỗng
            if (khungGios == null || khungGios.Count == 0)
            {
                ModelState.AddModelError("KhungGios", "Bạn cần chọn ít nhất một khung giờ.");

                return View();
            }

            // Lấy thông tin bác sĩ
            var findBacSi = await _bacSiRepository.GetByUserId(idUser);

            // Lấy tất cả lịch làm việc hiện tại của bác sĩ theo ngày
            var existingSchedules = await _lichLamViecRepository.getListLichLamViecByBacSiIdAndDate(findBacSi.Id, ngayLamViec);

            var existingScheduleDict = existingSchedules.ToDictionary(l => l.KhungThoiGianId);

            // Danh sách ID khung giờ trong database
            var existingKhungGioIds = existingScheduleDict.Keys;

            // Danh sách ID khung giờ được gửi lên (convert sang int)
            var newKhungGioIds = khungGios.Select(int.Parse).ToList();

            // Các khung giờ cần xóa (có trong database nhưng không được gửi lên)
            var khungGiosToDelete = existingKhungGioIds.Except(newKhungGioIds).ToList();

            // Các khung giờ cần thêm mới (có trong danh sách gửi lên nhưng không có trong database)
            var khungGiosToAdd = newKhungGioIds.Except(existingKhungGioIds).ToList();

            // Xóa các lịch làm việc không còn trong danh sách gửi lên
            foreach (var khungGioId in khungGiosToDelete)
            {
                var scheduleToDelete = existingScheduleDict[khungGioId];
                await _lichLamViecRepository.DeleteAsync(scheduleToDelete.Id);
            }

            // Thêm mới các lịch làm việc
            foreach (var khungGioId in khungGiosToAdd)
            {
                var newSchedule = new LichLamViec
                {
                    BacSiId = findBacSi.Id,
                    NgayLamViec = ngayLamViec,
                    KhungThoiGianId = khungGioId,
                    SoLuongToiDa = soLuongToiDa
                };
                await _lichLamViecRepository.AddAsync(newSchedule);
            }

            // Cập nhật các lịch làm việc hiện có
            foreach (var khungGioId in newKhungGioIds.Intersect(existingKhungGioIds))
            {
                var existingSchedule = existingScheduleDict[khungGioId];
                existingSchedule.SoLuongToiDa = soLuongToiDa;
                await _lichLamViecRepository.UpdateAsync(existingSchedule);
            }

            TempData["success"] = "Lịch làm việc đã được thêm thành công.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            // Lấy danh sách tất cả khung giờ
            var listTime = await _khungThoiGianRepository.GetAllAsync();

            // Lấy lịch làm việc theo ID
            var lichLamViec = await _lichLamViecRepository.GetByIdAsync(id);

            if (lichLamViec == null)
            {
                return NotFound();
            }

            // Lấy thông tin bác sĩ
            var findBacSi = await _bacSiRepository.GetByIdAsync(lichLamViec.BacSiId);

            // Lấy tất cả khung giờ đã được chọn trong ngày của bác sĩ
            var existingSchedules = await _lichLamViecRepository.getListLichLamViecByBacSiIdAndDate(findBacSi.Id, lichLamViec.NgayLamViec);
            var selectedKhungGios = existingSchedules.Select(l => l.KhungThoiGianId).ToList();

            // Truyền dữ liệu vào ViewBag
            ViewBag.KhungGio = listTime;
            ViewBag.SelectedKhungGios = selectedKhungGios;
            ViewBag.BacSi = findBacSi;

            return View(lichLamViec);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DateTime ngayLamViec, List<string> khungGios, int soLuongToiDa)
        {
            var lichLamViec = await _lichLamViecRepository.GetByIdAsync(id);
            if (lichLamViec == null)
            {
                return NotFound();
            }

            // Lấy danh sách lịch làm việc hiện tại của bác sĩ theo ngày
            var existingSchedules = await _lichLamViecRepository.getListLichLamViecByBacSiIdAndDate(lichLamViec.BacSiId, ngayLamViec);
            var existingScheduleDict = existingSchedules.ToDictionary(l => l.KhungThoiGianId);

            // Danh sách ID khung giờ trong database
            var existingKhungGioIds = existingScheduleDict.Keys;

            // Danh sách ID khung giờ được gửi lên (convert sang int)
            var newKhungGioIds = khungGios.Select(int.Parse).ToList();

            // Các khung giờ cần xóa (có trong database nhưng không được gửi lên)
            var khungGiosToDelete = existingKhungGioIds.Except(newKhungGioIds).ToList();

            // Các khung giờ cần thêm mới (có trong danh sách gửi lên nhưng không có trong database)
            var khungGiosToAdd = newKhungGioIds.Except(existingKhungGioIds).ToList();

            // Xóa các lịch làm việc không còn trong danh sách gửi lên
            foreach (var khungGioId in khungGiosToDelete)
            {
                var scheduleToDelete = existingScheduleDict[khungGioId];
                await _lichLamViecRepository.DeleteAsync(scheduleToDelete.Id);
            }

            // Thêm mới các lịch làm việc
            foreach (var khungGioId in khungGiosToAdd)
            {
                var newSchedule = new LichLamViec
                {
                    BacSiId = lichLamViec.BacSiId,
                    NgayLamViec = ngayLamViec,
                    KhungThoiGianId = khungGioId,
                    SoLuongToiDa = soLuongToiDa
                };
                await _lichLamViecRepository.AddAsync(newSchedule);
            }

            // Cập nhật các lịch làm việc hiện có
            foreach (var khungGioId in newKhungGioIds.Intersect(existingKhungGioIds))
            {
                var existingSchedule = existingScheduleDict[khungGioId];
                existingSchedule.SoLuongToiDa = soLuongToiDa;
                await _lichLamViecRepository.UpdateAsync(existingSchedule);
            }

            TempData["SuccessMessage"] = "Cập nhật lịch làm việc thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var lichLamViec = await _lichLamViecRepository.GetByIdAsync(id);
            if (lichLamViec == null)
            {
                return NotFound();
            }

            return View(lichLamViec);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _lichLamViecRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index)); // Chuyển hướng về trang danh sách sau khi xóa
        }
    }
}