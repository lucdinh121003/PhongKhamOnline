using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PhongKhamOnline.Models;
using System.Security.Claims;
using System;
using PhongKhamOnline.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace PhongKhamOnline.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Appointment> appointments;

            if (User.IsInRole("admin") || User.IsInRole("customer"))
            {
                appointments = _context.Appointments.Include(a => a.BacSi).ToList();
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                appointments = _context.Appointments.Include(a => a.BacSi).Where(a => a.UserId == userId).ToList();
            }

            return View(appointments);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Register", "Account"); // Chuyển hướng đến trang đăng ký
            }
            var bacSis = _context.BacSis.ToList(); // Lấy danh sách tất cả các bác sĩ từ cơ sở dữ liệu
            ViewBag.BacSis = new SelectList(bacSis, "Id", "Ten"); // Truyền danh sách bác sĩ sang view để tạo dropdownlist
            return View();
        }

        [HttpPost]
        public IActionResult Create(Appointment appointment)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Register", "Account"); // Chuyển hướng đến trang đăng ký
            }
            if (ModelState.IsValid!=null)
            {
                appointment.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Đặt lịch thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.BacSis = new SelectList(_context.BacSis, "Id", "Ten", appointment.BacSiId);
            return View(appointment);
        }



        [HttpPost]
        public IActionResult Confirm(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment != null && appointment.Status == AppointmentStatus.Pending)
            {
                appointment.Status = AppointmentStatus.Confirmed;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Cancel(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment != null && appointment.Status == AppointmentStatus.Pending)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }

}
