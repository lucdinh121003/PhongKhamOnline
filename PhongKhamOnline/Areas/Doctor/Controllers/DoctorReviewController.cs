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
    [Area("Doctor")]
    [Authorize(Roles = "doctor")]
    public class DoctorReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách đánh giá của bác sĩ (theo ID bác sĩ)
        public IActionResult Index()
        {
            List<DoctorReview> reviews;

            if (User.IsInRole("admin") || User.IsInRole("doctor"))
            {
                reviews = _context.doctorReviews.Include(a => a.BacSi).ToList();
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                reviews = _context.doctorReviews.Include(a => a.BacSi).Where(a => a.UserId == userId).ToList();
            }
            return View(reviews);
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