using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhongKhamOnline.DataAccess;
using PhongKhamOnline.Models;
using PhongKhamOnline.Repositories;
using System.Numerics;
using System.Security.Claims;

namespace PhongKhamOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ReviewController : Controller
    {
        private readonly IDoctorReviewRepository _reviewRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IBacSiRepository _bacSiRepository;


        public ReviewController(IBacSiRepository bacSiRepository, ApplicationDbContext context, IDoctorReviewRepository reviewRepository, UserManager<ApplicationUser> userManager)
        {
            _reviewRepository = reviewRepository;
            _userManager = userManager;
            _context = context;
            _bacSiRepository = bacSiRepository;
        }
        public async Task<IActionResult> Index()
        {
            var reviews = await _reviewRepository.GetAllReviewsAsync();
            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            // Lấy thông tin người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Lấy đánh giá theo ID
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null)
            {
                return NotFound("Đánh giá không tồn tại.");
            }

            // Kiểm tra quyền
            if (User.IsInRole("admin") || review.UserId == userId)
            {
                // Xóa đánh giá
                await _reviewRepository.DeleteReviewAsync(reviewId);

                TempData["SuccessMessage"] = "Đánh giá đã được xóa thành công.";
                
                    return RedirectToAction("Index");

            }
            else
            {
                return Unauthorized("Bạn không có quyền xóa đánh giá này.");
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditReply(int id)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kiểm tra nếu là Admin thì bỏ qua kiểm tra bác sĩ
            if (User.IsInRole("admin"))
            {
                return View(review);
            }

            var doctor = await _bacSiRepository.GetByUserIdAsync(userId);

            if (doctor == null || review.BacSiId != doctor.Id)
            {
                return Unauthorized(); // Từ chối nếu không phải bác sĩ của đánh giá này
            }

            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReply(DoctorReview review)
        {
            if (ModelState.IsValid!=null)
            {
                var updatedReview = await _reviewRepository.GetReviewByIdAsync(review.Id);
                if (updatedReview == null)
                {
                    return NotFound();
                }

                // Nếu là Admin, chỉ cần cập nhật phản hồi
                if (User.IsInRole("admin"))
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

        [HttpGet]
        public async Task<IActionResult> EditReview(int id)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Nếu là Admin, bỏ qua kiểm tra quyền sở hữu
            if (User.IsInRole("admin") || review.UserId == userId)
            {
                return View(review);
            }

            return Unauthorized(); // Từ chối nếu không phải chủ sở hữu và không phải Admin
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReview(DoctorReview review)
        {
            if (ModelState.IsValid!=null)
            {
                var updatedReview = await _reviewRepository.GetReviewByIdAsync(review.Id);
                if (updatedReview == null)
                {
                    return NotFound();
                }

                // Nếu là Admin, cho phép chỉnh sửa bất kỳ đánh giá nào
                if (User.IsInRole("admin") || updatedReview.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    updatedReview.ReviewText = review.ReviewText;
                    updatedReview.Rating = review.Rating;

                    await _reviewRepository.UpdateReviewAsync(updatedReview);

                    // Nếu là Admin, chuyển hướng về trang danh sách tất cả đánh giá
                    if (User.IsInRole("admin"))
                    {
                        return RedirectToAction("Index", "Review", new { area = "Admin" });
                    }

                }

                return Unauthorized(); // Từ chối nếu không có quyền
            }

            return View(review);
        }


    }
}
