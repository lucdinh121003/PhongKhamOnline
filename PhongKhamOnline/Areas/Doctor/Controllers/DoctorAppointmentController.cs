using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhongKhamOnline.Models;
using PhongKhamOnline.Repositories;
using System.Security.Claims;

namespace PhongKhamOnline.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "doctor")]
    public class DoctorAppointmentController : Controller
    {
        private readonly IDatLichKhamRepository _datLichKhamRepository;
        private readonly IBacSiRepository _bacSiRepository;
        private readonly IKhungThoiGianRepository _khungThoiGianRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public DoctorAppointmentController(UserManager<ApplicationUser> userManager, IDatLichKhamRepository datLichKhamRepository, IBacSiRepository bacSiRepository, IKhungThoiGianRepository khungThoiGianRepository)
        {
            _datLichKhamRepository = datLichKhamRepository;
            _bacSiRepository = bacSiRepository;
            _khungThoiGianRepository = khungThoiGianRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(AppointmentStatus? status = null)
        {
            IEnumerable<Appointment> appointments;

            if (User.IsInRole("admin"))
            {
                // Admin: Hiển thị tất cả lịch khám
                appointments = status.HasValue
                    ? await _datLichKhamRepository.GetByStatusAsync(status.Value)
                    : await _datLichKhamRepository.GetAllAsync();
            }
            else if (User.IsInRole("doctor"))
            {
                // Doctor: Hiển thị lịch khám của bác sĩ đăng nhập
                var doctorId = await GetDoctorIdAsync();
                if (doctorId == null)
                {
                    return Unauthorized(); // Nếu không tìm thấy bác sĩ, từ chối truy cập
                }
                appointments = status.HasValue
                    ? await _datLichKhamRepository.GetByDoctorAndStatusAsync(doctorId.Value, status.Value)
                    : await _datLichKhamRepository.GetByDoctorAsync(doctorId.Value);
            }
            else
            {
                // Patient: Hiển thị lịch sử đặt lịch của tài khoản đăng nhập
                var patientId = _userManager.GetUserId(User); // Lấy UserId của bệnh nhân đăng nhập
                appointments = status.HasValue
                    ? await _datLichKhamRepository.GetByPatientAndStatusAsync(patientId, status.Value)
                    : await _datLichKhamRepository.GetByPatientAsync(patientId);
            }


            return View(appointments);
        }

        public async Task<IActionResult> Index(DateTime? selectedDate)
        {
            IEnumerable<Appointment> appointments;

            if (selectedDate.HasValue!)
            {
                // Lọc dữ liệu theo ngày được truyền qua query string
                appointments = await _datLichKhamRepository.GetByDateAsync(selectedDate.Value);
            }
            else
            {
                // Nếu không truyền ngày, lấy tất cả lịch khám
                appointments = await _datLichKhamRepository.GetAllAsync();
            }

            return View(appointments);
        }

        private async Task<int?> GetDoctorIdAsync()
        {
            var userId = _userManager.GetUserId(User); // Lấy UserId của tài khoản
            var doctor = await _bacSiRepository.GetByUserIdAsync(userId); // Tìm bác sĩ theo UserId
            return doctor?.Id;
        }





        [HttpGet]
        public async Task<IActionResult> GetKhungGioDaDat(int idUser, string ngayLamViec)
        {
            var timeSlots = await _datLichKhamRepository.GetAvailableTimeSlotsAsync(idUser, ngayLamViec);
            return Json(timeSlots.Select(k => new { id = k.Id, time = k.Time }));
        }

        [HttpGet]
        public async Task<IActionResult> DatLich(string ngay, int timeId, int idBacSi)
        {
            var doctor = await _bacSiRepository.GetByIdAsync(idBacSi);
            if (doctor == null)
            {
                return NotFound("Không tìm thấy thông tin bác sĩ.");
            }

            var timeSlot = await _khungThoiGianRepository.GetById(timeId);
            if (timeSlot == null)
            {
                return NotFound("Không tìm thấy thông tin khung giờ.");
            }

            var model = new Appointment
            {
                NgayKham = DateTime.Parse(ngay),
                KhungThoiGianId = timeSlot.Id,
                KhungThoiGian = timeSlot,
                BacSiId = idBacSi,
                BacSi = doctor
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatLich(Appointment appointment)
        {
            if (ModelState.IsValid != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                appointment.UserId = userId;
                appointment.CreatedAt = DateTime.Now;
                appointment.Status = AppointmentStatus.Pending;
                await _datLichKhamRepository.AddAsync(appointment);

                return RedirectToAction("Index", "Appointment");
            }

            return View(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); // Lấy thông tin người dùng đăng nhập
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(); // Lấy role của người dùng
            var userId = user.Id; // Lấy UserId

            IEnumerable<Appointment> appointments;

            if (role == "admin")
            {
                // Admin: Hiển thị tất cả lịch khám
                appointments = await _datLichKhamRepository.GetAllAsync();
            }
            else if (role == "doctor")
            {
                // Doctor: Hiển thị lịch khám liên quan đến bác sĩ hiện tại
                var doctor = await _bacSiRepository.GetByUserIdAsync(userId);
                if (doctor == null)
                {
                    return View(new List<Appointment>()); // Nếu không tìm thấy bác sĩ, trả về danh sách rỗng
                }

                appointments = await _datLichKhamRepository.GetByDoctorIdAsync(doctor.Id);
            }
            else
            {
                // Patient: Hiển thị lịch sử đặt lịch của tài khoản hiện tại
                appointments = await _datLichKhamRepository.GetByUserIdAsync(userId);
            }

            return View(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _datLichKhamRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = AppointmentStatus.Cancelled; // Cập nhật trạng thái thành Cancelled
            await _datLichKhamRepository.UpdateAsync(appointment);

            TempData["Message"] = "Đã hủy lịch khám thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            var appointment = await _datLichKhamRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = AppointmentStatus.Confirmed; // Cập nhật trạng thái thành Confirmed
            await _datLichKhamRepository.UpdateAsync(appointment);

            TempData["Message"] = "Đã xác nhận lịch khám thành công!";
            return RedirectToAction("Index");
        }

    }
}
