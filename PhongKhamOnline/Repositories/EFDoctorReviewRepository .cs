using Microsoft.EntityFrameworkCore;
using PhongKhamOnline.DataAccess;
using PhongKhamOnline.Models;

namespace PhongKhamOnline.Repositories
{
    public class DoctorReviewRepository : IDoctorReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<DoctorReview>> GetAllReviewsAsync()
        {
            return await _context.doctorReviews
                .Include(r => r.BacSi) // Bao gồm thông tin bác sĩ
                .ToListAsync();
        }
        public async Task<List<DoctorReview>> GetReviewsByDoctorAsync(int doctorId)
        {
            return await _context.doctorReviews
                                 .Where(r => r.BacSiId == doctorId)
                                 .ToListAsync();
        }

        public async Task<DoctorReview> GetReviewByIdAsync(int reviewId)
        {
            return await _context.doctorReviews
                                 .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task AddReviewAsync(DoctorReview review)
        {
            _context.doctorReviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReviewAsync(DoctorReview review)
        {
            _context.doctorReviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var review = await GetReviewByIdAsync(reviewId);
            if (review != null)
            {
                _context.doctorReviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<DoctorReview>> GetReviewsByUserIdAsync(string userId)
        {
            return await _context.doctorReviews
                .Where(r => r.UserId == userId)
                .Include(r => r.BacSi)  // Bao gồm thông tin bác sĩ
                .ToListAsync();  // Lấy tất cả các đánh giá của người dùng
        }

        public async Task<IEnumerable<DoctorReview>> GetReviewsByDoctorIdAsync(int doctorId)
        {
            return await _context.doctorReviews
                .Where(r => r.BacSiId == doctorId) // Lọc theo BacSiId
                .ToListAsync();
        }
    }
}
