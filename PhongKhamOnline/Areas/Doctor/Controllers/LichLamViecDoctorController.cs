using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using PhongKhamOnline.Models;
using PhongKhamOnline.Repositories;
using System.Data;

namespace PhongKhamOnline.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    //[Authorize(Roles = "doctor")]

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
        [AllowAnonymous] // Để cho mọi tài khoản truy cập
        public async Task<IActionResult> GetKhungGioDaDat(string idUser, DateTime ngayLamViec)
        {
            // Tìm bác sĩ dựa trên idUser (idUser có thể là ID của tài khoản hoặc bác sĩ)
            var findBacSi = await _bacSiRepository.GetByUserId(idUser);
            if (findBacSi == null)
            {
                return Json(new { error = "Không tìm thấy bác sĩ." });
            }
            // Lấy danh sách khung giờ đã đặt
            var existingSchedules = await _lichLamViecRepository.getListLichLamViecByBacSiIdAndDate(findBacSi.Id, ngayLamViec);
            if (existingSchedules == null || !existingSchedules.Any())
            {
                return Json(new { message = "Bác sĩ không có lịch rảnh hôm nay." });
            }
            var khungGioDaDat = existingSchedules.Select(l => new
            {
                Id = l.KhungThoiGianId,
                time = l.KhungThoiGian.Time,
                maxQuantity = l.SoLuongToiDa
            }).ToList();

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
                ModelState.AddModelError("NgayLamViec", "Vui lòng không chọn ngày đã qua.");
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

            TempData["SuccessMessage"] = "Lịch làm việc đã được thêm thành công.";

            return RedirectToAction(nameof(Create));
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
            return RedirectToAction(nameof(Edit));
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


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _lichLamViecRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index)); // Chuyển hướng về trang danh sách sau khi xóa
        }
        public IActionResult CreateByFile()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(IFormFile file, string idUser)
        {

            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn file Excel.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var currentUserEmail = currentUser.Email; // lấy email của user hiện tại

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Lấy sheet đầu tiên
                        int rowCount = worksheet.Dimension.Rows; // Số dòng
                        int colCount = worksheet.Dimension.Columns; // Số cột
                        bool hasErrorOccurred = false; // Biến flag để theo dõi lỗi
                        bool isAnyScheduleAdded = false; // Biến flag để theo dõi xem có lịch làm việc nào được thêm không
                        var khungThoiGianList = await _khungThoiGianRepository.GetAllAsync();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var bacSiName = worksheet.Cells[row, 1].Text.Trim();
                            var email = worksheet.Cells[row, 2].Text.Trim();
                            var ngayLamViecText = worksheet.Cells[row, 3].Text.Trim();
                            var thoiGianText = worksheet.Cells[row, 4].Text.Trim();
                            var soLuongToiDaText = worksheet.Cells[row, 5].Text.Trim();
                            var bacSi = await _bacSiRepository.GetByEmail(email);
                            if (bacSi == null)
                            {
                                TempData["ErrorMessage"] += $"Không tìm thấy email bác sĩ: {email} ở dòng {row}. <br>";
                                continue;
                            }

                            // Kiểm tra email trong tệp có khớp với email của người đăng nhập không
                            if (!string.Equals(email, currentUserEmail, StringComparison.OrdinalIgnoreCase))
                            {
                                TempData["ErrorMessage"] += $"Email {email} ở dòng {row} không khớp với email tài khoản đăng nhập. <br>";
                                continue;
                            }


                            if (!DateTime.TryParse(ngayLamViecText, out var ngayLamViec))
                            {
                                TempData["ErrorMessage"] += $"Ngày làm việc không hợp lệ ở dòng {row}. <br>";
                                continue;
                            }

                            // Kiểm tra nếu ngày làm việc là quá khứ
                            if (ngayLamViec.Date < DateTime.Now.Date)
                            {
                                TempData["ErrorMessage"] += $"Ngày làm việc {ngayLamViec:dd/MM/yyyy} ở dòng {row} là ngày trong quá khứ. <br>";
                                continue;
                            }

                            // Tách các khung giờ
                            var khungThoiGianTexts = thoiGianText.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                                .Select(t => t.Trim());
                            foreach (var khungGio in khungThoiGianTexts)
                            {
                                var khungThoiGian = khungThoiGianList.FirstOrDefault(ktg => ktg.Time == khungGio);
                                if (khungThoiGian == null)
                                {
                                    TempData["ErrorMessage"] += $"Không tìm thấy khung thời gian: {khungGio} ở dòng {row}. <br>";
                                    continue;
                                }

                                // Kiểm tra lịch làm việc đã tồn tại
                                bool isScheduleExists = await _lichLamViecRepository.GetLichDaTonTai(bacSi.Id, ngayLamViec, khungThoiGian.Id);
                                if (isScheduleExists)
                                {
                                    TempData["ErrorMessage"] += $"Lịch làm việc đã tồn tại cho bác sĩ {bacSi.Ten} vào ngày {ngayLamViec:dd/MM/yyyy} tại khung giờ {khungGio} dòng {row}. <br>";
                                    continue;
                                }

                                var newSchedule = new LichLamViec
                                {
                                    BacSiId = bacSi.Id,
                                    NgayLamViec = ngayLamViec,
                                    KhungThoiGianId = khungThoiGian.Id,
                                    SoLuongToiDa = Convert.ToInt32(soLuongToiDaText)
                                };

                                await _lichLamViecRepository.AddAsync(newSchedule);
                                // Đánh dấu là có lịch làm việc được thêm
                                isAnyScheduleAdded = true;
                                TempData["SuccessMessage"] += $"Lịch làm việc ngày {ngayLamViec:dd/MM/yyyy} khung giờ {khungGio} được thêm thành công.<br>";
                            }
                        }
                        // Kiểm tra và chỉ hiển thị thông báo thành công tổng thể khi không có lỗi và ít nhất một lịch làm việc được thêm
                        if (!hasErrorOccurred && isAnyScheduleAdded)
                        {
                            TempData["SuccessMessage"] += "Tải lên file Excel thành công.";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
            }

            return RedirectToAction(nameof(CreateByFile));
        }
    }
}