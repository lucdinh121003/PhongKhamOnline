using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamOnline.DataAccess;
using PhongKhamOnline.Models;
using PhongKhamOnline.Repositories;
using System.Security.Claims;

namespace PhongKhamOnline.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "doctor")]
    public class DoctorReviewController : Controller
    {
        private readonly IDoctorReviewRepository _reviewRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IBacSiRepository _bacSiRepository;


        public DoctorReviewController(IBacSiRepository bacSiRepository, ApplicationDbContext context, IDoctorReviewRepository reviewRepository, UserManager<ApplicationUser> userManager)
        {
            _reviewRepository = reviewRepository;
            _userManager = userManager;
            _context = context;
            _bacSiRepository = bacSiRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Lấy ID người dùng hiện tại
            var doctor = await _bacSiRepository.GetByUserIdAsync(userId);  // Lấy bác sĩ theo UserId

            if (doctor == null)
            {
                return Unauthorized();  // Nếu không phải bác sĩ, từ chối truy cập
            }

            // Lấy danh sách các đánh giá của bác sĩ
            var reviews = await _reviewRepository.GetReviewsByDoctorIdAsync(doctor.Id);
            return View(reviews);  // Truyền danh sách đánh giá vào view
        }


        [HttpGet]
        public async Task<IActionResult> EditReply(int id)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu bác sĩ đang đăng nhập có phải là bác sĩ của đánh giá này không
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _bacSiRepository.GetByUserIdAsync(userId);

            if (doctor == null || review.BacSiId != doctor.Id)
            {
                return Unauthorized();  // Nếu bác sĩ không đúng thì từ chối
            }

            return View(review);  // Trả về view để bác sĩ chỉnh sửa phản hồi
        }

        // Cập nhật phản hồi của bác sĩ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReply(DoctorReview review)
        {
            if (ModelState.IsValid != null)
            {
                var updatedReview = await _reviewRepository.GetReviewByIdAsync(review.Id);
                if (updatedReview == null)
                {
                    return NotFound();
                }

                if (User.IsInRole("doctor"))
                {
                    updatedReview.Reply = review.Reply;
                    updatedReview.RepliedAt = DateTime.Now;
                }
                else
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var doctor = await _bacSiRepository.GetByUserIdAsync(userId);

                    if (doctor == null || updatedReview.BacSiId != doctor.Id)
                    {
                        return Unauthorized(); // Từ chối nếu không phải bác sĩ của đánh giá này
                    }

                    updatedReview.Reply = review.Reply;
                    updatedReview.RepliedAt = DateTime.Now;
                }

                await _reviewRepository.UpdateReviewAsync(updatedReview);

                return RedirectToAction(nameof(Index)); // Quay lại danh sách đánh giá
            }
            return View(review);
        }
    }
}
