using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhongKhamOnline.DataAccess;
using PhongKhamOnline.Models;

using System;
using System.Security.Claims;

namespace PhongKhamOnline.Controllers
{
    [Authorize]
    public class BNDoctorReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BNDoctorReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách đánh giá của bác sĩ (theo ID bác sĩ)
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



        // Tạo đánh giá mới
        public IActionResult Create(int bacSiId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Register", "Account"); // Chuyển hướng đến trang đăng ký
            }
            var bacSi = _context.BacSis.FirstOrDefault(b => b.Id == bacSiId);
            if (bacSi == null)
            {
                return NotFound(); // Nếu không tìm thấy bác sĩ, trả về lỗi 404
            }

            // Lưu ID của bác sĩ vào ViewBag để sử dụng trong View
            ViewBag.BacSiId = bacSiId;
            ViewBag.BacSiName = bacSi.Ten; // Lưu tên bác sĩ (nếu cần hiển thị)

            return View();
        }

        // Xử lý tạo đánh giá mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorReview review)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Register", "Account"); // Chuyển hướng đến trang đăng ký
            }
            if (ModelState.IsValid == false)
            {
                review.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                review.CreatedAt = DateTime.Now; // Gán thời gian tạo đánh giá
                review.RepliedAt = null;
                _context.doctorReviews.Add(review); // Thêm đánh giá vào DbContext
                _context.SaveChanges();// Lưu thay đổi vào database
                return RedirectToAction("ReviewHistory"); // Quay lại danh sách đánh giá
            }
            ViewBag.BacSis = new SelectList(_context.BacSis, "Id", "Ten", review.BacSiId);
            return View(review);
        }

        public IActionResult ReviewHistory()
        {
            // Lấy UserId của người dùng đang đăng nhập
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kiểm tra nếu người dùng chưa đăng nhập
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("Bạn cần đăng nhập để xem lịch sử đánh giá.");
            }

            // Lấy danh sách đánh giá của người dùng hiện tại
            var reviews = _context.doctorReviews
                          .Include(r => r.BacSi) // Bao gồm thông tin bác sĩ
                          .Where(r => r.UserId == currentUserId) // Lọc theo UserId
                          .ToList();

            // Trả danh sách đánh giá về View
            return View(reviews);
        }

    }
}