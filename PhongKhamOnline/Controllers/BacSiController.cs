using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public BacSiController(IBacSiRepository BacSiRepository, IChuyenMonBacSiRepository ChuyenMonBacSiRepository)
        {
            _BacSiRepository = BacSiRepository;
            _ChuyenMonBacSiRepository = ChuyenMonBacSiRepository;

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
            var bacSi = await _BacSiRepository.GetByIdAsync(id);
            if (bacSi == null)
            {
                return NotFound();
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
