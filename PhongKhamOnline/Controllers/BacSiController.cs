using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhongKhamOnline.DataAccess;
using PhongKhamOnline.Models;
using PhongKhamOnline.Repositories;

namespace PhongKhamOnline.Controllers
{
    
    public class BacSiController : Controller
    {
        private readonly IDatLichKhamRepository _datLichKhamRepository;
        private readonly IBacSiRepository _BacSiRepository;
        private readonly ILichLamViecRepository _lichLamViecRepository;
        private readonly IChuyenMonBacSiRepository _ChuyenMonBacSiRepository;
        private IEnumerable<BacSi> listDoctor = [];
        private IEnumerable<ChuyenMonBacSi> listSpecialties = [];
        private readonly ApplicationDbContext _context;


        public BacSiController(IDatLichKhamRepository datLichKhamRepository, IBacSiRepository BacSiRepository, IChuyenMonBacSiRepository ChuyenMonBacSiRepository, ApplicationDbContext context)
        {
            _datLichKhamRepository = datLichKhamRepository;
            _BacSiRepository = BacSiRepository;
            _ChuyenMonBacSiRepository = ChuyenMonBacSiRepository;
            _context = context;

        }

        public async Task<IActionResult> Index(int? specialtyId)
        {
            // Lấy danh sách chuyên môn và bác sĩ
            listSpecialties = await _ChuyenMonBacSiRepository.GetAllAsync();

            // Lọc bác sĩ theo chuyên môn nếu specialtyId được chỉ định
            IEnumerable<BacSi> BacSis;// Khai báo một danh sách
            if (specialtyId.HasValue && specialtyId.Value > 0)
            {
                BacSis = await _BacSiRepository.GetBySpecialtyAsync(specialtyId.Value);
            }
            else
            {
                BacSis = await _BacSiRepository.GetAllAsync();
            }

            var viewModel = new DoctorViewModel
            {
                BacSis = BacSis,
                ChuyenMons = listSpecialties,
                SelectedSpecialtyId = specialtyId ?? 0
            };

            return View(viewModel);
        }


        [HttpGet]
        [AllowAnonymous] // Để cho mọi tài khoản truy cập
        public async Task<IActionResult> GetKhungGioDaDatOfUser(string idUser, DateTime ngayLamViec, int idBacSi)
        {
            // Lấy danh sách lịch làm việc của bác sĩ
            var lichLamViecBacSi = await _lichLamViecRepository.getListLichLamViecByBacSiIdAndDate(idBacSi, ngayLamViec);

            // Kiểm tra nếu bác sĩ không có lịch làm việc hôm nay
            if (lichLamViecBacSi == null || !lichLamViecBacSi.Any())
            {
                return Json(new { message = "Bác sĩ không có thời gian rảnh hôm nay." });
            }

            // Lấy danh sách lịch khám của user với bác sĩ đó
            var lichKhamUser = await _datLichKhamRepository.GetByPatientAsync(idUser);
            var lichKhamCuaBacSi = lichKhamUser
                .Where(a => a.BacSiId == idBacSi && a.NgayKham.Date == ngayLamViec.Date)
                .Select(a => a.KhungThoiGianId)
                .ToList();

            // Lấy danh sách các khung giờ đã được đặt
            var khungGioDaDatOfUser = lichLamViecBacSi
                .Where(l => lichKhamCuaBacSi.Contains(l.KhungThoiGianId))
                .Select(l => new
                {
                    l.KhungThoiGianId,
                    l.KhungThoiGian.Time
                })
                .ToList();        

            return Json(khungGioDaDatOfUser);
        }

        public async Task<IActionResult> Display(int id)
        {
            var bacSi = await _BacSiRepository.GetByIdAsync(id); // Lấy thông tin bác sĩ
            if (bacSi == null)
            {
                return NotFound();
            }

            // Lấy tổng số đánh giá của bác sĩ
            var totalReviews = await _context.doctorReviews
                .Where(r => r.BacSiId == id)
                .CountAsync();

            // Nếu bác sĩ chưa có đánh giá, gán giá trị trung bình là null hoặc 0
            if (totalReviews == 0)
            {
                ViewBag.AverageRating = null; // Hoặc ViewBag.AverageRating = 0;
                ViewBag.TotalReviews = totalReviews;
            }
            else
            {
                // Tính trung bình số sao
                var averageRating = await _context.doctorReviews
                    .Where(r => r.BacSiId == id)
                    .AverageAsync(r => r.Rating); // Tính trung bình số sao

                // Rút gọn số sao trung bình đến 1 chữ số sau dấu phẩy (ví dụ 4.5 thay vì 4.555...)
                var roundedRating = Math.Round(averageRating, 1);

                // Gán giá trị vào ViewBag
                ViewBag.AverageRating = roundedRating;
                ViewBag.TotalReviews = totalReviews;

            }

            return View(bacSi);
        }

        public async Task<IActionResult> Search(string searchString)
        {

            listSpecialties = await _ChuyenMonBacSiRepository.GetAllAsync();

            IEnumerable<BacSi> filteredListDoctor;

            if (string.IsNullOrEmpty(searchString))
            {
                filteredListDoctor = await _BacSiRepository.GetAllAsync();
            }
            else
            {

                filteredListDoctor = await _BacSiRepository.searchDoctor(searchString);
            }


            var viewModel = new DoctorViewModel
            {
                BacSis = filteredListDoctor,
                ChuyenMons = listSpecialties,
                SelectedSpecialtyId = 0
            };

            return View("Index", viewModel);
        }


    }
}
