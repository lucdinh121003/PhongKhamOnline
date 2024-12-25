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
        private readonly IBacSiRepository _BacSiRepository;
        private readonly IChuyenMonBacSiRepository _ChuyenMonBacSiRepository;
        private IEnumerable<BacSi> listDoctor = [];
        private IEnumerable<ChuyenMonBacSi> listSpecialties = [];
        private readonly ApplicationDbContext _context;

        public BacSiController(IBacSiRepository BacSiRepository, IChuyenMonBacSiRepository ChuyenMonBacSiRepository, ApplicationDbContext context)
        {
            _BacSiRepository = BacSiRepository;
            _ChuyenMonBacSiRepository = ChuyenMonBacSiRepository;
            _context = context;

        }

        public async Task<IActionResult> Index(int? specialtyId)
        {
            // Lấy danh sách chuyên môn và bác sĩ
            listSpecialties = await _ChuyenMonBacSiRepository.GetAllAsync();

            // Lọc bác sĩ theo chuyên môn nếu specialtyId được chỉ định
            IEnumerable<BacSi> BacSis;
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
