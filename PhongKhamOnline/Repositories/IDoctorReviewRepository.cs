using PhongKhamOnline.Models;

namespace PhongKhamOnline.Repositories
{
    public interface IDoctorReviewRepository
    {
        Task<List<DoctorReview>> GetReviewsByDoctorAsync(int doctorId);
        Task<DoctorReview> GetReviewByIdAsync(int reviewId);
        Task AddReviewAsync(DoctorReview review);
        Task UpdateReviewAsync(DoctorReview review);
        Task DeleteReviewAsync(int reviewId);
        Task<List<DoctorReview>> GetReviewsByUserIdAsync(string userId);

        Task<IEnumerable<DoctorReview>> GetReviewsByDoctorIdAsync(int doctorId);
        Task<IEnumerable<DoctorReview>> GetAllReviewsAsync();
    }
}
