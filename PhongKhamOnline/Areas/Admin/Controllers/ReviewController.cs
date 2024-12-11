using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamOnline.DataAccess;
using PhongKhamOnline.Models;
using System.Security.Claims;

namespace PhongKhamOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ReviewController : Controller
    {
        private ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách đánh giá của bác sĩ (theo ID bác sĩ)
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kiểm tra vai trò của người dùng
            var isAdmin = User.IsInRole("admin");

            List<DoctorReview> reviews;

            if (isAdmin)
            {
                // Nếu là Admin, lấy tất cả các đánh giá
                reviews = await _context.doctorReviews
                                        .Include(r => r.BacSi) // Load thông tin bác sĩ nếu cần
                                        .ToListAsync();
            }
            else
            {
                // Tìm Id của bác sĩ liên kết với UserId
                var bacSi = await _context.BacSis.FirstOrDefaultAsync(b => b.UserId == userId);
                if (bacSi == null)
                {
                    return NotFound("Không tìm thấy bác sĩ liên kết với tài khoản này.");
                }

                // Lấy danh sách đánh giá chỉ thuộc về bác sĩ này
                reviews = await _context.doctorReviews
                                        .Include(r => r.BacSi) // Load thông tin bác sĩ nếu cần
                                        .Where(r => r.BacSiId == bacSi.Id)
                                        .ToListAsync();
            }

            // Truyền danh sách đánh giá vào View
            return View(reviews);
        }

        public async Task<IActionResult> DeleteReview(int id)
        {
            // Tìm đánh giá theo Id
            var review = await _context.doctorReviews.FindAsync(id);

            if (review == null)
            {
                return NotFound("Không tìm thấy đánh giá cần xóa.");
            }

            // Kiểm tra vai trò hoặc quyền truy cập
            var isAdmin = User.IsInRole("admin");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bacSi = await _context.BacSis.FirstOrDefaultAsync(b => b.UserId == userId);

            if (!isAdmin && (bacSi == null || review.BacSiId != bacSi.Id))
            {
                return Forbid("Bạn không có quyền xóa đánh giá này.");
            }

            // Xóa đánh giá
            _context.doctorReviews.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        public IActionResult Reply(int reviewId)
        {
            var review = _context.doctorReviews
                .Include(r => r.BacSi)
                .FirstOrDefault(r => r.Id == reviewId);

            if (review == null) return NotFound();

            return View(review);
        }

        [HttpPost]
        public IActionResult Reply(int reviewId, string reply)
        {
            var review = _context.doctorReviews.Find(reviewId);

            if (review != null && !string.IsNullOrEmpty(reply))
            {
                review.Reply = reply;
                review.RepliedAt = DateTime.Now;

                _context.doctorReviews.Update(review);
                _context.SaveChanges();
                return RedirectToAction("Index", new { doctorId = review.BacSi });
            }

            return View(review);
        }

        public IActionResult ShowReviews(int selectedDoctorId)
        {
            var reviews = _context.doctorReviews.Include(r => r.BacSi).ToList();
            ViewBag.SelectedDoctorId = selectedDoctorId; // Truyền ID bác sĩ được chọn vào ViewBag
            return View(reviews);
        }
    }
}
