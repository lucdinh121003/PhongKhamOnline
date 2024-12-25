using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhongKhamOnline.DataAccess;
using PhongKhamOnline.Models;
using PhongKhamOnline.Repositories;
using System.Security.Claims;

namespace PhongKhamOnline.Controllers
{
    public class BenhNhanReviewController : Controller
    {
        private readonly IDoctorReviewRepository _reviewRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IBacSiRepository _bacSiRepository;

        public BenhNhanReviewController(IBacSiRepository bacSiRepository,ApplicationDbContext context, IDoctorReviewRepository reviewRepository, UserManager<ApplicationUser> userManager)
        {
            _reviewRepository = reviewRepository;
            _userManager = userManager;
            _context = context;
            _bacSiRepository = bacSiRepository;
        }

        // GET: DoctorReview/Index
        public IActionResult Index(int bacSiId)
        {
            // Lấy UserId của người dùng đang đăng nhập
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Lọc danh sách đánh giá chỉ thuộc về bác sĩ được chọn
            var reviews = _context.doctorReviews
                          .Include(r => r.BacSi) // Tải thông tin bác sĩ liên quan
                          .Where(r => r.BacSiId == bacSiId) // Lọc theo ID bác sĩ
                          .ToList();

            // Truyền ID bác sĩ được chọn vào ViewBag nếu cần sử dụng trong View
            ViewBag.BacSiId = bacSiId;

            // Trả danh sách đánh giá về View
            return View(reviews);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int bacSiId)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Register", "Account");  // Chuyển hướng đến trang đăng ký
            }

            var doctor = await _bacSiRepository.GetByIdAsync(bacSiId);
            if (doctor == null)
            {
                return NotFound("Không tìm thấy thông tin bác sĩ.");  // Nếu không tìm thấy bác sĩ
            }

            // Lưu thông tin bác sĩ vào ViewBag để hiển thị trên view
            ViewBag.BacSiId = bacSiId;
            ViewBag.BacSiName = doctor.Ten;

            return View();
        }

        // POST: Create Review
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorReview review)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Register", "Account");  // Chuyển hướng đến trang đăng ký
            }

            if (ModelState.IsValid != null)
            {
                // Lấy thông tin người dùng hiện tại
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                review.UserId = userId;
                review.CreatedAt = DateTime.Now;  // Gán thời gian tạo đánh giá

                // Gọi phương thức AddReviewAsync để lưu đánh giá vào cơ sở dữ liệu
                await _reviewRepository.AddReviewAsync(review);

                return RedirectToAction("ReviewHistory");  // Quay lại danh sách đánh giá của người dùng
            }

            // Nếu model không hợp lệ, hiển thị lại thông tin bác sĩ
            ViewBag.BacSiId = review.BacSiId;
            return View(review);
        }

        [HttpGet]
        public async Task<IActionResult> EditReview(int id)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kiểm tra xem người dùng hiện tại có phải là người đã tạo đánh giá này không
            if (review.UserId != userId)
            {
                return Unauthorized();  // Nếu không phải người tạo đánh giá, từ chối chỉnh sửa
            }

            return View(review);  // Trả về view để bệnh nhân sửa đánh giá
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReview(DoctorReview review)
        {
            if (ModelState.IsValid !=null)
            {
                var updatedReview = await _reviewRepository.GetReviewByIdAsync(review.Id);
                if (updatedReview == null)
                {
                    return NotFound();
                }

                updatedReview.ReviewText = review.ReviewText;
                updatedReview.Rating = review.Rating;


                await _reviewRepository.UpdateReviewAsync(updatedReview);  // Cập nhật vào cơ sở dữ liệu

                return RedirectToAction(nameof(ReviewHistory));  // Quay lại danh sách đánh giá
            }

            return View(review);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the review by ID
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null || review.UserId != userId)
            {
                return NotFound("Đánh giá không tồn tại hoặc không thuộc về người dùng này.");
            }

            // Delete the review
            await _reviewRepository.DeleteReviewAsync(reviewId);

            TempData["SuccessMessage"] = "Đánh giá đã được xóa thành công.";
            return RedirectToAction("ReviewHistory");
        }


        [HttpGet]
        public async Task<IActionResult> ReviewHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy ID của người dùng hiện tại
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Register", "Account"); // Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng ký
            }

            // Lấy tất cả đánh giá của người dùng
            var reviews = await _reviewRepository.GetReviewsByUserIdAsync(userId);

            return View(reviews);  // Trả về View với danh sách các đánh giá của người dùng
        }
    }
}
