using PhongKhamOnline.Models;


namespace PhongKhamOnline.Repositories
{
    public interface IBacSiRepository
    {
        Task<IEnumerable<BacSi>> GetAllAsync();
        Task<IEnumerable<BacSi>> searchDoctor(string value);
        Task<IEnumerable<BacSi>> GetBySpecialtyAsync(int specialtyId);
        Task<BacSi> GetByIdAsync(int id);
        Task AddAsync(BacSi bacSi);
        Task UpdateAsync(BacSi bacSi);
        Task DeleteAsync(int id);
        Task<BacSi> GetByUserId(string userId);

        Task<BacSi> GetByEmail(string email);
        Task<BacSi> GetByUserIdAsync(string userId);

    }

}
